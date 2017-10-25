using PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace PluginTests
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            MainForm wnd = new MainForm();

            AggregateCatalog cata = new AggregateCatalog();
            cata.Catalogs.Add(new DirectoryCatalog(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory)));
            var container = new CompositionContainer(cata);
            try
            {
                container.ComposeParts(wnd);
            }
            catch(CompositionException e)
            {
                foreach (var ex in e.RootCauses)
                {
                    Console.WriteLine(ex.InnerException);
                }
            }

            wnd.Init();
            wnd.ShowDialog();
        }
    }
}
