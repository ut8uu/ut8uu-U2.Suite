const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');

let mainWindow;

function createWindow() {
    mainWindow = new BrowserWindow({
        width: 600,
        height: 400,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js')
        }
    });

    mainWindow.loadFile('index/index.html');
}

app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
        createWindow();
    }
});

// IPC Handler to open a new window
ipcMain.on('open-list-window', () => {
    const listWindow = new BrowserWindow({
        width: 800,
        height: 600,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js')
        }
    });
    listWindow.loadFile('list/list.html');
});

// IPC Handler for resizing the window
ipcMain.handle('resize-window', (event, width, height) => {
    const webContents = event.sender;
    const window = BrowserWindow.fromWebContents(webContents);
    if (window) {
        window.setSize(width, height);
    }
});

// New IPC Handler to get the current window size
ipcMain.handle('get-window-size', (event) => {
    const webContents = event.sender;
    const window = BrowserWindow.fromWebContents(webContents);
    if (window) {
        return window.getSize();
    }
    return [0, 0];
});
