using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AjaxHandler
{
    /// <summary>
    /// XML编码器
    /// </summary>
    public class XmlEncoder : WebResponseEncoder
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
        public XmlEncoder(HttpContext context)
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
            return string.Empty;
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="srcObj">数据对戏</param>
        public override void Write(object srcObj)
        {
            throw new NotImplementedException();
        }
    }
}