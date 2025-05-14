const { app, BrowserWindow, Tray, Menu, globalShortcut, ipcMain, nativeImage } = require('electron');
const path = require('path');
const { exec } = require('child_process');
const Store = require('electron-store');

// Setup persistent storage
const store = new Store();

// Keep a global reference of objects to prevent garbage collection
let mainWindow;
let tray;

// Define language state
let currentLanguage = {
  id: 'Unknown',
  name: 'Unknown'
};

// Set to run at startup
const setAutoLaunch = (enable) => {
  app.setLoginItemSettings({
    openAtLogin: enable,
    path: app.getPath('exe')
  });
};

// Get installed keyboard layouts
const getInstalledLanguages = () => {
  return new Promise((resolve, reject) => {
    exec('powershell.exe -Command "Get-WinUserLanguageList | ConvertTo-Json"', (error, stdout) => {
      if (error) {
        console.error(`Error getting language list: ${error.message}`);
        resolve([]);
        return;
      }
      
      try {
        const languages = JSON.parse(stdout);
        const result = Array.isArray(languages) 
          ? languages 
          : [languages]; // Handle case where only one language is installed
        
        resolve(result.map(lang => ({
          id: lang.LanguageTag,
          name: lang.DisplayName,
          inputMethods: lang.InputMethodTips
        })));
      } catch (err) {
        console.error(`Error parsing language list: ${err.message}`);
        resolve([]);
      }
    });
  });
};

// Get the current keyboard layout
const getCurrentLanguage = () => {
  return new Promise((resolve, reject) => {
    exec('powershell.exe -Command "Get-WinUserLanguageList | Where-Object {$_.Autochthonous -eq $true} | ConvertTo-Json"', (error, stdout) => {
      if (error) {
        console.error(`Error getting current language: ${error.message}`);
        resolve({ id: 'Unknown', name: 'Unknown' });
        return;
      }
      
      try {
        const language = JSON.parse(stdout);
        resolve({
          id: language.LanguageTag,
          name: language.DisplayName
        });
      } catch (err) {
        console.error(`Error parsing current language: ${err.message}`);
        resolve({ id: 'Unknown', name: 'Unknown' });
      }
    });
  });
};

// Switch to a specific language
const switchToLanguage = (languageId) => {
  // Windows keyboard shortcut simulation
  // This simulates Alt+Shift to switch languages
  exec(`powershell.exe -Command "Set-WinUserLanguageList -LanguageList '${languageId}' -Force"`, (error) => {
    if (error) {
      console.error(`Error switching language: ${error.message}`);
      return;
    }
    
    // Update the current language state
    getCurrentLanguage().then(language => {
      currentLanguage = language;
      updateTray();
      
      // Notify the renderer process
      if (mainWindow) {
        mainWindow.webContents.send('language-changed', language);
      }
    });
  });
};

// Update the tray icon and menu
const updateTray = () => {
  if (!tray) return;
  
  const contextMenu = Menu.buildFromTemplate([
    { 
      label: `Current Language: ${currentLanguage.name || 'Unknown'}`, 
      enabled: false 
    },
    { type: 'separator' },
    { 
      label: 'Run at Startup', 
      type: 'checkbox',
      checked: app.getLoginItemSettings().openAtLogin,
      click: (menuItem) => {
        setAutoLaunch(menuItem.checked);
      }
    },
    { type: 'separator' },
    { 
      label: 'Quit', 
      click: () => {
        app.quit();
      }
    }
  ]);
  
  tray.setToolTip(`Language Switcher - ${currentLanguage.name || 'Unknown'}`);
  tray.setContextMenu(contextMenu);
};

// Create the browser window (hidden)
function createWindow() {
  mainWindow = new BrowserWindow({
    width: 400,
    height: 300,
    show: false,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false
    }
  });

  // Load the index.html file
  mainWindow.loadFile('index.html');
  
  // Hide the window instead of closing when user clicks X
  mainWindow.on('close', (event) => {
    if (!app.isQuitting) {
      event.preventDefault();
      mainWindow.hide();
      return false;
    }
  });
}

