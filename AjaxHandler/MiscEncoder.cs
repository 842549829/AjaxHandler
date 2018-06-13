using System.Web;

namespace AjaxHandler
{
    /// <summary>
    /// Text编码器
    /// </summary>
    internal class MiscEncoder : WebResponseEncoder
    {
        /// <summary>
        /// ContentType
        /// </summary>
        public override string MimeType
        {
            get
            {
                return "text/xml";
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">当前上下文</param>
        public MiscEncoder(HttpContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 实现对象序列化
        /// </summary>
        /// <param name="srcObj">对象</param>
        /// <returns>序列化字符串</returns>
        public override string Serialize(object srcObj)
        {
            return (srcObj == null) ? string.Empty : srcObj.ToString();
        }
    }
}