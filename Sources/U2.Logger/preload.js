const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('api', {
    openListWindow: () => ipcRenderer.send('open-list-window'),
    resizeWindow: (width, height) => ipcRenderer.invoke('resize-window', width, height),
    getWindowSize: () => ipcRenderer.invoke('get-window-size')
});
