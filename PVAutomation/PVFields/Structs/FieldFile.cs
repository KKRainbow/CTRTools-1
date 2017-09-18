using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVAutomation.PVFields.Structs
{
    struct FileHeader
    {
        // 0x00 - 0x0C
        public string filename;
        // 0x0C - 0x1A
        public string reference;
        // 0x1A - 0x3C
        public string description;
        // 0x3C - 0x40: 01000200 or 00000000
        public Int32 type1;
        // 0x40 - 0x42 0xa0cd if a == 1; then userdefined; if ab == 02; then special
        public Int16 type2;
        public bool IsUserDefined()
        {
            return (this.type2 & 0x1000) != 0;
        }
        public bool IsSpecial()
        {
            return (this.type2 & 0xff00) == 0x02;
        }
        // 0x42 - 0x62
        public byte[] unkonwn1;
        // 0x62 - 0x64
        public UInt16 Count1;
        // 0x64 - 0x66
        public UInt16 entryCount;
        // 0x66 - 0x98
        public byte[] unknown2;
        public const int Size = 0x98;
    }
    class FieldHeader
    {
        public string FieldName
        {
            get
            {
                return this.name.TrimStart();
            }
        }
        public int FieldLevel
        {
            get
            {
                return name.Split(' ').Length - 1;
            }
        }
        // 0x00 - 0x20
        public string name;
        // 0x20 - 0x88
        public byte[] padding;
        public const int Size = 0x58;

        public FieldHeader parent;
        public List<FieldHeader> children;
        public int index;
        public IReadOnlyCollection<FieldHeader> Children
        {
            get
            {
                return new ReadOnlyCollection<FieldHeader>(children);
            }
        }
    }
}
