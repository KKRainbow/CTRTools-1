using PVAutomation.PVFields;
using PVAutomation.PVWindows.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Automation;
using TestStack.White.InputDevices;
using TestStack.White.UIA;
using TestStack.White.WindowsAPI;
using w = TestStack.White;
using wi = TestStack.White.UIItems;

namespace PVAutomation.PVWindows
{
    public partial class MainWindow
    {
        w.Application app;
        Process appProcess;
        PVConfig config;

        private Dictionary<string, wi.WindowItems.Window> prefixWindowMap = new Dictionary<string, wi.WindowItems.Window>();

        const int MaxWheel = 267;

        string dbFolder;
        public MainWindow(string name = "Powrvw-V")
        {
            var pvProc = Process.GetProcessesByName("Powrvw-V");
            if (pvProc.Length != 1)
            {
                throw new Exception("Please open exactly one pv process");
            }

            app = w.Application.Attach(pvProc[0]);
            appProcess = pvProc[0];

            config = new PVConfig(appProcess.StartInfo.FileName);
            dbFolder = config.Get(PVConfig.Keys.DatabaseFolder);

            if (dbFolder == null || !File.Exists(dbFolder))
            {
                throw new Exception("Cannot get db folder of " + dbFolder);
            }
        }
        ~MainWindow()
        {
            app.Dispose();
        }
        private wi.WindowItems.Window Window
        {
            get
            {
                const string MainWindowPrefix = "Kantar Worldpanel Powerview";
                return FindWindowByNamePrefix(MainWindowPrefix);
            }
        }

        public wi.WindowItems.Window FindWindowByNamePrefix(string prefix)
        {
            if (prefixWindowMap.ContainsKey(prefix))
            {
                return prefixWindowMap[prefix];
            }
            foreach (var w in this.app.GetWindows())
            {
                if (w.Name.StartsWith(prefix))
                {
                    prefixWindowMap[prefix] = w;
                    return w;
                }
            }
            return null;
        }

        public void ActivateWindow()
        {
            UITools.SetForegroundWindow(this.appProcess.MainWindowHandle);
        }

        private void ClickFieldItem(int i, int total, bool multi = true)
        {
            var sheet = FieldItemSheet;
            const int cellPerWhell = 3;
            ActivateWindow();
            FieldItemSheet.Focus();
            // 计算一个单元格的高度
            var sheetHeight = sheet.Bounds.Height;
            var cellHeight = sheetHeight / 43;
            Console.WriteLine("Cell Height:", cellHeight);
            Func<int, int> clickCell = (x) =>
            {
                // 注意表头
                double posy = cellHeight * (x + 1) + sheet.Bounds.Top + cellHeight / 2;
                double posx = sheet.Bounds.Left + sheet.Bounds.Width / 8;
                Window.Mouse.Location = new System.Windows.Point(posx, posy);
                if (multi)
                {
                    Window.Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);
                }
                Window.Mouse.Click();
                if (multi)
                {
                    Window.Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
                }
                return 1;
            };
            Window.Mouse.Wheel(100);
            if (i < 42)
            {
                clickCell(i);
            }
            else if (i + 42 >= total)
            {
                Window.Mouse.Wheel(-100);
                clickCell(i - (total - 42));
            }
            else
            {
                clickCell(-(i % cellPerWhell - 1));
            }
        }

        public IEnumerable<string> GetFieldItemsOfSelectedField()
        {
            var fn = FieldListBox.SelectedItemText;
            var fdata = FieldFile.GetFieldFile(fn, databaseDir);
            var ds = fdata.ReadFieldHeaders();
            return from d in ds select d.name;
        }


        private void SelectFieldItems(IEnumerable<int> indices)
        {
            var items = GetFieldItemsOfSelectedField();
            var totalItems = items.Count();
            var mouse = Window.Mouse;
            // 取消掉所有
            FieldItemSheetClearAllButton.Click();

            var sheet = FieldItemSheet;
            const int cellPerWhell = 3;
            double cellHeight = 18.0, borderWidth = 2.0;
            int maxCellInScreen = (int)Math.Round((sheet.Bounds.Height - 4.0) / cellHeight) - 1;
            ActivateWindow();
            mouse.Location = FieldItemSheet.Bounds.Center();
            // 计算一个单元格的高度, 注意表头
            var sheetHeight = sheet.Bounds.Height;
            Func<int, System.Windows.Point> getCellPos = (x) =>
            {
                // 注意表头
                double posy = cellHeight * (x + 1) + sheet.Bounds.Top + cellHeight / 2 + borderWidth;
                double posx = sheet.Bounds.Left + sheet.Bounds.Width / 8;
                return new System.Windows.Point(posx, posy);
            };
            Func<int, int> clickCell = (x) =>
            {
                Window.Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);
                mouse.Click(getCellPos(x));
                Window.Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
                return 1;
            };

