using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterfaces
{
    public enum InfoType
    {
        Field, //Need specify a field
        FieldItems, //Need to specify a field with selected items
        FieldItemsAlias, //Need to specify a field with selected items and alias
        Integer, //Need a int number
        Float, //Need a float number
        File, //A file path
        Directory, //A directory path
    }
    class NeededInfo
    {
        public InfoType type;
    }
    public struct FiedDetail
    {
        string name;
        List<string> fieldItems;
    }

    public interface IPlugin
    {
        string GetNeededInfoDescription();
        void RunWithNeededInfo(string neededInfoJson, IPVAutomation pv);
    }
}
