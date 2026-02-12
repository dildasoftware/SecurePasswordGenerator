// Clipboard (Pano) Yönetim Sistemi
// Panoya kopyalama ve otomatik temizleme

let autoClearTimeout = null;

/**
 * Metni panoya kopyalar
 */
window.copyToClipboard = async function (text) {
    try {
        // Modern Clipboard API kullan
        if (navigator.clipboard && navigator.clipboard.writeText) {
            await navigator.clipboard.writeText(text);
            console.log('Text copied to clipboard');
            return true;
        }
        // Fallback: Eski yöntem
        else {
            return fallbackCopyToClipboard(text);
        }
    } catch (error) {
        console.error('Error copying to clipboard:', error);
        // Fallback dene
        return fallbackCopyToClipboard(text);
    }
};

/**
 * Eski tarayıcılar için fallback kopyalama yöntemi
 */
function fallbackCopyToClipboard(text) {
    try {
        // Geçici bir textarea oluştur
        const textArea = document.createElement('textarea');
        textArea.value = text;

        // Görünmez yap
        textArea.style.position = 'fixed';
        textArea.style.top = '-9999px';
        textArea.style.left = '-9999px';
        textArea.style.opacity = '0';

        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        // Kopyalama komutunu çalıştır
        const successful = document.execCommand('copy');
        document.body.removeChild(textArea);

        if (successful) {
            console.log('Text copied using fallback method');
            return true;
        } else {
            console.error('Fallback copy failed');
            return false;
        }
    } catch (error) {
        console.error('Fallback copy error:', error);
        return false;
    }
}

/**
 * Panoyu temizler
 */
window.clearClipboard = async function () {
    try {
        if (navigator.clipboard && navigator.clipboard.writeText) {
            await navigator.clipboard.writeText('');
            console.log('Clipboard cleared');
            return true;
        } else {
            // Fallback: Boş string kopyala
            return fallbackCopyToClipboard('');
        }
    } catch (error) {
        console.error('Error clearing clipboard:', error);
        return false;
    }
};

/**
 * Metni kopyalar ve belirtilen süre sonra panoyu temizler
 */
window.copyWithAutoClear = async function (text, clearAfterSeconds = 30) {
    try {
        // Önce kopyala
        const copied = await window.copyToClipboard(text);

        if (!copied) {
            return false;
        }

        // Önceki timeout varsa iptal et
        if (autoClearTimeout) {
            clearTimeout(autoClearTimeout);
        }

        // Yeni timeout ayarla
        autoClearTimeout = setTimeout(async () => {
            await window.clearClipboard();
            console.log(`Clipboard auto-cleared after ${clearAfterSeconds} seconds`);

            // Kullanıcıya bildirim göster (opsiyonel)
            if (window.showToast) {
                window.showToast('Pano temizlendi', 'info');
            }
        }, clearAfterSeconds * 1000);

        console.log(`Clipboard will be cleared in ${clearAfterSeconds} seconds`);
        return true;

    } catch (error) {
        console.error('Error in copyWithAutoClear:', error);
        return false;
    }
};

/**
 * Otomatik temizleme işlemini iptal eder
 */
window.cancelAutoClear = function () {
    if (autoClearTimeout) {
        clearTimeout(autoClearTimeout);
        autoClearTimeout = null;
        console.log('Auto-clear cancelled');
        return true;
    }
    return false;
};

/**
 * Pano izni kontrolü
 */
window.checkClipboardPermission = async function () {
    try {
        if (navigator.permissions && navigator.permissions.query) {
            const result = await navigator.permissions.query({ name: 'clipboard-write' });
            console.log('Clipboard permission:', result.state);
            return result.state === 'granted' || result.state === 'prompt';
        }
        // Permissions API yoksa, varsayılan olarak true döndür
        return true;
    } catch (error) {
        console.warn('Clipboard permission check not supported:', error);
        return true;
    }
};

/**
 * Panodan okuma (şifre gücü test etmek için kullanılabilir)
 */
window.readFromClipboard = async function () {
    try {
        if (navigator.clipboard && navigator.clipboard.readText) {
            const text = await navigator.clipboard.readText();
            console.log('Text read from clipboard');
            return text;
        } else {
            console.warn('Clipboard read not supported');
            return null;
        }
    } catch (error) {
        console.error('Error reading from clipboard:', error);
        return null;
    }
};
