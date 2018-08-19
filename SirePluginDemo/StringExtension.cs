using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SirePluginDemo
{
    /// <summary>
    /// 用于各种string操作
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 将指定的字符串每x个切开返回
        /// </summary>
        /// <param name="s"></param>
        /// <param name="partLength"></param>
        /// <returns></returns>
        public static IEnumerable<string> SplitInParts(this string s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException("s");
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", "partLength");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        /// <summary>
        /// 删除注释
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns></returns>
        public static string RemoveComments(this string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            var blockComments = @"/\*(.*?)\*/";
            var lineComments = @"//(.*?)\r?\n";
            var strings = @"""((\\[^\n]|[^""\n])*)""";
            var verbatimStrings = @"@(""[^""]*"")+";

            return Regex.Replace(s,
                blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                me =>
                {
                    if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                        return me.Value.StartsWith("//") ? Environment.NewLine : "";
                    // Keep the literal strings
                    return me.Value;
                },
                RegexOptions.Singleline);
        }

        public static string RemoveBlanks(this string s)
        {
            return s.Replace(" ", string.Empty).Replace("\t", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
        }

        public static string[] Split(this string s, string splitter)
        {
            return s.Split(new[] { splitter }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] Split(this string s, string[] splitter)
        {
            return s.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 判断输入的字符是否是16进制数字
        /// </summary>
        /// <param name="ch">字符</param>
        /// <returns></returns>
        public static bool IsHexDigit(this char ch)
        {
            return ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f' || ch >= 'A' && ch <= 'F';
        }


        /// <summary>
        /// 从脚本中读取注入点的核心函数
        /// </summary>
        /// <param name="str">格式：每个注入点为2行，首行为 注入代码地址: 第二行为机器码，以16进制编写，只有一位的byte必须补足为2位。可以有空格等分隔符。</param>
        /// <returns></returns>
        static public InjectData[] ToInjectData(this string str)
        {
            string[] lines = str.Trim().RemoveComments().RemoveBlanks().Split(new[] { "<Address>", "<Code>" });
            var ret = new List<InjectData>();

            for (int i = 0; i < lines.Length; i += 2)
            {
                if (lines[i].Any(x => !x.IsHexDigit()))
                    throw new FormatException("检测到了非16进制字符：" + lines[i].First(x => !x.IsHexDigit()));
                if (lines[i + 1].Any(x => !x.IsHexDigit()))
                    throw new FormatException("检测到了非16进制字符：" + lines[i + 1].First(x => !x.IsHexDigit()));
                int address = Int32.Parse(lines[i], System.Globalization.NumberStyles.HexNumber);
                byte[] value = Enumerable.Range(0, lines[i + 1].Length)
                 .Where(x => x % 2 == 0)
                 .Select(x => Convert.ToByte(lines[i + 1].Substring(x, 2), 16))
                 .ToArray();
                ret.Add(new InjectData(value, address));
            }

            return ret.ToArray();
        }
    }
}
