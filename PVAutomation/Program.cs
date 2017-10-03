using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterfaces;

namespace PVAutomation
{
    class PVAutomation : IPVAutomation
    {
        List<Field> fieldListCache;
        Dictionary<int, List<FieldItem>> fieldItemCache = new Dictionary<int, List<FieldItem>>();

        public SavedTable CalcTable(string tableSpecJson)
        {
            throw new NotImplementedException();
        }

        public IMatrixFileReader GetMatrixReader(string name)
        {
            throw new NotImplementedException();
        }

        public List<Field> GetFieldList()
        {
            if (fieldListCache == null)
            {
                var fieldItems = mainWindow.FieldListBox.Items;
                var fieldDescItems = mainWindow.FieldDescListBox.Items;
                int count = fieldItems.Count;
                List<Field> res = new List<Field>();
                for (int i = 0; i < count; i++)
                {
                    res.Add(new Field
                    {
                        name = fieldItems[i].Text,
                        desc = fieldDescItems[i].Text,
                        index = i,
                    });
                }
                fieldListCache = res;
            }
            return fieldListCache;
        }

        public void FillFieldItemList(Field field)
        {
            var fl = GetFieldList();
            if (!fieldItemCache.ContainsKey(field.index))
            {
                PVFields.FieldFile.GetFieldFile(fl[field.index].name, "");
            }
        }

        private PVWindows.MainWindow mainWindow;
        PVAutomation()
        {
            mainWindow = new PVWindows.MainWindow();
        }
    }
}
