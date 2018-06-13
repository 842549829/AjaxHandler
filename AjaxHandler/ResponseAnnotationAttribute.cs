using System;

namespace AjaxHandler
{
    /// <summary>
    /// 响应特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ResponseAnnotationAttribute : Attribute
    {
        /// <summary>
        /// 默认特性
        /// </summary>
        internal static ResponseAnnotationAttribute Default = new ResponseAnnotationAttribute();

        /// <summary>
        /// 缓存时间
        /// </summary>
        public int CacheDuration
        {
            get;
            set;
        }

        /// <summary>
        /// 响应格式
        /// </summary>
        public ResponseFormat ResponseFormat
        {
            get;
            set;
        }

        /// <summary>
        /// 请求参数长度
        /// </summary>
        public int ParameterCount
        {
            get;
            set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ResponseAnnotationAttribute()
        {
            this.CacheDuration = 0;
            this.ParameterCount = 0;
            this.Description = "略";
        }
    }
}