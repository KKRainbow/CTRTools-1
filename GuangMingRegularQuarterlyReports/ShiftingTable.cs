using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorPlugins.GuangMingRegularQuarterlyReports
{
    public struct ShiftingTable
    {
        public string vendor;
        public string region;
        public string description;
        //数据区的行数，列数（包括表头）
        public int row, col;
        public string[,] dataArray;
    }

    public static class Tools
    {

    }
}
