using Newtonsoft.Json;
using PluginInterfaces;
using PluginInterfaces.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginTests.NeededInfoGen
{
    using NeededInfo = SortedDictionary<string, SortedDictionary<string, object>>;
    public class NeedInfoFormGen
    {
        NeededInfo neededInfoDict = new NeededInfo();
        TableLayoutPanel propertyTable;
        IPlugin plugin;
        IPVAutomation pv;

        public delegate void FormGenEvent();
        public event FormGenEvent AfterShown;
        public void CallAfterShownEvent()
        {
            AfterShown?.Invoke();
        }

        string jsonFile = "d:/MyJSON.json";

        public NeedInfoFormGen(TableLayoutPanel tablePanel, IPlugin plugin, IPVAutomation pv)
        {
            propertyTable = tablePanel;
            this.plugin = plugin;
            this.pv = pv;

            try
            {
                string json = File.ReadAllText(jsonFile, Encoding.UTF8);
                neededInfoDict = JsonConvert.DeserializeObject<NeededInfo>(json);
            }
            catch (Exception)
            {
            }

            propertyTable.HandleDestroyed += (o, e) =>
            {
                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(neededInfoDict)); 
            };
        }

        public void InitForm()
        {
            var json = plugin.GetNeededInfoDescription();
            var forms = JsonConvert.DeserializeObject<List<object>>(json);
            propertyTable.RowCount = forms.Count + 1;

            propertyTable.ColumnCount = 3;
            while (propertyTable.ColumnStyles.Count < propertyTable.ColumnCount)
            {
                propertyTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
            }
            propertyTable.ColumnStyles[0].SizeType = SizeType.AutoSize;
            propertyTable.ColumnStyles[1].SizeType = SizeType.AutoSize;
            propertyTable.ColumnStyles[2].SizeType = SizeType.Percent;
            propertyTable.ColumnStyles[2].Width = 50;

            for (int i = 0; i < forms.Count; i++)
            {
                var form = JsonConvert.DeserializeObject<FormItemBase>(forms[i].ToString());

                if (!neededInfoDict.ContainsKey(form.name))
                {
                    neededInfoDict.Add(form.name, new SortedDictionary<string, object>());
                }

                if (i >= propertyTable.RowStyles.Count)
                {
                    propertyTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }
                else
                {
                    propertyTable.RowStyles[i].SizeType = SizeType.AutoSize;
                }
                switch (form.Type)
                {
                    case InfoType.FieldItems:
                        var fiFrom = JsonConvert.DeserializeObject<FieldItems>(forms[i].ToString());
                        MakeLabel(form.name, i, 0);
                        var fieldCombo = MakeFieldComboBox(form.name, i, 1);
                        MakeFieldItemListBox(form.name, fieldCombo, i, 2);
                        break;
                    case InfoType.File:
                        var f = JsonConvert.DeserializeObject<FileForm>(forms[i].ToString());
                        MakeLabel(form.name, i, 0);
                        MakeFileSelector(form.name, f.filter, f.isDir, f.isSave, f.isMulti, i, 1);
                        break;
                }
            }
        }

        private Button MakeFileSelector(string name, string filter, bool isDir, bool isSave, bool multi, int row, int col)
        {
            var btn = new Button();
            btn.Text = "请选择";
            propertyTable.Controls.Add(btn, col, row);

            var label = new Label();
            label.Text = "请选择" + (isDir ? "文件" : "目录");
            propertyTable.Controls.Add(label, col + 1, row);

            btn.Click += (obj, e) =>
            {
                if (isDir)
                {
                    FolderBrowserDialog dia = new FolderBrowserDialog();
                    dia.ShowNewFolderButton = !isSave;
                    if (dia.ShowDialog() == DialogResult.OK)
                    {
                        label.Text = dia.SelectedPath;
                        neededInfoDict[name]["path"] = dia.SelectedPath;
                    }
                }
                else
                {
                    var dia = isSave ? (FileDialog)new SaveFileDialog() : new OpenFileDialog();
                    dia.Filter = filter;
                    if (dia.ShowDialog() == DialogResult.OK)
                    {
                        label.Text = dia.FileName;
                        neededInfoDict[name]["path"] = dia.FileName;
                    }
                }
            };
            btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            label.AutoSize = true;
            label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            label.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;

            if (neededInfoDict[name].ContainsKey("path"))
            {
                label.Text = (string)neededInfoDict[name]["path"];
            }
            return btn;
        }

        private Label MakeLabel(string name, int row, int col)
        {
            var textControl = new Label();
            textControl.Text = name;
            textControl.Parent = propertyTable;
            textControl.Anchor = AnchorStyles.None;
            propertyTable.Controls.Add(textControl, col, row);
            return textControl;
        }
        private Button MakeFieldItemListBox(string name, ComboBox fieldBox, int row, int col)
        {
            var selBtn = new Button();
            selBtn.Text = "选择域";
            var currentSet = new SortedSet<Int32>();
            if (neededInfoDict[name].ContainsKey("fieldItems"))
            {
                var items = ((Newtonsoft.Json.Linq.JArray)neededInfoDict[name]["fieldItems"]).ToObject<List<int>>();
                currentSet = new SortedSet<Int32>(items);
            }
            EventHandler onClick = (obj, arg) =>
            {
                Field f = new Field
                {
                    index = (int)fieldBox.SelectedValue,
                };
                f = pv.FillFieldItemList(f);

                FieldItemSelectorForm win = new FieldItemSelectorForm(f.items, currentSet);
                win.ShowDialog();
                if (win.Ok)
                {
                    currentSet = win.Result;
                    neededInfoDict[name]["fieldItems"] = win.Result;
                }
            };

            selBtn.Click += onClick;
            selBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            propertyTable.Controls.Add(selBtn, col, row);
            return selBtn;
        }

        private List<int> GetSelectedFieldItemIndices(CheckedListBox clb)
        {
            List<int> res = new List<int>();
            foreach (var idx in clb.SelectedIndices)
            {
                res.Add(((List<int>)clb.Tag)[(int)idx]);
            }
            return res;
        }

        private ComboBox MakeFieldComboBox(string name, int row, int col)
        {
            int selected = -1;
            DataRow selRow = null;
            try
            {
                selected = Convert.ToInt32(neededInfoDict[name]["field"]);
            }
            catch(Exception  e)
            {

            }

            var fieldComboBox = new ComboBox();
            var fields = pv.GetFieldList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Index", typeof(int));

            int selectedIndex = -1, curIndex = 0;
            foreach (var f in fields)
            {
                var r = dt.Rows.Add(f.name, f.index);
                if (selected == f.index)
                {
                    selectedIndex = curIndex;
                }
                curIndex++;
            }
            fieldComboBox.DataSource = dt;
            fieldComboBox.DisplayMember = "Name";
            fieldComboBox.ValueMember = "Index";
            fieldComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            fieldComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            fieldComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

            fieldComboBox.Dock = DockStyle.Fill;

            fieldComboBox.SelectedIndexChanged += (obj, e) =>
            {
                neededInfoDict[name]["field"] = fieldComboBox.SelectedValue;
            };

            propertyTable.Controls.Add(fieldComboBox, col, row);

            if (selectedIndex >= 0)
            {
                AfterShown += () =>
                {
                    fieldComboBox.SelectedIndex = selectedIndex;
                };
            }

            return fieldComboBox;
        }

        public string Result
        {
            get
            {
                return JsonConvert.SerializeObject(neededInfoDict, Formatting.Indented);
            }
        }
    }
}
