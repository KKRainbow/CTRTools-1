using PluginInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using PluginInterfaces.Forms;
using PluginTests.NeededInfoGen;

namespace PluginTests
{
    public partial class MainForm : Form
    {
        [Import]
        IPVAutomation pv { get; set; }

        [ImportMany]
        IEnumerable<Lazy<IPlugin, IPluginMetadata>> plugins { get; set; }

        NeedInfoFormGen gen;
        public MainForm()
        {
            InitializeComponent();
            this.Shown += (o, e) =>
            {
                gen.CallAfterShownEvent();
            };
        }

        public void Init()
        {
            gen = new NeedInfoFormGen(propertyTable, plugins.ElementAt(0).Value, pv);
            gen.InitForm();
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(plugins.ElementAt(0).Value.GetTablesToExecute(gen.Result));
        }
    }
}
