using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDll
{
    /// <summary>
    /// 可以访问任意Key而不会出现Key不存在异常，如果Key不存在会将Key所对应Value设为DefaultValue并返回。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class InitializedDictionary<TKey, TValue>: 
        DictionaryEx<TKey, TValue>
    {
        #region Data
        object defaultValue;
        object[] reflectionParams;
        #endregion

        #region Protected Property
        protected TValue DefaultValue
        {
            get
            {
                if(defaultValue is Type)
                {
                    return (TValue)Activator.CreateInstance((Type)defaultValue, reflectionParams);
                }
                else //(defaultValue is TValue)
                {
                    return (TValue)defaultValue;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 手动设置defaultValue
        /// </summary>
        /// <param name="defaultValue">可以是：1.TValue类型，2.Type类型，值必须为typeof(TValue)</param>
        /// <param name="reflectionParams">当defaultValue是Type类型时，提供给动态创建对象操作的参数。
        /// 例：类A有一个构造函数为A(string s)，那么要构造一个默认值为"abc"，整数键的InitializedDictionary，可参照以下写法：
        /// var dict = new InitializedDictionary<int, A>(A, "abc");</int></param>
        public InitializedDictionary(TValue defaultValue)
        {
            this.defaultValue = defaultValue;
        }

        /// <summary>
        /// 前两个参数除了用于函数签名外无其他意义，该函数以typeof(TValue)为默认值，取不存在键处的数据时会用Type动态创建一个对象。
        /// </summary>
        /// <param name="defaultValueType"></param>
        /// <param name="reflectionParams"></param>
        public InitializedDictionary(int k, int j, params object[] reflectionParams)
        {
            defaultValue = typeof(TValue);
            this.reflectionParams = reflectionParams;
        }

        /// <summary>
        /// 自动把defaultValue设置成default(TValue)
        /// </summary>
        public InitializedDictionary(): this(default(TValue))
        {

        }
        #endregion

        #region Public Method
        public new TValue this[TKey key]
        {
            get
            {
                if(base.ContainsKey(key))
                {
                    return base[key];
                }
                else
                {
                    // 总结：
                    // 之前是这样写的
                    // base[key] = DefaultValue;
                    // return DefaultValue;
                    // 这里犯得错误就是把DefaultValue当成一个固定的值的，实际上它是一个Property
                    // 会根据情况运行，如果它返回的是一个新创建对象，那么调用this[key]就会出错
                    // 因为返回的Value并没有被存入this，所以对返回Value的操作也是无效的，
                    // 或者说对这个假的Value操作并没有影响到预期修改的目标this[key]，这与设计的预期目标有出入。
                    var defaultValue = DefaultValue;
                    base[key] = defaultValue;
                    return defaultValue;
                }
            }
            set
            {
                base[key] = value;
            }
        }

        /// <summary>
        /// 注意：该函数可能会拷贝Value，i.e.调用Value.Copy()或Value.Clone()来生成copyTo的Value。
        /// </summary>
        /// <param name="copyTo"></param>
        /// <returns></returns>
        public InitializedDictionary<TKey, TValue> Copy(InitializedDictionary<TKey, TValue> copyTo = null)
        {
            if (copyTo == null) copyTo = new InitializedDictionary<TKey, TValue>();
            copyTo.defaultValue = defaultValue;
            copyTo.reflectionParams = reflectionParams;
            base.Copy(copyTo);

            return copyTo;
        }
        #endregion
    }
}
