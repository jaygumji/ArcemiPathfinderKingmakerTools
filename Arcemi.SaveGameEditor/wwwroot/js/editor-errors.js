(() => {
    const dialog = document.getElementById('editor-error-dialog');
    const text = document.getElementById('editor-error-report');
    const message = document.getElementById('editor-error-message');
    const close = document.getElementById('editor-error-close');
    const status = document.getElementById('editor-error-status');
    const pending = [];
    let reloadRequired = false;

    function show(details, requiresReload) {
        if (dialog.open) {
            if (reloadRequired) return;
            if (!requiresReload) {
                pending.push(details);
                return;
            }
        }

        reloadRequired = requiresReload;
        if (requiresReload) pending.length = 0;
        const version = document.documentElement.dataset.editorVersion;
        const platform = navigator.userAgentData?.platform || navigator.platform;
        text.value = [
            `Arcemi Save Game Editor ${version}`,
            `Platform: ${platform}`,
            `UTC: ${new Date().toISOString()}`,
            `Page: ${location.pathname}`,
            '',
            String(details).replace(/([?&]token=|\/electronAuthToken=)[^\s&]+/gi, '$1[redacted]'),
        ].join('\n');

        message.textContent = requiresReload
            ? 'The editor needs to reload. Copy the report first if you want to share it. Unsaved edits may be lost.'
            : 'The operation failed. You can close this report and continue using the editor.';
        close.textContent = requiresReload ? 'Reload editor' : 'Close';
        status.textContent = '';
        if (!dialog.open) dialog.showModal();
        text.scrollTop = 0;
    }

    document.getElementById('editor-error-copy').addEventListener('click', async () => {
        try {
            await navigator.clipboard.writeText(text.value);
            status.textContent = 'Report copied.';
        } catch {
            text.select();
            status.textContent = document.execCommand('copy') ? 'Report copied.' : 'Report selected. Use your copy shortcut.';
        }
    });

    close.addEventListener('click', () => {
        if (reloadRequired) location.reload();
        else dialog.close();
    });
    dialog.addEventListener('cancel', event => {
        if (reloadRequired) event.preventDefault();
    });
    dialog.addEventListener('close', () => {
        if (pending.length) show(pending.shift(), false);
    });

    window.arcemiErrors = { showRecoverable: details => show(details, false) };
    window.addEventListener('error', event => {
        if (event.error || event.message) show(event.error?.stack || event.message, true);
        else if (event.target instanceof HTMLScriptElement && event.target.src.includes('blazor.server.js')) {
            show('The Blazor client script could not load.', true);
        }
    }, true);
    window.addEventListener('unhandledrejection', event => {
        show(event.reason?.stack || event.reason || 'An asynchronous editor operation failed.', true);
    });

    const circuitBanner = document.getElementById('blazor-error-ui');
    function checkFatalReport() {
        const report = document.querySelector('[data-editor-fatal-report]');
        if (report) show(report.dataset.editorFatalReport, true);
        else if (circuitBanner?.style.display === 'block') {
            show('The Blazor session stopped responding before it could provide an exception.', true);
        }
    }

    const observer = new MutationObserver(checkFatalReport);
    observer.observe(document.body, { childList: true, subtree: true });
    if (circuitBanner) observer.observe(circuitBanner, { attributes: true, attributeFilter: ['style'] });
    checkFatalReport();
})();
