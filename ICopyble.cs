using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDll
{
    public interface ICopyble<T>
    {
        T Copy(T copyTo = default(T));
    }
}
