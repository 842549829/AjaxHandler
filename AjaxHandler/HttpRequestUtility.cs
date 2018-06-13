using System.IO;
using System.Text;

namespace AjaxHandler
{
    /// <summary>
    /// Http工具类型
    /// </summary>
    public class HttpRequestUtility
    {
        /// <summary>
        /// 获取当前请求流
        /// </summary>
        /// <param name="inputStream">请求流</param>
        /// <param name="charset">编码</param>
        /// <returns>结果</returns>
        public static string GetMessage(Stream inputStream, string charset)
        {
            Encoding encoding = Encoding.GetEncoding(charset);
            StreamReader streamReader = new StreamReader(inputStream, encoding);
            StringBuilder stringBuilder = new StringBuilder();
            string arg = string.Empty;
            while ((arg = streamReader.ReadLine()) != null)
            {
                stringBuilder.AppendFormat("{0}\n", arg);
            }
            streamReader.Close();
            return stringBuilder.ToString();
        }
    }
}