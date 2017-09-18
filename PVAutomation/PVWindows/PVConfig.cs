using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVAutomation.PVWindows
{
    class PVConfig
    {
        private IEnumerable<string> lines;
        public class Keys
        {
            public const string DatabaseFolder = "SystemPath";
        }
        public PVConfig(string pvExecutable)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(pvExecutable), "powrview.ini");

            if (!File.Exists(configFile))
            {
                throw new ArgumentException("PV Config not exists");
            }

            lines = File.ReadLines(configFile);
        }

        public string Get(string key)
        {
            var res = from line in lines
                      let tmp = line.Split("=".ToCharArray())
                      where tmp.Length == 2 && tmp[0] == key
                      select tmp[1];
            return res.Count() == 0 ? null : res.ElementAt(0);
        }
    }
}
