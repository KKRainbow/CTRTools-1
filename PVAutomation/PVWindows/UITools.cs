using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using w = TestStack.White;
using wi = TestStack.White.UIItems;

namespace PVAutomation.PVWindows
{
    static class UITools
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetForegroundWindow(IntPtr windowHandle);
        [DllImport("user32", EntryPoint = "SendInput")]
        static extern int SendInput(uint numberOfInputs, ref Input input, int structSize);
        [DllImport("user32", EntryPoint = "SendInput")]
        static extern int SendInput64(int numberOfInputs, ref Input64 input, int structSize);
        [DllImport("user32.dll")]
        static extern IntPtr GetMessageExtraInfo();
        public static void Wheel(int delta) 
        {
            object minput = new MouseInput(WindowsConstants.MOUSEEVENTF_WHEEL, GetMessageExtraInfo());
            var prop = minput.GetType().GetField("mouseData", BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(minput, 120 * delta);
            Input input = InputFactory.Mouse((MouseInput)minput);
            // Added check for 32/64 bit  
            if (IntPtr.Size == 4)
            {
                var v = SendInput(1, ref input, Marshal.SizeOf(typeof(Input)));
            }
            else
            {
                var input64 = new Input64(input);
                SendInput64(1, ref input64, Marshal.SizeOf(typeof(Input)));
            }
        }
        public static void Wheel(this AttachedMouse mouse, int delta)
        {
            Wheel(delta);
        }
        public static void Wheel(this Mouse mouse, int delta)
        {
            Wheel(delta);
        }
        public static Process[] GetProcessBlock(string name)
        {
            var procs = new Process[0];
            while (procs.Length == 0)
            {
                procs = Process.GetProcessesByName(name);
                Thread.Sleep(1000);
            }
            return procs;
        }

        public static void ClickButton(this wi.WindowItems.Window win, string text)
        {
            win.Get(wi.Finders.SearchCriteria.ByText(text)).Click();
        }
        public static void ClickOK(this wi.WindowItems.Window win)
        {
            ClickButton(win, "OK");
        }

        public static void ClickCancel(this wi.WindowItems.Window win)
        {
            ClickButton(win, "Cancel");
        }
        public static void HandleModelWindow(this wi.WindowItems.Window win, Func<wi.WindowItems.Window, bool> func)
        {
            foreach (var w in win.ModalWindows())
            {
                func(w);
            }
        }
    }
}
