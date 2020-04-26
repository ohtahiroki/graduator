using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checker
{
    public class CsvParser : IDisposable
    {
        public CsvParser(System.IO.TextReader tr)
        {
            BaseTextReader = tr;
        }
        public System.IO.TextReader BaseTextReader { get; }

        public void Dispose()
        {
            ((IDisposable)BaseTextReader).Dispose();
        }

        public string[] ReadRecord()
        {
            List<string> o = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool flag_r = false;
            bool flag_w = true;
            bool flag_q = false;
            while (flag_w)
            {
                int value = BaseTextReader.Read();
                switch (value)
                {
                    case '\r':
                        flag_r = true;
                        sb.Append((char)value);
                        continue;

                    case '\n':
                        if (flag_q)
                        {
                            sb.Append((char)value);
                        }
                        else
                        {
                            if (flag_r)
                            {
                                sb.Remove(sb.Length - 1, 1);
                            }
                            o.Add(sb.ToString());
                            sb.Clear();
                            flag_w = false;
                        }
                        break;

                    case ',':
                        if (flag_q)
                        {
                            sb.Append((char)value);
                        }
                        else
                        {
                            o.Add(sb.ToString());
                            sb.Clear();
                        }
                        break;

                    case '\"':
                        flag_q = !flag_q;
                        sb.Append((char)value);
                        break;

                    case -1:
                        flag_w = false;
                        if (sb.Length > 0)
                        {
                            o.Add(sb.ToString());
                            sb.Clear();
                        }
                        break;

                    default:
                        sb.Append((char)value);
                        break;
                }
                flag_r = false;
            }
            return o.Select(a => a.RemoveDoubleQuotation()).ToArray();
        }

        public string[][] ReadRecordToEnd()
        {
            List<string[]> arr = new List<string[]>();
            string[] record;
            while((record = ReadRecord()).Length != 0)
            {
                arr.Add(record);
            }
            return arr.ToArray();
        }
    }
    internal static class MyExtention
    {
        public static string RemoveDoubleQuotation(this string inp)
        {
            if (inp.IndexOf('\"') == 0 && inp.LastIndexOf('\"') == inp.Length - 1)
            {
                return (inp.Substring(1, inp.Length - 2));
            }
            else
            {
                return (inp);
            }

        }
    }

}
