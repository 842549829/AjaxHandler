using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AjaxHandler
{
    /// <summary>
    /// 请求解码器
    /// </summary>
    public abstract class WebRequestDecoder
    {
        /// <summary>
        /// 当前上下文
        /// </summary>
        protected HttpContext m_context;

        /// <summary>
        /// 请求参数集
        /// </summary>
        protected NameValueCollection m_nvc;

        /// <summary>
        /// 请求名字
        /// </summary>
        public virtual string LogicalMethod
        {
            get
            {
                int num = this.m_context.Request.Url.Segments.Length;
                return this.m_context.Request.Url.Segments[num - 1];
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">当前上下文</param>
        public WebRequestDecoder(HttpContext context)
        {
            this.m_context = context;
            this.m_nvc = ((this.m_context.Request.HttpMethod.ToUpper() == "GET") ? this.m_context.Request.QueryString : this.m_context.Request.Form);
        }

        /// <summary>
        /// 创建解码器
        /// </summary>
        /// <param name="context">当前上下文</param>
        /// <returns>解码器</returns>
        public static WebRequestDecoder CreateInstance(HttpContext context)
        {
            WebRequestDecoder result = null;
            string text = context.Request.ContentType.ToLower();
            if (text.IndexOf("application/json") >= 0)
            {
                result = new JsonDecoder(context);
            }
            else if (text.IndexOf("application/x-www-form-urlencoded") >= 0)
            {
                result = new SimpleUrlDecoder(context);
            }
            return result;
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        /// <returns>参数集</returns>
        public abstract Dictionary<string, object> Deserialize();
    }
}
