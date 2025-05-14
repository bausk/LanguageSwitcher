using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace LanguageSwitcher
{
    /// <summary>
    /// Class to manage language/keyboard layout switching
    /// </summary>
    public class LanguageManager
    {
        // Windows API declarations
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int LoadKeyboardLayout(string pwszKLID, uint Flags);

        [DllImport("user32.dll")]
        private static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, uint Flags);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardLayoutList(int nBuff, [Out] IntPtr[] lpList);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetKeyboardLayoutName([Out] StringBuilder pwszKLID);

        // Constants
        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private const uint KLF_ACTIVATE = 0x00000001;

        // Properties for storing available language IDs
        private IntPtr[] _availableLayouts;
        private int _firstLanguageIndex = 0;
        private int _secondLanguageIndex = 1;

        public LanguageManager()
        {
            // Get a list of installed language layouts
            InitializeAvailableLayouts();
        }

        private void InitializeAvailableLayouts()
        {
            // Get count of keyboard layouts
            int count = GetKeyboardLayoutList(0, null);
            
            // Allocate array and get layouts
            _availableLayouts = new IntPtr[count];
            GetKeyboardLayoutList(count, _availableLayouts);
            
            // Ensure indexes are valid
            _firstLanguageIndex = 0;
            _secondLanguageIndex = count > 1 ? 1 : 0;
        }

        /// <summary>
        /// Get the current language name
        /// </summary>
        public string GetCurrentLanguage()
        {
            try
            {
                // Get current keyboard layout
                IntPtr foregroundWindow = GetForegroundWindow();
                uint threadId = GetWindowThreadProcessId(foregroundWindow, out uint _);
                IntPtr keyboardLayout = GetKeyboardLayout(threadId);

                // Get name of keyboard layout
                StringBuilder name = new StringBuilder(256);
                GetKeyboardLayoutName(name);
                
                // Format the name based on the language identifier
                int langId = keyboardLayout.ToInt32() & 0xFFFF;
                
                // You could add a mapping of language IDs to human-readable names here
                // For now, we'll use the language ID in hex
                return $"0x{langId:X4}";
            }
            catch (Exception ex)
            {
                return $"Unknown ({ex.Message})";
            }
        }

        /// <summary>
        /// Switch to the first language
        /// </summary>
        public void SwitchToFirstLanguage()
        {
            if (_availableLayouts != null && _availableLayouts.Length > 0)
            {
                SwitchToLanguage(_availableLayouts[_firstLanguageIndex]);
            }
        }

        /// <summary>
        /// Switch to the second language
        /// </summary>
        public void SwitchToSecondLanguage()
        {
            if (_availableLayouts != null && _availableLayouts.Length > 1)
            {
                SwitchToLanguage(_availableLayouts[_secondLanguageIndex]);
            }
        }

        /// <summary>
        /// Switch to a specific language using its handle
        /// </summary>
        private void SwitchToLanguage(IntPtr layoutHandle)
        {
            try
            {
                // Get current foreground window
                IntPtr foregroundWindow = GetForegroundWindow();
                
                // Send message to change input language
                PostMessage(foregroundWindow, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, layoutHandle);
                
                // Also activate the keyboard layout
                ActivateKeyboardLayout(layoutHandle, KLF_ACTIVATE);
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                MessageBox.Show($"Error switching language: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}