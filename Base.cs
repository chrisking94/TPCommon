using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;
using System.Runtime.CompilerServices;

/// <summary>
/// author: chris
/// date: 18.08.27
/// </summary>
namespace CommonDll
{
    /// <summary>
    /// contains some utilities, including debug print.
    /// without any data member.
    /// </summary>
    public class Base
    {
        #region METHOD
        [Conditional("DEBUG_CHRIS")]
        protected void Print(string s, string title, ConsoleColor titleColor = ConsoleColor.Blue, string end = "\n")
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            //PrintTraceBackPath(3);
            Console.Write(this.ToString());
            Console.Write('[');
            if (title == "")
            {
                title = "msg";
            }
            Console.ForegroundColor = titleColor;
            Console.Write(title);
            Console.ForegroundColor = oldColor;
            Console.Write("]: ");
            Console.Write(s);
            Console.Write(end);
        }

        [Conditional("DEBUG_CHRIS")]
        protected void Msg(string msg, string title = "")
        {
            Print(msg, title);
        }

        [Conditional("DEBUG_CHRIS")]
        protected void Warning(string msg)
        {
            Print(msg, "warning", ConsoleColor.Yellow);
        }
        #endregion

        #region STATIC_METHOD
        public static string FormatTime(double ms)
        {
            if (ms < 1000)
            {
                return $"{(int)ms}ms";
            }
            int s = (int)ms / 1000;
            if (s < 60)
            {
                return $"{s}s";
            }
            else
            {
                int m = s / 60;
                s = s % 60;
                int h = m / 60;
                m = m % 60;
                return $"{h}:{m}:{s}";
            }
        }
        /// <summary>
        /// a C style sprintf, it will return a string built by format.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string sprintf(string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder(1024);
            for (int i = 0, j = -1; i < format.Length; ++i)
            {
                Char chr = format[i];
                if (chr == '%')
                {
                    chr = format[++i];
                    switch (chr)
                    {
                        case 'c':
                            sb.Append((Char)args[++j]);
                            break;
                        case 's':
                            sb.Append((string)args[++j]);
                            break;
                        case 'd':
                            sb.Append(args[++j].ToString());
                            break;
                        case 'f':
                            sb.Append((double)args[++j]);
                            break;
                        case '*':
                            sb.Append(args[++j].ToString());
                            break;
                        case '[':  // 输出IEnumerable，%[,]，其中,为元素之间的分隔符，默认为,。
                            var sepBuilder = new StringBuilder();
                            chr = format[++i];
                            while (chr != ']')
                            {
                                sepBuilder.Append(chr);
                                chr = format[++i];
                            }
                            var separator = sepBuilder.ToString();
                            if(separator == "") { separator = ","; }
                            var arg = args[++j];
                            int nCount = 0;
                            // 生成字符串
                            sb.Append('[');
                            if(arg is IEnumerable)
                            {
                                foreach(var item in (IEnumerable)arg)
                                {
                                    sb.Append(item);
                                    sb.Append(separator);
                                    ++nCount;
                                }
                                if(nCount > 0)
                                {
                                    sb.Remove(sb.Length - separator.Length, separator.Length);  // 移除最后一个Separator
                                }
                                sb.Append(']');
                                sb.Append(".Count=");
                                sb.Append(nCount);
                            }
                            else
                            {
                                throw new TypeErrorException($"param{j} [{arg}] is not IEnumerable!");
                            }
                            break;
                        case '%':
                            sb.Append(chr);
                            break;
                        default:
                            throw (new FormatException($"invalid char [{chr}] after % in {format}!"));
                    }
                }
                else
                {
                    sb.Append(chr);
                }
            }
            return sb.ToString();
        }

        #region DEBUG_CHRIS
        [Conditional("DEBUG_CHRIS")]
        public static void printf(string format, params object[] args)
        {
            Console.Write(sprintf(format, args));
        }

        /// <summary>
        /// python风格的print函数，默认换行。最后一个参数为'\n'表示不换行
        /// </summary>
        /// <param name="args"></param>
        [Conditional("DEBUG_CHRIS")]
        public static void print(params object[] args)
        {
            var sb = new StringBuilder(1024);
            int i = 1;
            foreach (var arg in args)
            {
                if (i == args.Length && arg is char)  // 最后一个参数有特殊用途
                {
                    if((char)arg == '\n')  // 不换行
                    {
                        break;
                    }
                }
                if (arg is IEnumerable)  // 打印可枚举类型
                {
                    if (arg is string)
                    {
                        sb.Append(arg);
                    }
                    else
                    {
                        sb.Append('[');
                        int len = 0;
                        foreach (var item in arg as IEnumerable)
                        {
                            sb.Append(item);
                            sb.Append(", ");
                            ++len;
                        }
                        if(len != 0)
                        {
                            sb.Remove(sb.Length - 2, 2);
                        }
                        sb.Append(']');
                    }
                }
                else if(arg is Stopwatch)
                {
                    sb.Append(sprintf("[timecost: %dms]", ((Stopwatch)arg).ElapsedMilliseconds));
                }
                else
                {
                    sb.Append(arg);
                }
                if(i == args.Length)
                {
                    sb.Append('\n');
                }
                else
                {
                    sb.Append(", ");
                }
                ++i;
            }
            Console.Write(sb.ToString());
        }

        private static Stopwatch sw = new Stopwatch();
        /// <summary>
        /// start a commonly used StopWatch, do not use this method in multi-thread scene.
        /// </summary>
        [Conditional("DEBUG_CHRIS")]
        public static void starttimer()
        {
            sw.Reset();
            sw.Start();
        }
        [Conditional("DEBUG_CHRIS")]
        public static void msgtimecost(string msg = "")
        {
            print(msg, sw);
        }

        /// <summary>
        /// print trace back path
        /// </summary>
        /// <param name="frame">param for StackTrack.GetFrame(int)</param>
        [Conditional("DEBUG_CHRIS")]
        public static void PrintTraceBackPath(int frame)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            StackTrace stackTrance = new StackTrace();
            System.Reflection.MethodBase grandFunc = (new StackTrace()).GetFrame(frame).GetMethod();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(grandFunc.Module.Name);
            Console.ForegroundColor = oldColor;
            Console.Write('.');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(grandFunc.DeclaringType.FullName);
            Console.ForegroundColor = oldColor;
            Console.Write('.');
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(grandFunc.Name);
            Console.ForegroundColor = oldColor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void swap<T>(ref T a, ref T b)
        {
            var tmp = b;
            b = a;
            a = tmp;
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// A derived class of Base, the difference is that BaseEx contains some data members, such as an object of class StopWatch.
    /// It implementes some methods who need to use class's data members.
    /// </summary>
    public class BaseEx : Base
    {
        #region DATA
        private Stopwatch sw;
        #endregion

        #region METHOD
        [Conditional("DEBUG_CHRIS")]
        protected void StartTimer()
        {
            sw = new Stopwatch();
            sw.Start();
        }

        [Conditional("DEBUG_CHRIS")]
        protected void MsgTimecost()
        {
            Print($"{sw.Elapsed.TotalMilliseconds}ms", "timecost", ConsoleColor.Magenta);
        }
        #endregion
    }
}
