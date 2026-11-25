const { app, BrowserWindow, clipboard, dialog, Menu } = require('electron');
const path = require('node:path');

const icon = path.join(__dirname, 'app-icon.png');
let splashWindow;
let showTimer;
let startupTimer;
let startupComplete = false;
let startupPhase;
let bridgeEndpoint;
let backendErrorOutput = '';
let quitting = false;
let reporting = false;

exports.onStartup = () => {
    app.on('will-quit', () => {
        quitting = true;
        clearTimeout(startupTimer);
    });
    process.on('uncaughtException', error => reportCrash(error.stack || error.message));
    process.on('unhandledRejection', reason => reportCrash(reason?.stack || String(reason)));
    app.whenReady().then(() => {
        if (process.platform === 'darwin') {
            Menu.setApplicationMenu(Menu.buildFromTemplate([
                { label: 'Arcemi Save Game Editor', role: 'appMenu' },
                { role: 'editMenu' },
            ]));
            app.dock.setIcon(icon);
        } else {
            Menu.setApplicationMenu(null);
        }

        if (!reporting && !startupComplete) showTimer = setTimeout(showSplash, 180);
        if (!startupPhase) waitForStartup('Open the local Electron bridge');
    });
};

function waitForStartup(phase, timeout = 45_000) {
    if (startupComplete || quitting || reporting) return;
    startupPhase = phase;
    clearTimeout(startupTimer);
    startupTimer = setTimeout(() => reportCrash('The editor did not respond within the startup time limit.'), timeout);
}

exports.bridgeListening = (address, port) => {
    bridgeEndpoint = address.includes(':') ? `[${address}]:${port}` : `${address}:${port}`;
    waitForStartup('Connect the editor backend to the local Electron bridge');
};

exports.bridgeConnected = () => waitForStartup('Create the editor window');

exports.bridgeFailed = (error) => {
    reportCrash(`Could not bind the local Electron bridge. ${error.code || ''} ${error.message}`);
};

exports.finishStartup = (window) => {
    waitForStartup('Load the editor page', 120_000);
    let loaded = false;
    const finishIfReady = () => {
        if (!loaded || !window.isVisible() || reporting) return;
        startupComplete = true;
        clearTimeout(startupTimer);
    };
    const onLoad = () => {
        if (window.webContents.getURL() === 'about:blank') return;
        window.webContents.removeListener('did-finish-load', onLoad);
        loaded = true;
        finishIfReady();
    };

    window.webContents.on('did-finish-load', onLoad);
    window.webContents.on('did-fail-load', (_event, code, description, _url, isMainFrame) => {
        if (!startupComplete && isMainFrame && code !== -3) {
            reportCrash(`Could not load the local editor page. ${description} (${code}).`);
        }
    });
    window.once('show', () => {
        hideSplash();
        finishIfReady();
    });
    window.once('closed', () => {
        clearTimeout(startupTimer);
        hideSplash();
    });
    window.webContents.on('before-input-event', (event, input) => {
        if (input.type !== 'keyDown' || input.key.toLowerCase() !== 'i') return;

        const isShortcut = process.platform === 'darwin'
            ? (input.meta && input.alt && !input.control && !input.shift)
            : (input.control && input.shift && !input.alt && !input.meta);
        if (!isShortcut) return;

        event.preventDefault();
        if (!input.isAutoRepeat) window.webContents.toggleDevTools();
    });
    window.webContents.on('render-process-gone', (_event, details) => {
        if (details.reason !== 'clean-exit') {
            reportCrash(`The editor renderer exited: ${details.reason} (${details.exitCode}).`);
        }
    });
};

function showSplash() {
    const window = new BrowserWindow({
        width: 420,
        height: 200,
        title: 'Arcemi Save Game Editor',
        backgroundColor: '#172733',
        frame: false,
        show: false,
        skipTaskbar: true,
        resizable: false,
        focusable: false,
        closable: false,
        icon,
        webPreferences: { sandbox: true },
    });

    splashWindow = window;
    window.once('ready-to-show', () => window.show());
    window.loadFile(path.join(__dirname, 'startup.html')).catch((error) => {
        console.warn('Could not show the startup screen:', error.message);
        if (!window.isDestroyed()) window.hide();
    });
}

function hideSplash() {
    clearTimeout(showTimer);
    if (splashWindow && !splashWindow.isDestroyed()) {
        splashWindow.destroy();
    }
    splashWindow = undefined;
}

exports.watchBackend = (backend) => {
    backend.stderr.on('data', chunk => {
        backendErrorOutput = (backendErrorOutput + chunk.toString()).slice(-12_000);
    });
    backend.once('error', error => reportCrash(error.stack || error.message));
    backend.once('close', (code, signal) => {
        if (!startupComplete || code !== 0) {
            reportCrash(`The editor backend exited (${signal || code}).`);
        }
    });
};

async function reportCrash(details) {
    if (quitting || reporting) return;
    reporting = true;
    clearTimeout(showTimer);
    clearTimeout(startupTimer);
    await app.whenReady();
    if (splashWindow && !splashWindow.isDestroyed()) splashWindow.hide();

    const duringStartup = !startupComplete;
    const report = [
        `Arcemi Save Game Editor ${app.getVersion()}`,
        `Platform: ${process.platform} ${process.arch}`,
        `UTC: ${new Date().toISOString()}`,
        ...(duringStartup ? [`Startup stage: ${startupPhase || 'Start the editor'}`] : []),
        ...(duringStartup && bridgeEndpoint ? [`Local bridge: ${bridgeEndpoint}`] : []),
        '',
        details,
        ...(backendErrorOutput.trim() ? ['', 'Backend error output', backendErrorOutput.trim()] : []),
    ].join('\n');
    const safeReport = redactReport(report);
    const instructions = duringStartup
        ? 'The editor uses a connection within this computer. If you use a VPN, proxy, or firewall, check whether it permits connections to 127.0.0.1 and ::1. Copy the report for troubleshooting.'
        : 'Copy the error report if you want to share it. Unsaved edits may be lost.';

    try {
        let detail = instructions;
        while (true) {
            const { response } = await dialog.showMessageBox({
                type: 'error',
                title: 'Arcemi Save Game Editor',
                message: duringStartup ? 'The editor could not start' : 'The editor needs to restart',
                detail,
                buttons: ['Copy report', 'Restart editor', 'Close editor'],
                defaultId: 1,
                cancelId: 2,
                noLink: true,
            });

            if (response === 0) {
                try {
                    await clipboard.writeText(safeReport);
                    detail = 'The report was copied. Review it before sharing.';
                } catch (error) {
                    console.error('Could not copy the editor error report:', error);
                    detail = 'The clipboard is unavailable. You can still restart or close the editor.';
                }
                continue;
            }

            quitting = true;
            if (response === 1) {
                app.relaunch({ execPath: process.env.APPIMAGE || process.execPath });
            }
            app.quit();
            return;
        }
    } catch (error) {
        console.error('Could not show the editor error report:', error);
    } finally {
        reporting = false;
    }
}

function redactReport(report) {
    if (typeof global.authToken === 'string' && global.authToken) {
        report = report.replaceAll(global.authToken, '[redacted]');
    }
    return report.replace(/([?&]token=|\/electronAuthToken=|Use Auth Token:\s*)[^\s&]+/gi, '$1[redacted]');
}
