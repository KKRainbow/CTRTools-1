using System.Windows.Automation;
using wi = TestStack.White.UIItems;

namespace PVAutomation.PVWindows
{
    public partial class MainWindow
    {
        public wi.ListBoxItems.ListBox FieldListBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(wi.Finders.SearchCriteria.ByControlType(ControlType.List).
                        AndByClassName("ThunderRT6ListBox").AndAutomationId("17"));
            }
        }
        public wi.ListBoxItems.ListBox FieldDescListBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(wi.Finders.SearchCriteria.ByControlType(ControlType.List).
                    AndByClassName("ThunderRT6ListBox").AndAutomationId("13"));
            }
        }

        public wi.IUIItem FieldItemSheet
        {
            get
            {
                return Window.Get(wi.Finders.SearchCriteria.ByClassName("SPR32X30_SpreadSheet"));
            }
        }
        public wi.Button FieldItemSheetClearAllButton
        {
            get
            {
                return (wi.Button)Window.Get(wi.Finders.SearchCriteria.ByAutomationId("2"));
            }
        }

        public wi.MenuItems.Menu MenuSwitchingAnalysis
        {
            get
            {
                return Window.MenuBars[1].MenuItem(new string[] { "Analysis", "Switching Analysis..." });
            }
        }

    }
}
