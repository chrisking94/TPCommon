using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonDll
{
    public class TargetNotFoundException : Exception
    {
        public TargetNotFoundException(object target) : base($"[{target.ToString()}] not found!")
        {

        }
    }

    public class TypeErrorException : Exception
    {
        public TypeErrorException(string msg = ""): base(msg) { }
        public TypeErrorException(Type t1, Type t2, string msg = "") : base($"不能将{t1.ToString()}转换为{t2.ToString()}.${msg}") { }
    }

    public class ValueErrorException : Exception
    {
        public ValueErrorException(string msg): base(msg) { }
    }
}
