using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirePluginDemo
{
    /// <summary>
    /// 用于注入的代码数据结构，包括机器码(byte)，注入长度与起始地址
    /// </summary>
    class InjectData
    {
        public byte[] Value { get; private set; }
        public int Address { get; private set; }
        public int Length { get; private set; }

        InjectData(byte[] value, int length, int address)
        {
            Length = length;
            Value = value;
            Address = address;
        }

        InjectData(byte[] value, int address)
        {
            Length = value.Length;
            Value = value;
            Address = address;
        }

        /// <summary>
        /// 从脚本中读取注入点的核心函数
        /// </summary>
        /// <param name="input">格式：每个注入点为2行，首行为 注入代码地址: 第二行为机器码，以16进制编写，只有一位的byte必须补足为2位。可以有空格等分隔符。</param>
        /// <returns></returns>
        static public InjectData[] ReadScript(string str)
        {
            string[] lines = str.Trim().RemoveComments().Split('\n');
            var ret = new List<InjectData>();

            for (int i = 0; i < lines.Length; i += 2)
            {
                int address = Int32.Parse(lines[i].Substring(0, lines[i].Length - 1), System.Globalization.NumberStyles.HexNumber);
                byte[] value = Enumerable.Range(0, lines[i + 1].Length)
                 .Where(x => x % 2 == 0)
                 .Select(x => Convert.ToByte(lines[i + 1].Substring(x, 2), 16))
                 .ToArray();
                ret.Add(new InjectData(value, address));
            }

            return ret.ToArray();
        }

        /// <summary>
        /// 判断输入的字符是否是16进制数字
        /// </summary>
        /// <param name="ch">字符</param>
        /// <returns></returns>
        static private bool IsHexDigit(char ch)
        {
            return ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f' || ch >= 'A' && ch <= 'F';
        }
    }
}
