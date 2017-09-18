using System.Linq;
using System.Threading.Tasks;
using System.Windows.Automation;
using wi = TestStack.White.UIItems;

namespace PVAutomation.PVWindows
{
    public enum PenetrationCorrectionOptions
    {
        Week52 = 0,
        Week24,
        Week12,
        Week4,
        None,
    }
    public partial class SwitchingAnalysis
    {
        public SwitchingAnalysis(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        private wi.ListBoxItems.ListBox DemographicFieldBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(
                    wi.Finders.SearchCriteria.ByAutomationId("13")
                    );
            }
        }
        private wi.ListBoxItems.ListBox Period1Box
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(
                    wi.Finders.SearchCriteria.ByAutomationId("12")
                    );
            }
        }
        private wi.ListBoxItems.ListBox Period2Box
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(
                    wi.Finders.SearchCriteria.ByAutomationId("11")
                    );
            }
        }
        private wi.ListBoxItems.ListBox DataFilterBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(
                    wi.Finders.SearchCriteria.ByAutomationId("10")
                    );
            }
        }
        private wi.ListBoxItems.ListBox BrandsBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(
                    wi.Finders.SearchCriteria.ByAutomationId("9")
                    );
            }
        }
        private wi.ListBoxItems.ListBox PrimaryVolumeBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(
                    wi.Finders.SearchCriteria.ByAutomationId("8")
                    );
            }
        }
        private wi.ListBoxItems.ListBox SecondaryVolumeBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(
                    wi.Finders.SearchCriteria.ByAutomationId("7")
                    );
            }
        }
        private wi.ListBoxItems.ListBox DealFilterBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)Window.Get(
                    wi.Finders.SearchCriteria.ByAutomationId("6")
                    );
            }
        }
        private wi.CheckBox RollingGainLossCheckBox
        {
            get
            {
                return (wi.CheckBox)Window.Get(
                    wi.Finders.SearchCriteria.ByAutomationId("2")
                    );
            }
        }

        private Task<wi.IUIItem[]> periodListTask;
        private Task<wi.IUIItem[]> PeriodListTask
        {
            get
            {
                if (periodListTask == null)
                {
                    periodListTask = Task<wi.IUIItem>.Run(() =>
                    {
                        return Window.GetMultiple(wi.Finders.SearchCriteria.ByControlType(ControlType.List));
                    });
                }
                return periodListTask;
            }
        }


        private wi.ListBoxItems.ListBox PeriodLengthListBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)PeriodListTask.Result.ElementAt(0);
            }
        }
        private wi.ListBoxItems.ListBox PeriodIntervalListBox
        {
            get
            {
                return (wi.ListBoxItems.ListBox)PeriodListTask.Result.ElementAt(1);
            }
        }

        private wi.MenuItems.Menu SwitchingAnalysisClearAll
        {
            get
            {
                return Window.MenuBars[1].MenuItem(new string[] { "Edit", "Clear" });
            }
        }
        private wi.MenuItems.Menu ExecuteButton
        {
            get
            {
                return Window.MenuBars[1].MenuItem(new string[] { "Execute" });
            }
        }
        private wi.MenuItems.Menu LoadButton
        {
            get
            {
                return Window.MenuBars[1].MenuItem(new string[] { "File", "Load..." });
            }
        }
        private wi.MenuItems.Menu SaveButton
        {
            get
            {
                return Window.MenuBars[1].MenuItem(new string[] { "File", "Save..." });
            }
        }

    }
}
