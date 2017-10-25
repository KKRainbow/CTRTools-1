using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterfaces
{
    public class FieldDetail
    {
        public string name;
        public List<int> fieldItems = new List<int>();
    }

    public interface IPlugin
    {
        string GetNeededInfoDescription();
        string GetTablesToExecute(string neededInfoJson);
    }

    public interface IPluginMetadata
    {
        string Name { get; }
        string Description { get; }
        string Period { get; }
    }
}
