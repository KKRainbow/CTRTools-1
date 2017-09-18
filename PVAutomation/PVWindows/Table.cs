using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w = TestStack.White;
using wi = TestStack.White.UIItems;

namespace PVAutomation.PVWindows
{
    class Table
    {
        private MainWindow mainWindow;
        public wi.WindowItems.Window Window
        {
            get
            {
                const string mainWindowPrefix = "Table Spec";
                var pvTableSpec = mainWindow.FindWindowByNamePrefix(mainWindowPrefix);
                return pvTableSpec;
            }
        }
        public Table(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
    }
}
