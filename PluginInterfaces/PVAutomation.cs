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
    }

    public interface IMatrixFileReader
    {
        int Dim { get; }
        List<string> GetDimFieldList(int dim);
        int GetVolumnFieldIndex();
        decimal[,] GetMatrix(List<int> limits, int row, int col);
    }
}
