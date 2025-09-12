const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('api', {
    openListWindow: () => ipcRenderer.send('open-list-window')
});
