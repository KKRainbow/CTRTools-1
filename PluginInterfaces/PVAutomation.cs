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
        public int level;
        public string name;
        public int index;
        public IReadOnlyCollection<FieldItem> children;
        public FieldItem parent;
    }
    public class Field
    {
        public string name;
        public string desc;
        public int index;

        public IReadOnlyCollection<FieldItem> items;
        public FieldItem root;
    }
    public interface IPVAutomation
    {
        List<Field> GetFieldList();
        Field FillFieldItemList(Field field);

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
