// preload.js
const { app, contextBridge, ipcRenderer } = require('electron');

console.log("preload.js loaded");

contextBridge.exposeInMainWorld('electron', {
    ipcRenderer: {
        send: (channel, data) => ipcRenderer.send(channel, data),
        on: (channel, func) => ipcRenderer.on(channel, (event, ...args) => func(...args))
    }
});

console.log("ipcRenderer exposed in main world");

app.on('certificate-error', (event, webContents, url, error, certificate, callback) => {
    event.preventDefault();
    callback(true);
});

const mainWindow = new BrowserWindow({
    webPreferences: {
        webSecurity: false
    }
})

//app.on('web-contents-created', (event, contents) => {
//    contents.on('new-window', (event, url) => {
//        event.preventDefault();
//        shell.openExternal(url);  // This will open the link in the system's default browser (Chrome if set)
//   });
//});

