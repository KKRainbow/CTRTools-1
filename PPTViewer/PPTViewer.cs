using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PowerPoint = NetOffice.PowerPointApi;
using Office = NetOffice.OfficeApi;


namespace PPTViewer
{
    public partial class PPTViewer : Form
    {
        PowerPoint.Application ppt;
        PowerPoint.Presentation pret;
        System.Timers.Timer timer;
        public PPTViewer()
        {
            InitializeComponent();
            ppt = new PowerPoint.Application();
            ppt.Visible = Office.Enums.MsoTriState.msoTrue;
            timer = new System.Timers.Timer(200);
            timer.Elapsed += (sender, e) => refresh();
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Multiselect = false;
            fd.CheckFileExists = true;
            fd.RestoreDirectory = true;
            fd.InitialDirectory = "D:\\孙思杰\\桌面\\女王大人の任务\\花王\\新建文件夹";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                string filename = fd.FileName;
                pret = ppt.Presentations.Open(filename);
                timer.Start();
            };
        }

        private void refresh()
        {
            string res = "";
            foreach (PowerPoint.Shape shape in ppt.ActiveWindow.Selection.ShapeRange)
            {
                res += string.Format("Name: {0}, ID: {1}\n", shape.Name, shape.Id);
                res += string.Format("Left: {0}, Top: {1}, Width: {2}, Height: {3}\n", shape.Left, shape.Top, shape.Width, shape.Height);
            }
            label_show.Text = res;
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            timer.Stop();
            pret.Close();
            ppt.Quit();
        }
    }
}
