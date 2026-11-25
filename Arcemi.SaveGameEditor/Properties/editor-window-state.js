const { app, BrowserWindow, screen } = require('electron');
const fs = require('node:fs');
const path = require('node:path');
const { finishStartup } = require('./custom_main');

module.exports = function createEditorWindow(options) {
    const statePath = path.join(app.getPath('userData'), 'window-state.json');
    let state;
    try {
        state = JSON.parse(fs.readFileSync(statePath, 'utf8'));
    } catch (error) {
        if (error.code !== 'ENOENT') {
            console.warn('Could not read the editor window size:', error.message);
        }
    }

    const workArea = screen.getPrimaryDisplay().workAreaSize;
    const minWidth = Math.min(800, workArea.width);
    const minHeight = Math.min(540, workArea.height);
    const fit = (saved, fallback, min, max) => {
        const size = Number.isSafeInteger(saved) && saved > 0 ? saved : fallback;
        return Math.max(min, Math.min(size, max));
    };

    const titleBar = process.platform === 'darwin' ? {} : {
        titleBarStyle: 'hidden',
        titleBarOverlay: { color: '#172733', symbolColor: '#f3f6f8', height: 36 },
    };

    const window = new BrowserWindow({
        ...options,
        ...titleBar,
        icon: path.join(__dirname, 'app-icon.png'),
        minWidth,
        minHeight,
        width: fit(state?.width, options.width, minWidth, workArea.width),
        height: fit(state?.height, options.height, minHeight, workArea.height),
    });

    if (state?.maximized === true) {
        window.once('ready-to-show', () => window.maximize());
    }

    window.once('close', () => {
        const { width, height } = window.getNormalBounds();
        const nextState = { width, height, maximized: window.isMaximized() };
        const temporaryPath = statePath + '.tmp';
        try {
            fs.mkdirSync(path.dirname(statePath), { recursive: true });
            fs.writeFileSync(temporaryPath, JSON.stringify(nextState));
            fs.renameSync(temporaryPath, statePath);
        } catch (error) {
            console.warn('Could not save the editor window size:', error.message);
        }
    });

    finishStartup(window);
    return window;
};