// Setup the tray icon
function createTray() {
  tray = new Tray(nativeImage.createFromDataURL('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAALZSURBVFhH7ZZNSFRRFMfP+968cUYnR/swZspKSawoiPZBQYvoAyIialmLNrWobVGL2rQIatGiRauWQZuIIKhNQUFFH0aZFo1mTjrjfLx5783/nPvem4/3ZkTGRQz94XDPu/eec/537rn3vKfoP+eXCJimScNlYjljlGAcDnuM6JMWziyeRRUgZlhUVS5rbpyisXMKstkjsthKTnQBrLNY93rgXQw9HYXeHGVl31Q9aSL2vUQVwMe0ZvOWwGBH0ht7pVp7rFQnWLtDH5RbzNKR4lp2fDQl+lwzYn7QRyWyLYHBvgmPz/9Cy7MWfSKiw6Sc8Xr2dZZJdlvyRLg3S0tCCsA4a1K97EKDEukPmvr6nDLDIvhM4xFPHUxTMZM3nW+zCeQQnQTbUJzc6M+R5i9XlBegwPRPKE3Vb0QQD3N3rDFHztKTQW/vI9VkS58YXadQpAtLNxI+cXHUKiIqwBbvfGaB99WkEdxD6IfohZiZtdWqR5jNpknJa0oObFTCXQAdRMnq+GYDFwMlpwvHmyWlbEQRBRZELgZKThcKCuAm23OwKHCXeE0uOV3YVoCVhG/zVS45XdilwAYW4Ay5nY2MrWNSCZecLmwBsJuJ8mwdLo4q0fNw8igtEbsKPDjZ0PzGMc54sSxnlRt+1Xx16EGtUFoM9pqQ0Wjt3bTZtlQCY1mOJWiPv4Zu26Rt+Pq1+5K3htsqmWmxACyC0kLYbsRMXLp3u/7Gm/FJbr+EnnAi05k2tW9jvvbmmHtHzcBAqEF4DmwBVp7yw9GYr2vo6riPYR923LizvRxwWgIo4PWFR9wJz+7qPdVfU2KKDYN0AQ5iVJ9T1j1RV5l4+tkzOTQynkgR3f14ryv+WPg83TE1IyrTEXwQwS7QFR1ctnf7Fh6PUTUGdgLn+TnRQO9gVIr4mJADsB3cBJCxZ8HhKRfiLSM4Ac/iFSgtK9t65f5vCPkJOgHEvHBYYlMAAAAASUVORK5CYII='));
  
  updateTray();
  
  // Show/hide the window on tray icon click
  tray.on('click', () => {
    if (mainWindow.isVisible()) {
      mainWindow.hide();
    } else {
      mainWindow.show();
    }
  });
}

// Register global shortcuts
function registerShortcuts(languages) {
  // Unregister any existing shortcuts
  globalShortcut.unregisterAll();
  
  // Only proceed if we have languages to work with
  if (!languages || languages.length === 0) {
    console.error('No languages available for shortcuts');
    return;
  }
  
  // Register left Ctrl for first language
  globalShortcut.register('ControlLeft', () => {
    if (languages.length > 0) {
      switchToLanguage(languages[0].id);
    }
  });
  
  // Register right Ctrl for second language (if available)
  globalShortcut.register('ControlRight', () => {
    if (languages.length > 1) {
      switchToLanguage(languages[1].id);
    } else if (languages.length > 0) {
      // If only one language is available, use that
      switchToLanguage(languages[0].id);
    }
  });
}

// Handle IPC messages
ipcMain.handle('get-current-language', async () => {
  return currentLanguage;
});

ipcMain.on('quit-app', () => {
  app.isQuitting = true;
  app.quit();
});

// App lifecycle events
app.whenReady().then(async () => {
  createWindow();
  createTray();
  
  // Auto launch setting (default to true)
  const shouldAutoLaunch = store.get('autoLaunch', true);
  setAutoLaunch(shouldAutoLaunch);
  
  // Get installed languages and current language
  const languages = await getInstalledLanguages();
  currentLanguage = await getCurrentLanguage();
  
  // Register keyboard shortcuts
  registerShortcuts(languages);
  
  // Update the tray with current language
  updateTray();
  
  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow();
    }
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('will-quit', () => {
  // Unregister all shortcuts
  globalShortcut.unregisterAll();
});