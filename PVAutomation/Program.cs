using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream logFile;
            logFile = new FileStream("d:/pvauto.log", FileMode.Create);
            logFile.SetLength(0);
            //Console.SetOut(new StreamWriter(logFile));
        }
    }
}
