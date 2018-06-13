using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace AjaxHandler
{

    /// <summary>
    /// 文本解码器
    /// </summary>
    internal class SimpleUrlDecoder : WebRequestDecoder
    {
        /// <summary>
        /// 请求方法
        /// </summary>
        public override string LogicalMethod
        {
            get
            {
                string text = base.LogicalMethod;
                if (text.ToLower().LastIndexOf(".ashx") >= 0)
                {
                    text = "GetJavascript";
                }
                return text;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">当前上下文</param>
        public SimpleUrlDecoder(HttpContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        /// <returns>参数集</returns>
        public override Dictionary<string, object> Deserialize()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            NameValueCollection @params = this.m_context.Request.Params;
            string[] allKeys = @params.AllKeys;
            for (int i = 0; i < allKeys.Length; i++)
            {
                string text = allKeys[i];
                if (text == "ALL_HTTP")
                {
                    break;
                }
                if (!string.IsNullOrEmpty(text))
                {
                    dictionary.Add(text, @params[text]);
                }
            }
            return dictionary;
        }
    }
}