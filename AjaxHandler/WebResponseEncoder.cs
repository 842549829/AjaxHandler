using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AjaxHandler
{
    /// <summary>
    /// web响应编码器
    /// </summary>
    public abstract class WebResponseEncoder
    {
        /// <summary>
        /// 当前上下文
        /// </summary>
        protected HttpContext m_context;

        /// <summary>
        /// 请求类型
        /// </summary>
        public abstract string MimeType { get; }

        /// <summary>
        /// 响应处理
        /// </summary>
        /// <param name="context">当前上下文</param>
        protected WebResponseEncoder(HttpContext context)
        {
            List<string> list = new List<string>();
            list.AddRange(context.Request.AcceptTypes);
            this.m_context = context;
        }

        /// <summary>
        /// 创建响编码器
        /// </summary>
        /// <param name="context">当前上下文</param>
        /// <param name="format">类型格式</param>
        /// <returns>结果</returns>
        public static WebResponseEncoder CreateInstance(HttpContext context, ResponseFormat format)
        {
            WebResponseEncoder result;
            switch (format)
            {
                case ResponseFormat.JSON:
                    result = new JsonEncoder(context);
                    break;
                case ResponseFormat.HTML:
                    result = new HtmlEncoder(context);
                    break;
                case ResponseFormat.XML:
                    result = new XmlEncoder(context);
                    break;
                case ResponseFormat.JAVASCRIPT:
                    result = new JQueryScriptEncoder(context);
                    break;
                default:
                    result = new MiscEncoder(context);
                    break;
            }
            return result;
        }

       /// <summary>
       /// 序列化数据
       /// </summary>
       /// <param name="srcObj">数据对象</param>
       /// <returns>序列化字符串</returns>
        public abstract string Serialize(object srcObj);

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="srcObj">数据对戏</param>
        public virtual void Write(object srcObj)
        {
            HttpResponse response = this.m_context.Response;
            string s = this.Serialize(srcObj);
            response.ContentEncoding = this.m_context.Request.ContentEncoding;
            response.ContentType = this.MimeType;
            response.Write(s);
        }
    }
}