using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterfaces
{
    enum InfoType
    {
        Field, //Need specify a field
        FieldItems, //Need to specify a field with selected items
        FieldItemsAlias, //Need to specify a field with selected items and alias
        Integer, //Need a int number
        Float, //Need a float number
    }
    class NeededInfo
    {

    }
    public struct FiedDetail
    {
        string name;
        List<string> fieldItems;
    }
    public interface IPlugin
    {
        string GetNeededInfoDescription();
    }
}
