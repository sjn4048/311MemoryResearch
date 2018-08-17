using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SirePluginDemo
{
    class Modifier
    {
        static readonly string ProcessName = "san11pk";

        public bool IsProcessOpen()
        {
            return MemoryManager.IsProcessOpen(ProcessName);
        }
    }
}
