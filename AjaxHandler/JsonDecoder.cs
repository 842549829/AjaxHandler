using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;

namespace AjaxHandler
{
    /// <summary>
    /// JSON解码器
    /// </summary>
    internal class JsonDecoder : WebRequestDecoder
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">当前上下文</param>
        internal JsonDecoder(HttpContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        /// <returns>参数</returns>
        public override Dictionary<string, object> Deserialize()
        {
            string bodyName = this.m_context.Request.ContentEncoding.BodyName;
            string message = HttpRequestUtility.GetMessage(this.m_context.Request.InputStream, bodyName);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            object obj = javaScriptSerializer.DeserializeObject(message);
            Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
            string[] allKeys = this.m_nvc.AllKeys;
            for (int i = 0; i < allKeys.Length; i++)
            {
                string text = allKeys[i];
                dictionary.Add(text, this.m_nvc[text]);
            }
            return dictionary;
        }
    }
}