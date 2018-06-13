using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace AjaxHandler
{
    /// <summary>
    /// JSON编码器
    /// </summary>
    public class JsonEncoder : WebResponseEncoder
    {
        /// <summary>
        /// ContentType
        /// </summary>
        public override string MimeType
        {
            get
            {
                return "application/json";
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">当前上下文</param>
        public JsonEncoder(HttpContext context)
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
            string text = "{}";
            if (srcObj != null)
            {
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                text = javaScriptSerializer.Serialize(srcObj);
                text = Regex.Replace(text, "\\\\/Date\\((\\d+)\\)\\\\/", delegate(Match match)
                {
                    DateTime dateTime = new DateTime(1970, 1, 1);
                    dateTime = dateTime.AddMilliseconds((double)long.Parse(match.Groups[0].Value));
                    dateTime = dateTime.ToLocalTime();
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                });
            }
            return text;
        }
    }
}