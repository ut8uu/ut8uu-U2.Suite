const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');

function createMainWindow() {
    const win = new BrowserWindow({
        width: 800,
        height: 510,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            nodeIntegration: false,
            contextIsolation: true
        }
    });

    win.loadFile('index.html');
    win.setMenu(null);

    // Listen for the 'close' event on the main window.
    // When the main window is closed, we will find and close any other open windows.
    win.on('close', () => {
        const allWindows = BrowserWindow.getAllWindows();
        allWindows.forEach(window => {
            // Do not close the main window itself, as it is already being closed.
            if (window.id !== win.id) {
                window.close();
            }
        });
    });
}

function createListWindow() {
    const listWin = new BrowserWindow({
        width: 1000,
        height: 700,
        webPreferences: {
            preload: path.join(__dirname, 'preload.js'),
            nodeIntegration: false,
            contextIsolation: true
        }
    });

    listWin.loadFile('list.html');
    //listWin.setMenu(null); // Optional: Remove the menu bar for a cleaner look
}

app.whenReady().then(() => {
    createMainWindow();

    // Listen for the 'open-list-window' message from the renderer process
    ipcMain.on('open-list-window', () => {
        createListWindow();
    });
});

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit();
    }
});

app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
        createMainWindow();
    }
});
