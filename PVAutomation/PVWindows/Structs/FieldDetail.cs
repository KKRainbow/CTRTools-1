using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PVAutomation.PVWindows.Structs
{
    public class FieldDetail
    {
        public string fieldName;
        public SortedSet<string> fieldItems = new SortedSet<string>();
        public SortedSet<int> fieldItemsIndex = new SortedSet<int>();
    }
}
