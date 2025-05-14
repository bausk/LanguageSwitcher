using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace LanguageSwitcher.Setup
{
    class SetupProgram
    {
        static void Main(string[] args)
        {
            try 
            {
                // Add to startup
                AddToStartup();
                MessageBox.Show("Language Switcher has been installed and set to run at startup.", 
                    "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Start the application
                string appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LanguageSwitcher.exe");
                Process.Start(appPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Installation failed: {ex.Message}", 
                    "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        static void AddToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                string appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LanguageSwitcher.exe");
                key.SetValue("LanguageSwitcher", $"\"{appPath}\"");
            }
        }
    }
}