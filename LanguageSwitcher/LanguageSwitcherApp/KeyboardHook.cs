using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LanguageSwitcher
{
    /// <summary>
    /// Class for hooking low level keyboard events
    /// </summary>
    public class KeyboardHook : IDisposable
    {
        // Windows constants
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        // Delegate for the keyboard hook callback
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookId = IntPtr.Zero;

        // Event for key press
        public event KeyEventHandler KeyDown;

        // P/Invoke declarations
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // Constructor
        public KeyboardHook()
        {
            _proc = HookCallback;
            _hookId = SetHook(_proc);
        }

        // Set up the hook
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // Callback for processing keyboard messages
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                // Detect single Ctrl key presses
                if (key == Keys.LControlKey || key == Keys.RControlKey)
                {
                    var e = new KeyEventArgs(key);
                    OnKeyDown(e);
                    
                    // If handled, don't pass to other applications
                    if (e.Handled)
                        return (IntPtr)1;
                }
            }
            
            // Call the next hook in the chain
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        // Raise the KeyDown event
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        // Dispose method to clean up the hook
        public void Dispose()
        {
            if (_hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookId);
                _hookId = IntPtr.Zero;
            }
        }
    }
}