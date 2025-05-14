using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LanguageSwitcher
{
    public class LanguageSwitcherApp : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private KeyboardHook keyboardHook;
        private LanguageManager languageManager;
        private string currentLanguage = "Unknown";

        public LanguageSwitcherApp()
        {
            // Initialize components
            InitializeComponents();
            
            // Setup language manager
            languageManager = new LanguageManager();
            UpdateLanguageIcon();
            
            // Setup keyboard hook
            keyboardHook = new KeyboardHook();
            keyboardHook.KeyDown += KeyboardHook_KeyDown;
        }

        private void InitializeComponents()
        {
            // Create a hidden form
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(1, 1);
            this.Load += (s, e) => this.Hide();

            // Create tray icon and menu
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Current Language: Unknown", null, null);
            trayMenu.Items.Add("-"); // Separator
            trayMenu.Items.Add("Quit", null, QuitApplication);

            trayIcon = new NotifyIcon
            {
                Text = "Language Switcher",
                Icon = SystemIcons.Application, // Default icon
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.DoubleClick += (s, e) => ShowLanguageInfo();
        }

        private void KeyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            // Check for single Ctrl key press
            if (e.KeyCode == Keys.LControlKey)
            {
                // Switch to first language
                languageManager.SwitchToFirstLanguage();
                UpdateLanguageIcon();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.RControlKey)
            {
                // Switch to second language
                languageManager.SwitchToSecondLanguage();
                UpdateLanguageIcon();
                e.Handled = true;
            }
        }

        private void UpdateLanguageIcon()
        {
            currentLanguage = languageManager.GetCurrentLanguage();
            trayMenu.Items[0].Text = $"Current Language: {currentLanguage}";
            
            // Update tray text
            trayIcon.Text = $"Language Switcher - {currentLanguage}";
            
            // You could also change the icon based on the language
            // trayIcon.Icon = ...
        }

        private void ShowLanguageInfo()
        {
            MessageBox.Show(
                $"Current Language: {currentLanguage}\n\n" +
                "Use Left Ctrl to switch to first language\n" +
                "Use Right Ctrl to switch to second language",
                "Language Switcher",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void QuitApplication(object sender, EventArgs e)
        {
            // Clean up and exit
            if (keyboardHook != null)
            {
                keyboardHook.Dispose();
            }
            
            trayIcon.Visible = false;
            Application.Exit();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (keyboardHook != null)
                {
                    keyboardHook.Dispose();
                }
                
                if (trayIcon != null)
                {
                    trayIcon.Dispose();
                }
            }
            
            base.Dispose(disposing);
        }
    }
}