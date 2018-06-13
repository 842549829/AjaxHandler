using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace AjaxHandler
{
    /// <summary>
    /// WebAjax处理类
    /// </summary>
    public class WebAjaxHandler : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// 实例可再次使用，则为 true；否则为 false。
        /// </summary>
        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// 当前Session
        /// </summary>
        protected HttpSessionState Session
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        /// <summary>
        /// 当前Application
        /// </summary>
        protected HttpApplicationState Application
        {
            get
            {
                return HttpContext.Current.Application;
            }
        }

        /// <summary>
        /// 通过实现 System.Web.IHttpHandler 接口的自定义 HttpHandler 启用 HTTP Web 请求的处理。
        /// </summary>
        /// <param name="context">它提供对用于为 HTTP 请求提供服务的内部服务器对象（如 Request、Response、Session和 Server）的引用。</param>
        public void ProcessRequest(HttpContext context)
        {
            WebRequestDecoder webRequestDecoder = WebRequestDecoder.CreateInstance(context);
            if (webRequestDecoder == null)
            {
                webRequestDecoder = new SimpleUrlDecoder(context);
            }
            string logicalMethod = webRequestDecoder.LogicalMethod;
            ResponseAnnotationAttribute methodAnnotation = this.GetMethodAnnotation(logicalMethod);
            WebResponseEncoder webResponseEncoder = WebResponseEncoder.CreateInstance(context, methodAnnotation.ResponseFormat);
            if (webResponseEncoder == null)
            {
                webResponseEncoder = new JQueryScriptEncoder(context);
            }
            try
            {
                this.Pre_Invoke(context);
                object srcObj = this.InvokeMethod(logicalMethod, webRequestDecoder.Deserialize());
                webResponseEncoder.Write(srcObj);
            }
            catch (Exception ex)
            {
                string message = (ex.InnerException == null) ? ex.Message : ex.InnerException.Message;
                this.ResponseException(context, message, webResponseEncoder);
            }
            WebAjaxHandler.InitializeCachePolicy(context, methodAnnotation);
        }

        /// <summary>
        /// 创建解码器
        /// </summary>
        /// <param name="context">当前上下文</param>
        /// <returns>解码器</returns>
        protected virtual WebRequestDecoder OnDecodeResolve(HttpContext context)
        {
            return new SimpleUrlDecoder(context);
        }

        /// <summary>
        /// 获取请求方法特性
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <returns>特性对象</returns>
        private ResponseAnnotationAttribute GetMethodAnnotation(string methodName)
        {
            MethodInfo method = base.GetType().GetMethod(methodName);
            ResponseAnnotationAttribute[] array = (ResponseAnnotationAttribute[])method.GetCustomAttributes(typeof(ResponseAnnotationAttribute), false);
            ResponseAnnotationAttribute result = ResponseAnnotationAttribute.Default;
            if (array.Length > 0)
            {
                result = array[0];
            }
            return result;
        }

        /// <summary>
        /// Pre_Invoke
        /// </summary>
        /// <param name="context">当前上下文</param>
        protected virtual void Pre_Invoke(HttpContext context)
        {
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="args">参数</param>
        /// <returns>结果</returns>
        private object InvokeMethod(string methodName, Dictionary<string, object> args)
        {
            MethodInfo method = base.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
            {
                throw new ApplicationException("指定的逻辑方法不存在");
            }
            object[] arguments = this.GetArguments(method, args);
            return method.Invoke(this, arguments);
        }

        /// <summary>
        /// 参数解析
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="args">参数</param>
        /// <returns>结果</returns>
        private object[] GetArguments(MethodInfo method, Dictionary<string, object> args)
        {
            List<object> list = new List<object>();
            if (method != null)
            {
                ParameterInfo[] parameters = method.GetParameters();
                ParameterInfo[] array = parameters;
                for (int i = 0; i < array.Length; i++)
                {
                    ParameterInfo parameterInfo = array[i];
                    if (args != null && args.ContainsKey(parameterInfo.Name))
                    {
                        object obj = args[parameterInfo.Name];
                        if (obj != null)
                        {
                            Type parameterType = WebAjaxHandler.GetParameterType(parameterInfo);
                            if (obj is IDictionary<string, object> || obj is Array)
                            {
                                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                                string input = javaScriptSerializer.Serialize(obj);
                                list.Add(javaScriptSerializer.Deserialize(input, parameterType));
                            }

                            else if (obj.GetType() != parameterType)
                            {
                                if (parameterType.IsEnum)
                                {
                                    if (obj is int)
                                    {
                                        list.Add(Enum.ToObject(parameterType, obj));
                                    }
                                    else
                                    {
                                        list.Add(Enum.Parse(parameterType, obj.ToString(), true));
                                    }
                                }
                                else if (parameterType == typeof(Guid))
                                {
                                    list.Add(Guid.Parse(obj.ToString()));
                                }
                                else
                                {
                                    list.Add(Convert.ChangeType(obj, parameterType));
                                }
                            }
                            else
                            {
                                list.Add(obj);
                            }
                        }
                    }
                    else
                    {
                        list.Add(WebAjaxHandler.IsNullAssignable(parameterInfo.ParameterType) ? null : parameterInfo.DefaultValue);
                    }
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 判断空
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>bool</returns>
        private static bool IsNullAssignable(Type type)
        {
            return !type.IsValueType || WebAjaxHandler.IsNullable(type);
        }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        /// <param name="pi">ParameterInfo</param>
        /// <returns>Type</returns>
        private static Type GetParameterType(ParameterInfo pi)
        {
            return WebAjaxHandler.IsNullable(pi.ParameterType) ? WebAjaxHandler.GetNullableInstanceType(pi.ParameterType) : pi.ParameterType;
        }

        /// <summary>
        /// 判断空
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>bool</returns>
        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// 获取参数类型
        /// </summary>
        /// <param name="type">type</param>
        /// <returns>Type</returns>
        private static Type GetNullableInstanceType(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        /// <summary>
        /// 初始化缓存池
        /// </summary>
        /// <param name="context">当前上下文对象</param>
        /// <param name="responseAnnoto">特性参数</param>
        private static void InitializeCachePolicy(HttpContext context, ResponseAnnotationAttribute responseAnnoto)
        {
            if (responseAnnoto.CacheDuration > 0)
            {
                context.Response.Cache.SetCacheability(HttpCacheability.Server);
                context.Response.Cache.SetExpires(DateTime.Now.AddSeconds((double)responseAnnoto.CacheDuration));
                context.Response.Cache.SetSlidingExpiration(false);
                context.Response.Cache.SetValidUntilExpires(true);
                if (responseAnnoto.ParameterCount > 0)
                {
                    context.Response.Cache.VaryByParams["*"] = true;
                }
                else
                {
                    context.Response.Cache.VaryByParams.IgnoreParams = true;
                }
            }
            else
            {
                context.Response.Cache.SetNoServerCaching();
                context.Response.Cache.SetMaxAge(TimeSpan.Zero);
            }
        }

        /// <summary>
        /// 输出错误消息
        /// </summary>
        /// <param name="context">当前上下文</param>
        /// <param name="message">消息</param>
        /// <param name="encoder">响应</param>
        private void ResponseException(HttpContext context, string message, WebResponseEncoder encoder)
        {
            HttpResponse response = context.Response;
            response.ClearContent();
            response.StatusCode = 300;
            encoder.Write(message);
        }

        /// <summary>
        /// GetJavascript
        /// </summary>
        /// <returns>StringBuilder</returns>
        [ResponseAnnotation(CacheDuration = 0, ResponseFormat = ResponseFormat.JAVASCRIPT)]
        public StringBuilder GetJavascript()
        {
            Type type = base.GetType();
            Uri url = HttpContext.Current.Request.Url;
            string text = WebAjaxHandler.GetScriptTemplete();
            text = text.Replace("%H_DES%", "通过jQuery.ajax完成服务端函数调用");
            text = text.Replace("%H_DATE%", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            text = text.Replace("%URL%", (url.Query.Length > 0) ? url.ToString().Replace(url.Query, "") : url.ToString());
            text = text.Replace("%CLS%", type.Name);
            StringBuilder stringBuilder = new StringBuilder(text);
            MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            MethodInfo[] array = methods;
            for (int i = 0; i < array.Length; i++)
            {
                MethodInfo methodInfo = array[i];
                ResponseAnnotationAttribute methodAnnotation = this.GetMethodAnnotation(methodInfo.Name);
                stringBuilder.AppendLine("/*----------------------------");
                stringBuilder.AppendLine(string.Format("功能说明：{0}", methodAnnotation.Description));
                stringBuilder.AppendLine(string.Format("附加说明：缓存时间 {0} 秒", methodAnnotation.CacheDuration.ToString()));
                stringBuilder.AppendLine(string.Format("          输出类型 {0}", methodAnnotation.ResponseFormat.ToString()));
                stringBuilder.AppendLine("-----------------------------*/");
                string functionTemplete = WebAjaxHandler.GetFunctionTemplete(methodInfo, methodAnnotation.ResponseFormat);
                stringBuilder.AppendLine(functionTemplete);
            }
            return stringBuilder;
        }

        /// <summary>
        /// 获取模板
        /// </summary>
        /// <returns>模板</returns>
        private static string GetScriptTemplete()
        {
            Type typeFromHandle = typeof(WebAjaxHandler);
            AssemblyName assemblyName = new AssemblyName(typeFromHandle.Assembly.FullName);
            Stream manifestResourceStream = typeFromHandle.Assembly.GetManifestResourceStream(assemblyName.Name + ".net.js");
            if (manifestResourceStream == null)
            {
                throw new ApplicationException("模板未找到");
            }
            return HttpRequestUtility.GetMessage(manifestResourceStream, "utf-8");
        }

        /// <summary>
        /// 获取函数模板
        /// </summary>
        /// <param name="method">method</param>
        /// <param name="format">format</param>
        /// <returns>结果</returns>
        private static string GetFunctionTemplete(MethodInfo method, ResponseFormat format)
        {
            StringBuilder stringBuilder = new StringBuilder(method.DeclaringType.Name);
            stringBuilder.AppendFormat(".prototype.{0} = function(", method.Name);
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameters[i];
                stringBuilder.AppendFormat("{0},", parameterInfo.Name);
            }
            stringBuilder.Append("successCallback, errorCallback)");
            stringBuilder.AppendLine("{");
            stringBuilder.Append("\tvar args = {");
            parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameters[i];
                stringBuilder.AppendFormat("{0}:{1},", parameterInfo.Name, parameterInfo.Name);
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.AppendLine("};");
            if (format != ResponseFormat.JSON)
            {
                if (format == ResponseFormat.JAVASCRIPT)
                {
                    stringBuilder.AppendLine("\tvar options={dataType:'script'};");
                }
            }
            else
            {
                stringBuilder.AppendLine("\tvar options={dataType:'json'};");
            }
            stringBuilder.AppendLine("\t$.extend(true,options,{},this.Options);");
            stringBuilder.AppendFormat("\t$.net.CallWebMethod(options,'{0}',args, successCallback, errorCallback);", method.Name);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("}\t\t");
            return stringBuilder.ToString();
        }
    }
}