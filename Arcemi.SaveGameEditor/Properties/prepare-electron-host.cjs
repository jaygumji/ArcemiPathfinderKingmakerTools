const fs = require('node:fs');
const path = require('node:path');

const [packagePath, upstreamMainPath, outputMainPath] = process.argv.slice(2);
const upstreamHost = path.dirname(upstreamMainPath);
const outputHost = path.dirname(outputMainPath);
const manifest = JSON.parse(fs.readFileSync(packagePath, 'utf8').replace(/^\uFEFF/, ''));

// This editor has no custom Electron host hook.
delete manifest.dependencies['electron-host-hook'];
manifest.dependencies['image-size'] = '2.1.0';

const upstreamMain = fs.readFileSync(upstreamMainPath, 'utf8');
const oldImport = "const { imageSize } = require('image-size');";
const oldCall = /    imageSize\(imageFile, \(error, dimensions\) => \{\r?\n        if \(error\) \{\r?\n            console\.error\(`load splashscreen error:`, error\);\r?\n            throw new Error\(error\.message\);\r?\n        \}\r?\n\r?\n        startWindow\(dimensions\.width, dimensions\.height\);\r?\n    \}\);/;

if (!upstreamMain.includes(oldImport) || !oldCall.test(upstreamMain)) {
    throw new Error('Electron.NET changed its splash screen code. Review the image-size compatibility patch.');
}

const backendStart = 'apiProcess = cProcess(binFilePath, parameters, options);';
if (!upstreamMain.includes(backendStart)) {
    throw new Error('Electron.NET changed how it starts the editor backend. Review error reporting.');
}

const bridgeError = "console.error('Electron Socket: ' + error.message);";
const bridgeListening = 'const addr = server.address();';
const bridgeConnected = "console.info('Electron Socket: startup complete.');";
for (const anchor of [bridgeError, bridgeListening, bridgeConnected]) {
    if (!upstreamMain.includes(anchor)) {
        throw new Error('Electron.NET changed its bridge startup code. Review startup diagnostics.');
    }
}

const main = upstreamMain
    .replace(bridgeError, bridgeError + "\n        require('./custom_main').bridgeFailed(error);")
    .replace(bridgeListening, bridgeListening + "\n        require('./custom_main').bridgeListening(addr.address, addr.port);")
    .replace(bridgeConnected, bridgeConnected + "\n        require('./custom_main').bridgeConnected();")
    .replaceAll(backendStart, backendStart + "\n        require('./custom_main').watchBackend(apiProcess);")
    .replace(oldImport, "const { imageSizeFromFile } = require('image-size/fromFile');")
    .replace(oldCall, [
        '    imageSizeFromFile(imageFile).then(({ width, height }) => {',
        '        startWindow(width, height);',
        '    }).catch((error) => {',
        '        console.error(`load splashscreen error:`, error);',
        '        throw error;',
        '    });',
    ].join(upstreamMain.includes('\r\n') ? '\r\n' : '\n'));

fs.writeFileSync(packagePath, JSON.stringify(manifest, null, 2) + '\n');
writeHostScript('main.js', main);

const browserWindowsPath = 'api/browserWindows.js';
const browserWindows = fs.readFileSync(path.join(upstreamHost, browserWindowsPath), 'utf8');
const blazorOption = 'delete options.isRunningBlazor;';
const createWindow = 'window = new electron_1.BrowserWindow(options);';
if (!browserWindows.includes(blazorOption) || !browserWindows.includes(createWindow)) {
    throw new Error('Electron.NET changed how it creates Blazor windows. Review editor window state integration.');
}

writeHostScript(browserWindowsPath, browserWindows
    .replace(blazorOption, 'const { isRunningBlazor, ...windowOptions } = options;')
    .replace(createWindow, "window = isRunningBlazor ? require('../editor-window-state')(windowOptions) : new electron_1.BrowserWindow(windowOptions);"));

function writeHostScript(relativePath, content) {
    const outputPath = path.join(outputHost, relativePath);
    if (!fs.existsSync(outputPath) || fs.readFileSync(outputPath, 'utf8') !== content) {
        fs.mkdirSync(path.dirname(outputPath), { recursive: true });
        fs.writeFileSync(outputPath, content);
    }
}
