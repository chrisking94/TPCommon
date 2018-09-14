using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDll
{
    /// <summary>
    /// 继承ICopyble的Dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryEx<TKey, TValue>: 
        Dictionary<TKey, TValue>, ICopyble<DictionaryEx<TKey, TValue>>
    {
        #region Public Method
        /// <summary>
        /// 如果TValue有ICopyble接口，那么新的Value会通过调用Value的Copy()方法得到；
        /// 如果TValue继承了ICloneable，新的Value会通过调用Value.Clone()得到。
        /// </summary>
        /// <param name="copyTo"></param>
        /// <returns></returns>
        public DictionaryEx<TKey, TValue> Copy(DictionaryEx<TKey, TValue> copyTo = null)
        {
            if (copyTo == null) copyTo = new DictionaryEx<TKey, TValue>();
            if (typeof(ICopyble<TValue>).IsAssignableFrom(typeof(TValue)))
            {
                foreach (var key in Keys)
                {
                    copyTo[key] = (this[key] as ICopyble<TValue>).Copy(); ;
                }
            }
            else if (typeof(ICloneable).IsAssignableFrom(typeof(TValue)))
            {
                foreach (var key in Keys)
                {
                    copyTo[key] = (TValue)(this[key] as ICloneable).Clone();
                }
            }
            else
            {
                foreach (var key in Keys)
                {
                    copyTo[key] = this[key];
                }
            }

            return copyTo;
        }
        #endregion
    }
}
