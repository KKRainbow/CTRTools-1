using PVAutomation.PVFields.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVAutomation.PVFields
{
    public class NotDataFileException : Exception { }
    class FieldFile
    {
        private string filename;
        private BinaryReader binStream;
        private Encoding encoding = Encoding.GetEncoding("GB18030");

        private FieldHeader root = new FieldHeader()
        {
            children = new List<FieldHeader>(),
            name = "Header Root",
            index = -1
        };
        public FieldHeader Root
        {
            get
            {
                return root;
            }
        }
        private string ReadString(int length)
        {
            var str = encoding.GetString(binStream.ReadBytes(length));
            return str.Split("\0".ToCharArray())[0].TrimEnd();
        }
        public FileHeader ReadHeader()
        {
            if (binStream.BaseStream.Length < FileHeader.Size)
            {
                throw new NotDataFileException();
            }
            FileInfo fi = new FileInfo(filename);
            FileHeader header = new FileHeader();
            binStream.BaseStream.Position = 0;

            header.filename = ReadString(0x0D);

            if (!header.filename.Equals(fi.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new NotDataFileException();
            }

            header.reference = ReadString(0x0D);
            header.description = ReadString(0x3C - 0x0D * 2);
            header.type1 = binStream.ReadInt32();
            header.type2 = binStream.ReadInt16();
            header.unkonwn1 = binStream.ReadBytes(0x62 - (int)binStream.BaseStream.Position);
            header.Count1 = binStream.ReadUInt16();
            header.entryCount = binStream.ReadUInt16();
            header.unknown2 = binStream.ReadBytes(FileHeader.Size - (int)binStream.BaseStream.Position);
            return header;
        }
        public List<FieldHeader> ReadFieldHeaders()
        {
            var header = ReadHeader();
            var list = new List<FieldHeader>();
            for (int i = 0; i < header.entryCount; i++)
            {
                FieldHeader fh = new FieldHeader();
                fh.name = ReadString(0x20);
                fh.padding = binStream.ReadBytes(FieldHeader.Size - 0x20);
                fh.children = new List<FieldHeader>();
                for (int j = i - 1; j >= 0; j--)
                {
                    if (list[j].FieldLevel == fh.FieldLevel)
                    {
                        fh.parent = list[j].parent;
                        break;
                    }
                    else if (list[j].FieldLevel < fh.FieldLevel)
                    {
                        fh.parent = list[j];
                        break;
                    }
                }
                if (fh.parent == null)
                {
                    fh.parent = root;
                }
                fh.parent.children.Add(fh);
                fh.index = i;
                list.Add(fh);
            }
            return list;
        }
        public FieldFile(string filename)
        {
            this.filename = filename;
            binStream = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read));
        }
        public static FieldFile GetFieldFile(string name, string directory)
        {
            var files = Directory.GetFiles(directory, name + "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fn = new FileInfo(file).Name;
                if (!Char.IsNumber(fn[fn.Length - 1]))
                {
                    continue;
                }
                try
                {
                    var d = new FieldFile(file);
                    d.ReadHeader();
                    return d;
                }
                catch (NotDataFileException)
                {
                    continue;
                }
            }
            return null;
        }
    }
}
