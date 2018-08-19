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
    public class InjectData
    {
        public byte[] Value { get; private set; }
        public int Address { get; private set; }
        public int Length { get; private set; }

        public InjectData(byte[] value, int length, int address)
        {
            Length = length;
            Value = value;
            Address = address;
        }

        public InjectData(byte[] value, int address)
        {
            Length = value.Length;
            Value = value;
            // 补0
            if (Length % 4 != 0)
                for (int i = 0; i < 4 - Length % 4; i++)
                    Value = Value.Append((byte)0).ToArray();
            Address = address;
        }
    }
}
