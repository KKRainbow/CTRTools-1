using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterfaces
{
    public enum TableType
    {
        Normal,
        Shifting,
        Basic,
        Summary,
        Control,
        GainLoss,
    }
    public struct SavedTable
    {
        string name;
        Dictionary<TableType, string> matrixFile;
    }

    public class FieldItem
    {
        private FieldItem root = new FieldItem
        {
            children = new List<FieldItem>(),
            name = "field item root",
            index = -1,
            level = -1,
        };
        public FieldItem Root
        {
            get
            {
                return root;
            }
        }
        public int level;
        public string name;
        public int index;
        public IReadOnlyCollection<FieldItem> children;
        public FieldItem parent;
    }
    public struct Field
    {
        public string name;
        public IReadOnlyCollection<FieldItem> items;
    }
    public interface IPVAutomation
    {
        List<string> GetFieldList();
        List<string> GetFieldItemList(int fieldIdx);

        /*
         * {
         *  name: "tblename",
         *  type: "switch/normal/..."
         *  spec: {
         *  }
         * }
         */
        SavedTable CalcTable(string tableSpecJson);
        IMatrixFileReader GetMatrixReader(string name);
    }

    public interface IMatrixFileReader
    {
        int Dim { get; }
        List<string> GetDimFieldList(int dim);
        int GetVolumnFieldIndex();
        decimal[,] GetMatrix(List<int> limits, int row, int col);
    }
}
