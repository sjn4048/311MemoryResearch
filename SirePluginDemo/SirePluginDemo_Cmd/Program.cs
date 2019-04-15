using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.IO;
using SirePluginDemo;

namespace SirePluginDemo_Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            const string initial = "Initial.txt";
            if (!File.Exists(initial))
                throw new FileNotFoundException(@"Expected file ""Initial.txt"" as input.");

            string script = File.ReadAllText(initial);

            try
            {
                var memManager = new MemoryManager();
                memManager.WriteMemoryValue(script.ToInjectData());
            }
            catch (Exception ex)
            {
                throw new InvalidScriptException("Initial.txt");
            }
        }
    }
}
