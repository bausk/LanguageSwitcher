const { contextBridge, ipcRenderer } = require('electron');

// Expose protected methods that allow the renderer process to use
// the ipcRenderer without exposing the entire object
contextBridge.exposeInMainWorld(
  'api', {
    getCurrentLanguage: () => ipcRenderer.invoke('get-current-language'),
    onLanguageChange: (callback) => {
      ipcRenderer.on('language-changed', (_, language) => callback(language));
    },
    quit: () => ipcRenderer.send('quit-app')
  }
);