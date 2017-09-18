using PVAutomation.PVFields;
using PVAutomation.PVWindows.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVAutomation.Tests
{
    public partial class Tests
    {
        public static string DatabaseDir = "D:/pv/N182R3.PW";

        public void Test()
        {
            //return;
            var fieldData = FieldFile.GetFieldFile("REP30040", DatabaseDir);
            ActivateWindow();
            SwitchTableSpec spec = new SwitchTableSpec
            {
                demographicField = new FieldDetail()
                {
                    fieldName = "WPOCITY",
                    fieldItems =
                    {
                        "全国总体", " 上海", " 北京",
                    }
                },
                period1Field = new FieldDetail()
                {
                    fieldName = "W_PERIOD",
                    fieldItems = new SortedSet<string>((from header in PVData.GetDataFileOfField("W_PERIOD", databaseDir).ReadFieldHeaders()
                                                        select header.name).Skip(1).ToList())
                },
                dataFilter = new FieldDetail()
                {
                    fieldName = "REP30031",
                    fieldItems =
                    {
                        "  新鲜酸奶", "   果粒酸奶"
                    }
                },
                brands = new FieldDetail()
                {
                    fieldName = "REP30040",
                    fieldItemsIndex = new SortedSet<int> { 1,2, 3}
                },
                primaryVolume = new FieldDetail()
                {
                    fieldName = "R_SPEND",
                },
                secondaryVolume = new FieldDetail()
                {
                    fieldName = "R_SPEND",
                },
                isRolling = true,
                periodLength = 13,
                waveInterval = 13,
            };
            RunSwitchTableSpec(spec);
            ExecuteSwitchSpec();
            SaveSwitchSpec("123", "");
        }
    }
}
