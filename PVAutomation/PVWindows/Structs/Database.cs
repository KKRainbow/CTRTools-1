using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVAutomation.PVWindows.Structs
{
    public struct Database
    {
        public string name;
        public string description;
        public string folder;
        public static Database ParseFromFile(string file)
        {
            Encoding encoding = Encoding.GetEncoding("GB18030");
            var binStream = new BinaryReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
            Func<int, string> ReadString = (int length) =>
            {
                var str = encoding.GetString(binStream.ReadBytes(length));
                return str.Split("\0".ToCharArray())[0].TrimEnd();
            };

            Database db;
            db.name = ReadString(9);
            db.description = ReadString(51);
            db.folder = Path.GetDirectoryName(file);
            return db;
        }
    }
}
