using System.Web;

namespace AjaxHandler
{
    /// <summary>
    /// HTML编码器
    /// </summary>
    public class HtmlEncoder : WebResponseEncoder
    {
        /// <summary>
        /// ContentType
        /// </summary>
        public override string MimeType
        {
            get
            {
                return "text/html";
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">当前上下文</param>
        public HtmlEncoder(HttpContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 对象序列化处理
        /// </summary>
        /// <param name="srcObj">对象</param>
        /// <returns>字符串</returns>
        public override string Serialize(object srcObj)
        {
            return (srcObj == null) ? string.Empty : srcObj.ToString();
        }
    }
}