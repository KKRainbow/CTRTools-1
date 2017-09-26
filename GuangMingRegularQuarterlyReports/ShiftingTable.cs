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
        public decimal[,] shiftingMatrix;
        public decimal[,] summaryMatrix;
        public static string[] summaryLabel = { "品牌转换", "原有消费者购买增加/减少",
            "购买清单中增加/删除品牌", "新增/流失品类消费者" };
    }

    public static class Tools
    {

    }
}
