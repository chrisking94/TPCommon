using System.Collections.Generic;
using System.IO;

namespace CommonDll
{
    /// <summary>
    /// 使用 行索引，列索引 定位数据。某个位置的数据可以事先不存在。不存在时会返回defaultValue。
    /// </summary>
    /// <typeparam name="TRowIndex"></typeparam>
    /// <typeparam name="TColIndex"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Table<TRowIndex, TColIndex, TValue>: 
        InitializedDictionary<TRowIndex, InitializedDictionary<TColIndex, TValue>>
    {
        #region Constructor
        public Table(TValue defaultValue = default(TValue)): 
            base(0, 0, defaultValue)
        {

        }
        #endregion

        public virtual TValue this[TRowIndex row, TColIndex col]
        {
            get
            {
                return this[row][col];
            }
            set
            {
                this[row][col] = value;
            }
        }

        public Table<TRowIndex, TColIndex, TValue> Copy(Table<TRowIndex, TColIndex, TValue> copyTo = null)
        {
            if (copyTo == null) copyTo = new Table<TRowIndex, TColIndex, TValue>();
            foreach(var key in Keys)
            {
                copyTo[key] = this[key].Copy();
            }
            return copyTo;
        }
    }
}
