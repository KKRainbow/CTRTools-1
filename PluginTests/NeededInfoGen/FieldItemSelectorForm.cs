using PluginInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PluginTests.NeededInfoGen
{
    public partial class FieldItemSelectorForm : Form
    {
        bool ok = false;
        SortedSet<int> result;
        IEnumerable<FieldItem> fieldItems;
        public FieldItemSelectorForm(IEnumerable<FieldItem> items)
        {
            InitializeComponent();
            fieldItems = items;
            InitForm();
            Shown += (o, e) =>
            {
                fieldItemDataGrid.ClearSelection();
            };
        }
        public FieldItemSelectorForm(IEnumerable<FieldItem> items, IEnumerable<int> selected)
            :this(items)
        {
            Shown += (o, e) =>
            {
                SelectIndices(selected);
            };
        }
        public void InitForm()
        {
            fieldItemDataGrid.DataSource = GetDataTable();
            fieldItemDataGrid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            fieldItemDataGrid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }
        private void SelectIndices(IEnumerable<int> selected)
        {
            if (selected != null)
            {
                SortedSet<int> s = new SortedSet<int>(selected);
                for (int i = 0; i < fieldItemDataGrid.Rows.Count; i++)
                {
                    var row = fieldItemDataGrid.Rows[i];
                    if (s.Contains((int)row.Cells["Index"].Value))
                    {
                        row.Selected = true;
                    }
                }
            }
        }
        private DataTable GetDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("Index", typeof(int));
            dt.Columns.Add("Name");

            if (fieldItems != null)
            {
                foreach (var fi in fieldItems)
                {
                    dt.Rows.Add(fi.index, fi.name);
                }
            }

            return dt;
        }

        public SortedSet<int> Result
        {
            get
            {
                return result;
            }
        }
        public bool Ok
        {
            get
            {
                return ok;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            result = new SortedSet<int>();
            for (int i = 0; i < fieldItemDataGrid.SelectedRows.Count; i++)
            {
                var row = fieldItemDataGrid.SelectedRows[i];
                result.Add((int)row.Cells["Index"].Value);
            }
            ok = true;
            this.Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            ok = false;
            this.Close();
        }

        private void selectAllExceptFirstButton_Click(object sender, EventArgs e)
        {
            fieldItemDataGrid.SelectAll();
            fieldItemDataGrid.Rows[0].Selected = false;
        }
    }
}
