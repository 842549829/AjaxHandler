using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjaxHandler
{
    /// <summary>
    /// 参数特性
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    [Serializable]
    public class NamedProperty<T>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 默认值
        /// </summary>
        public T DefaultValue
        {
            get;
            set;
        }

        /// <summary>
        ///构造函数 
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="defaultValue">默认值</param>
        private NamedProperty(string name, T defaultValue)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>结果</returns>
        public static NamedProperty<T> Create(string name, T defaultValue)
        {
            return new NamedProperty<T>(name, defaultValue);
        }

        /// <summary>
        /// 冲洗比较器
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>结果</returns>
        public override bool Equals(object obj)
        {
            return (obj as NamedProperty<T>).Name.Equals(this.Name);
        }

        /// <summary>
        /// 重写Hash
        /// </summary>
        /// <returns>code</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
    }
}