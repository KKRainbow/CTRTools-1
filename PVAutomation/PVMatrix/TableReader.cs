using PVAutomation.PVMatrix.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVAutomation.PVMatrix
{
    class TableReader
    {
        TableDesc desc;
        BinaryReader matrixReader;
        private Spec TableSpec
        {
            get
            {
                return desc.TableSpec;
            }
        }

        public TableReader(TableDesc desc, string matrixFile)
        {
            this.desc = desc;
            matrixReader = new BinaryReader(File.Open(matrixFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }
        public TableReader(string descFile, string matrixFile): 
            this(TableDesc.GetTableFromSpecFile(descFile), matrixFile)
        { }
        public float[] ReadVolume(int n = -1)
        {
            if (n >= 0)
            {
                matrixReader.BaseStream.Position = n * desc.VolumeSize();
            }
            float[] res = new float[TableSpec.volume.items.Count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = matrixReader.ReadSingle();
            }
            return res;
        }
        private float ReadMatrixElem(int index)
        {
            return ReadVolume(index / TableSpec.volume.items.Count)[index % TableSpec.volume.items.Count];
        }

        public float[,] GetTable(string rowDimName, string colDimName, Dictionary<string, string> limitDimAndItem)
        {
            Dim rowDim, colDim;
            rowDim = TableSpec.nameDimMap[rowDimName];
            colDim = TableSpec.nameDimMap[colDimName];
            var limitDimItems = from dim in TableSpec.dims
                                where limitDimAndItem.ContainsKey(dim.name)
                                select TableSpec.nameDimMap[dim.name].nameDimItemMap[limitDimAndItem[dim.name]];
            if (limitDimItems.Count() != TableSpec.numDims - 2)
            {
                throw new ArgumentException("Count of LimitDim is not right");
            }

            var limitInterval = 0;
            foreach (var item in limitDimItems)
            {
                limitInterval += item.index * item.belongTo.interval;
            }

            float[,] table = new float[rowDim.items.Count, colDim.items.Count];

            for (int r = 0; r < table.GetLength(0); r++)
            {
                for (int c = 0; c < table.GetLength(1); c++)
                {
                    var index = limitInterval +
                        r * rowDim.interval + c * colDim.interval;
                    table[r, c] = ReadMatrixElem(index);
                }
            }
            return table;
        }
    }
}
