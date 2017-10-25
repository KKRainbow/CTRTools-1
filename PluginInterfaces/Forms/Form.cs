using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginInterfaces.Forms
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
    public class FormItemBase
    {
        public string name;
        public string description;
        public virtual InfoType Type { get; set; }
    }
    public class FieldForm : FormItemBase
    {
        public override InfoType Type
        {
            get { return InfoType.Field; }
        }
    }
    public class FileForm : FormItemBase
    {
        public override InfoType Type
        {
            get { return InfoType.File; }
        }
        public string filter;
        public bool isDir = false;
        public bool isMulti = false;
        public bool isSave = false;
    }
    public class FieldItems : FormItemBase
    {
        public override InfoType Type
        {
            get { return InfoType.FieldItems; }
        }
    }
    public class FieldItemsAlias : FormItemBase
    {
        public override InfoType Type
        {
            get { return InfoType.FieldItemsAlias; }
        }
    }
}
