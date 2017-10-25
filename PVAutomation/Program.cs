using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginInterfaces;
using System.ComponentModel.Composition;

namespace PVAutomation
{
    [Export(typeof(IPVAutomation))]
    class PVAutomation : IPVAutomation
    {
        List<Field> fieldListCache;
        Dictionary<int, List<FieldItem>> fieldItemCache = new Dictionary<int, List<FieldItem>>();
        Dictionary<int, FieldItem> fieldItemRootCache = new Dictionary<int, FieldItem>();

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
                    var f = new Field
                    {
                        name = fieldItems[i].Text,
                        desc = fieldDescItems[i].Text,
                        index = i,
                    };
                    res.Add(f);
                }
                fieldListCache = res;
            }
            return fieldListCache;
        }

        public Field FillFieldItemList(Field field)
        {
            var fl = GetFieldList();
            if (!fieldItemCache.ContainsKey(field.index))
            {
                var f = PVFields.FieldFile.GetFieldFile(fl[field.index].name, mainWindow.CurrDatabase.folder);
                var list = new List<FieldItem>();

                var rootChildren = new List<FieldItem>();
                var childrenListDict = new Dictionary<int, List<FieldItem>>
                {
                    {-1, rootChildren }
                };
                field.root = new FieldItem
                {
                    children = rootChildren,
                    name = "field item root",
                    index = -1,
                    level = -1,
                };
                fieldItemRootCache.Add(field.index, field.root);

                if (f != null)
                {
                    var headers = f.ReadFieldHeaders();
                    // Headers should be sorted by index and start with 0
                    foreach (var header in headers)
                    {
                        FieldItem item = new FieldItem();
                        item.index = header.index;
                        item.name = header.name;
                        item.level = header.FieldLevel;

                        var children = new List<FieldItem>();
                        item.children = children;
                        childrenListDict[item.index] = children;

                        item.parent = header.parent.index >= 0 ? list[header.parent.index] : field.root;
                        childrenListDict[item.parent.index].Add(item);

                        list.Add(item);
                    }
                }
                fieldItemCache[field.index] = list;
            }
            field.items = fieldItemCache[field.index];
            field.root = fieldItemRootCache[field.index];
            field.name = fl[field.index].name;
            return field;
        }

        private PVWindows.MainWindow mainWindow;
        PVAutomation()
        {
            mainWindow = new PVWindows.MainWindow();
        }
    }
}
