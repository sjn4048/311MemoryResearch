using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirePluginDemo
{
    static public class UnitTest
    {
        public const string beforeComment = "//this is a comment\n"
            + "/* test test test\n"
            + "bla bla bla\n"
            + "ha ha ha */"
            + "5785330:\n"
            + "90 90 90 8a";

        public const string sample = @"// support comment
// type in injection address
575330:
/* then input machine code (assembly not supported)
*/
90 90 90 90
// type in another injection address
575440:
aa aa            ffff
";
    }
}
