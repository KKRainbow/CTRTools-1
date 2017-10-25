using PVAutomation.PVWindows.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using TestStack.White.WindowsAPI;
using w = TestStack.White;
using wi = TestStack.White.UIItems;

namespace PVAutomation.PVWindows
{
    public partial class SwitchingAnalysis
    {
        private MainWindow mainWindow;
        private wi.WindowItems.Window Window
        {
            get
            {
                return mainWindow.FindWindowByNamePrefix("Switch Spec");
            }
        }
        public bool SaveSwitchSpec(string name, string description)
        {
            if (name.Length > 8 || name.Length < 1)
            {
                throw new ArgumentOutOfRangeException("name length should not greater than 8");
            }
            mainWindow.ActivateWindow();
            SaveButton.Click();
            bool result = true;
            Window.HandleModelWindow(saveWin =>
            {
                var titleText = (wi.TextBox)saveWin.Get(wi.Finders.SearchCriteria.ByAutomationId("6"));
                var descText = (wi.TextBox)saveWin.Get(wi.Finders.SearchCriteria.ByAutomationId("5"));
                titleText.SetValue(name);
                if (description.Length != 0)
                {
                    descText.SetValue(description);
                }
                saveWin.ClickOK();
                return true;
            });
            Window.HandleModelWindow(w =>
            {
                var desc = w.Get(wi.Finders.SearchCriteria.ByControlType(ControlType.Text));
                Console.WriteLine(desc.Name);
                if (desc.Name.Contains("Exists"))
                {
                    w.Get(wi.Finders.SearchCriteria.ByText("是(Y)")).Click();
                    result =  true;
                }
                else if (desc.Name.Contains("Missing"))
                {
                    w.Close();
                    result = false;
                }
                return true;
            });
            return result;
        }
        private void HandlePenetrationCorrectionOptions(wi.WindowItems.Window win, PenetrationCorrectionOptions op = PenetrationCorrectionOptions.None)
        {
            const string title = "Penetration Correction Options";
            if (win.Title != title)
            {
                throw new ArgumentException("Window title is not " + title);
            }
            if (op != PenetrationCorrectionOptions.None)
            {
                win.Get<wi.ListBoxItems.ListBox>(wi.Finders.SearchCriteria.ByControlType(ControlType.List)).Select((int)op);
            }
        }
        private void HandleWeightedFieldsSpec(wi.WindowItems.Window win)
        {
            const string title = "Weighted Fields Specification";
            if (win.Title != title)
            {
                throw new ArgumentException("Window title is not " + title);
            }
        }

        private void WaitExecuteWindow()
        {
            var proc = UITools.GetProcessBlock("Execute")[0];
            while (!proc.HasExited)
            {
                Thread.Sleep(500);
            }
            Console.WriteLine("Execution complete");
        }
        public void ExecuteSwitchSpec()
        {
            ExecuteButton.Click();
            for (int i = 0; i < 2; i++)
            {
                Window.HandleModelWindow(win =>
                {
                    HandlePenetrationCorrectionOptions(win, PenetrationCorrectionOptions.None);
                    win.ClickOK();
                    return true;
                });
                Window.HandleModelWindow(win =>
                {
                    HandleWeightedFieldsSpec(win);
                    win.ClickOK();
                    return true;
                });
                WaitExecuteWindow();
            }
            var viewer = UITools.GetProcessBlock("Viewer")[0];
            viewer.Kill();
            return;
        }
        private void SetPeriodListBoxValue(wi.ListBoxItems.ListBox box, int value)
        {
            if (value < 1 || value > 520)
            {
                throw new Exception("Fuck! 1-520 required");
            }
            RollingGainLossCheckBox.Checked = true;
            box.SetValue(value.ToString());
        }
        private void SetPeriodLength(int value)
        {
            SetPeriodListBoxValue(PeriodLengthListBox, value);
        }
        private void SetPeriodInterval(int value)
        {
            SetPeriodListBoxValue(PeriodIntervalListBox, value);
        }
        private void ClearSwitchingAnalysisSpec()
        {
            mainWindow.OpenSwitchingAnalysisWindow();
            SwitchingAnalysisClearAll.Click();
            Window.WaitWhileBusy();
            Window.HandleModelWindow(w =>
            {
                w.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.RETURN);
                return true;
            });
        }
        public void SetSpec(SwitchSpec spec)
        {
            ClearSwitchingAnalysisSpec();
            RollingGainLossCheckBox.Checked = spec.isRolling;
            if (spec.isRolling)
            {
                var t = PeriodListTask;
            }
            foreach (var field in typeof(SwitchSpec).GetFields())
            {
                if (field.FieldType == typeof(FieldDetail))
                {
                    var dir = new Dictionary<string, wi.ListBoxItems.ListBox>
                    {
                        { "demographic", DemographicFieldBox},
                        { "period1", Period1Box},
                        { "period2", Period2Box},
                        { "data", DataFilterBox},
                        { "brands", BrandsBox},
                        { "primary", PrimaryVolumeBox},
                        { "secondary", SecondaryVolumeBox},
                    };
                    var keys = dir.Keys;
                    var box = from key in keys
                              where field.Name.ToLower().Contains(key.ToLower())
                              select dir[key];
                    if (box.Count() != 1)
                    {
                        throw new Exception("Internal Error: Unknown field in switch table spec " + field.Name);
                    }
                    var fieldValue = (FieldDetail)field.GetValue(spec);
                    if (fieldValue.fieldName == null || fieldValue.fieldName.Length == 0)
                    {
                        continue;
                    }
                    mainWindow.SetField(fieldValue);
                    var listItem = mainWindow.FieldListBox.Items.Find(x => x.Name == fieldValue.fieldName);
                    listItem.Select();
                    Window.Mouse.DragAndDrop(listItem, box.First());
                    Window.HandleModelWindow(w =>
                    {
                        w.Close();
                        return true;
                    });
                }
            }
            RollingGainLossCheckBox.Checked = spec.isRolling;
            if (spec.isRolling)
            {
                SetPeriodInterval(spec.waveInterval);
                SetPeriodInterval(spec.waveInterval);
                SetPeriodLength(spec.periodLength);
            }
        }
    }
}