            Func<int, int, int> dragCell = (x, y) =>
            {
                Window.Keyboard.HoldKey(KeyboardInput.SpecialKeys.CONTROL);
                for (int i = x; i <= y; i++)
                {
                    mouse.Location = getCellPos(i);
                    Mouse.MouseLeftButtonUpAndDown();
                }
                Window.Keyboard.LeaveKey(KeyboardInput.SpecialKeys.CONTROL);
                return 1;
            };

            WheelSheet(items.Count() / 3 + 100);

            //如果总数不是3的倍数，那么会有空行
            if (totalItems % 3 != 0)
            {
                totalItems = totalItems + (3 - totalItems % 3);
            }

            indices = (from index in indices orderby index ascending select index).Distinct();
            int cur = 0, len = indices.Count();
            int minIndexInScreen = 0, maxIndexInScreen = minIndexInScreen + maxCellInScreen;
            while (cur < len)
            {
                Console.WriteLine("{0},{1}", minIndexInScreen, maxIndexInScreen);
                // 选所有现在能选的
                var avail = indices.Skip(cur).TakeWhile(x => x < maxIndexInScreen && x - minIndexInScreen < maxCellInScreen);
                int low = -1, contCount = 0;
                for (int i = 0; i <= avail.Count(); i++)
                {
                    int a = i < avail.Count() ? avail.ElementAt(i) : 999999999;
                    if (low == -1)
                    {
                        low = a;
                        continue;
                    }
                    if (a - low == contCount + 1)
                    {
                        // 连续的
                        contCount++;
                        continue;
                    }
                    else
                    {
                        if (contCount == 0)
                        {
                            Console.WriteLine("{0}, min: {1}", low, minIndexInScreen);
                            clickCell(low - minIndexInScreen);
                        }
                        else
                        {
                            Console.WriteLine("{0}-{1}, min: {2}", low, low + contCount, minIndexInScreen);
                            dragCell(low - minIndexInScreen, low + contCount - minIndexInScreen);
                        }
                        contCount = 0;
                        low = a;
                    }
                }
                cur += avail.Count();
                if (cur < len)
                {
                    //尽可能的移动窗口
                    var lowest = indices.ElementAt(cur);
                    int diff = lowest - minIndexInScreen;
                    var count = diff / cellPerWhell;
                    if (lowest >= totalItems - maxCellInScreen)
                    {
                        minIndexInScreen = totalItems - maxCellInScreen;
                        maxIndexInScreen = totalItems;
                    }
                    else
                    {
                        minIndexInScreen += count * cellPerWhell;
                        maxIndexInScreen += count * cellPerWhell;
                    }
                    WheelSheet(-count);
                }
            }
        }

        private void WheelSheet(int delta)
        {
            var mouse = Window.Mouse;
            int sign = delta / Math.Abs(delta);
            mouse.Location = FieldItemSheet.Bounds.Center();
            for (int i = 0; i < Math.Abs(delta) / MaxWheel; i++)
            {
                mouse.Location = FieldItemSheet.Bounds.Center();
                mouse.Wheel(MaxWheel * sign);
            }
            mouse.Location = FieldItemSheet.Bounds.Center();
            mouse.Wheel((Math.Abs(delta) % MaxWheel) * sign);
            mouse.Location = FieldItemSheet.Bounds.Center();
        }

        public void OpenSwitchingAnalysisWindow()
        {
            ActivateWindow();
            MenuSwitchingAnalysis.Click();
        }
        public void SetField(FieldDetail field)
        {
            Console.WriteLine("Searching field {0}", field.fieldName);
            FieldDescListBox.Items[FieldListBox.Items.FindIndex(x => x.Name == field.fieldName)].Select();
            if (field.fieldItems.Count == 0 && field.fieldItemsIndex.Count == 0)
            {
                return;
            }
            var headers = FieldFile.GetFieldFile(field.fieldName, dbFolder).ReadFieldHeaders();
            var indices = headers
                          .Select((item, index) => new { item.name, index })
                          .Where(pair => { return field.fieldItems.Contains(pair.name); })
                          .Select(x => x.index)
                          .Concat(field.fieldItemsIndex)
                          .OrderBy(x => x);
            foreach (var idx in indices)
            {
                Console.WriteLine("{0} -> {1}", idx, headers[idx].name);
            }
            SelectFieldItems(indices);
        }
    }
}
