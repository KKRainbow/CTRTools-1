using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVAutomation.PVMatrix.Structs
{
    public class Attributes: Dictionary<string, string>
    {
        public string GetAttr(string key)
        {
            if (ContainsKey(key))
            {
                return this[key];
            }
            return "";
        }
        public Attributes(Dictionary<string, string> dict): base(dict)
        {
        }
        public Attributes(): base()
        {
        }
    }
    class DimItemAttrNames
    {
        public const string Level = "level";
        public const string Alias = "Alias";
        public const string RowSkip = "RowSkip";
        public const string Format = "format"; //8,0,Normal
        public const string Measure = "MEASURE"; //VL1
        public const string Display = "DISPLAY"; //DISPLAY=FREQ/VERT
        public const string Storage = "Storage"; //Float
        public const string Type = "Type"; //Added VOL COLUMN
    }
    class DimAttrNames
    {
        public const string Type = "TYPE";
        public const string NumCols = "NUMCOLS";
        public const string NumLevels = "numlevels";
        public const string Orientation = "Orientation";
    }
    public class SpecAttrNames
    {
        public const string NumDims = "NumDims";
    }
    public class DimItem
    {
        public string name;
        public Attributes attrs;

        public int index;
        public Dim belongTo;


    };
    public class Dim
    {
        public string name;
        public Attributes attrs;

        public int interval;
        public Dictionary<string, DimItem> nameDimItemMap;
        public List<DimItem> items;
    }
    class Spec
    {
        public int numDims;
        public string[] descriptions; //注释
        public Attributes attrs;
        public List<Dim> dims;

        public Dictionary<string, Dim> nameDimMap;

        public Dim volume, measure;
    }
}
