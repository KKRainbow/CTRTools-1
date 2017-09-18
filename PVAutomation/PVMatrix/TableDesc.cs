using PVAutomation.PVMatrix.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PVAutomation.PVMatrix
{
    public class TableFileMalformatException : ArgumentException
    {
        public TableFileMalformatException(string msg) { }
    }
    class TableDesc
    {
        private static Encoding encoding = Encoding.GetEncoding("GB18030");
        private Spec spec;
        public Spec TableSpec
        {
            get
            {
                return spec;
            }
        }
        private static string ReadNameFromLine(string line)
        {
            var name = line.Split(';')[0].Trim();
            return name;
        }
        private static Attributes ReadAttributesFromLine(string line)
        {
            return new Attributes(line.Split(';').Skip(1).ToDictionary(x => x.Split('=')[0].TrimEnd(),
                x => string.Join("=", x.Split('=').Skip(1).ToArray<string>()).TrimEnd()));
        }
        private static Dim ReadDim(string[] lines, ref int lineno)
        {
            var dim = new Dim();
            var firstLine = lines[lineno];
            if (!firstLine.StartsWith("["))
            {
                //Not Title
                throw new ArgumentException("title don't start with [");
            }
            dim.name = ReadNameFromLine(firstLine);
            //删掉[ ]
            dim.name = dim.name.Substring(1, dim.name.Length - 2);
            dim.attrs = ReadAttributesFromLine(firstLine);
            dim.items = new List<DimItem>();
            dim.interval = 1;
            dim.nameDimItemMap = new Dictionary<string, DimItem>();

            lineno++;
            while (true)
            {
                if (lineno >= lines.Length)
                {
                    break;
                }
                string curLine = lines[lineno];
                if (curLine.StartsWith("["))
                {
                    break;
                }
                if (!curLine.StartsWith("/*"))
                {
                    var dimItem = new DimItem();
                    dimItem.name = ReadNameFromLine(curLine);
                    dimItem.attrs = ReadAttributesFromLine(curLine);
                    dimItem.belongTo = dim;
                    dimItem.index = dim.items.Count;
                    // VERT is calculated based on FREQ
                    if (dimItem.attrs.GetAttr(DimItemAttrNames.Display) != "VERT")
                    {
                        //TODO: how to deal with duplicate key
                        try
                        {
                            dim.nameDimItemMap.Add(dimItem.name, dimItem);
                        }
                        catch (ArgumentException) { }
                    }
                    dim.items.Add(dimItem);
                }
                lineno++;
            }
            return dim;
        }

        public static TableDesc GetTableFromSpecFile(string filename)
        {
            var lines = File.ReadLines(filename, encoding);
            int currLine = 0;
            Spec spec = new Spec();
            spec.nameDimMap = new Dictionary<string, Dim>();
            spec.descriptions = lines.TakeWhile(x => x.StartsWith("/*")).ToArray();

            var lineArr = lines.Where(x => x.Trim().Length != 0 && !x.StartsWith("/*")).ToArray();
            // Read Spec attrs
            while (!lineArr[currLine].StartsWith("["))
            {
                var l = lineArr[currLine];
                var splited = l.Split('=');
                spec.attrs = new Attributes();
                spec.attrs.Add(splited[0], string.Join("=", splited.Skip(1)));
                currLine++;
            }

            int numDim;
            bool res = int.TryParse(spec.attrs.GetAttr(SpecAttrNames.NumDims), out numDim);
            spec.dims = new List<Dim>();
            while (currLine < lineArr.Length)
            {
                var dim = ReadDim(lineArr, ref currLine);
                spec.nameDimMap.Add(dim.name, dim);
                if (dim.name == "VOLUME" && dim.attrs.GetAttr(DimAttrNames.Type) == "VOL")
                {
                    if (spec.volume != null && spec.volume.name != null && spec.volume.name.Length != 0)
                    {
                        throw new TableFileMalformatException("Duplicate volume dim");
                    }
                    spec.volume = dim;
                }
                else if (dim.attrs.GetAttr(DimAttrNames.Type) == "MEASURE")
                {
                    if (spec.measure != null && spec.measure.name != null && spec.measure.name.Length != 0)
                    {
                        throw new TableFileMalformatException("Duplicate mesure dim");
                    }
                    spec.measure = dim;
                }
                else
                {
                    spec.dims.Add(dim);
                }
            }

            if (spec.volume.name == null)
            {
                throw new TableFileMalformatException("No volume dim detected");
            }
            if (spec.measure.name == null)
            {
                throw new TableFileMalformatException("No volume dim detected");
            }

            //volume should be counted into dim
            spec.dims.Add(spec.volume);
            //calc intervals
            for (int i = spec.dims.Count - 2; i >= 0; i--)
            {
                spec.dims[i].interval = spec.dims[i + 1].interval * spec.dims[i + 1].items.Count;
            }

            if (res && numDim != spec.dims.Count)
            {
                throw new TableFileMalformatException("Dim not match numdims attr");
            }

            spec.numDims = spec.dims.Count;

            // ReadDims
            var pvtable = new TableDesc();
            pvtable.spec = spec;
            return pvtable;
        }


        public int VolumeSize()
        {
            int size = 0;
            foreach (var v in spec.volume.items)
            {
                switch (v.attrs[DimItemAttrNames.Storage])
                {
                    case "Float":
                        size += 4;
                        break;
                    default:
                        throw new ArgumentException("Unkonwn Vol Type");
                }
            }
            return size;
        }
        private string DumpDim(Dim dim)
        {
            StringBuilder strb = new StringBuilder();
            strb.AppendFormat("Dim name: {0}\n", dim.name);
            foreach (var item in dim.items)
            {
                strb.AppendFormat("\titem name: {0}\n", item.name);
            }
            strb.AppendFormat("total count: {0}\n", dim.items.Count);
            return strb.ToString();
        }

        public string DumpDim(int n)
        {
            return DumpDim(spec.dims[n]);
        }
        public string DumpDim(string name = ".*")
        {
            StringBuilder strb = new StringBuilder();
            var tmp = spec.dims.ToList();
            tmp.Add(spec.volume);
            var dims = from dim in tmp
                       where Regex.IsMatch(dim.name, name)
                       select dim;
            foreach (var d in dims)
            {
                strb.AppendLine(DumpDim(d));
            }
            return strb.ToString();
        }
        public string DumpVolume()
        {
            return DumpDim(spec.volume);
        }
        public string DumpDimNames()
        {
            StringBuilder strb = new StringBuilder();
            var tmp = spec.dims.ToList();
            tmp.Add(spec.measure);
            tmp.Add(spec.volume);
            foreach (var dim in tmp)
            {
                strb.Append(dim.name);
            }
            return strb.ToString();
        }
    }
}
