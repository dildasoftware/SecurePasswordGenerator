window.deferredPrompt = null;

window.addEventListener('beforeinstallprompt', (e) => {
    // Otomatik çıkan mini-bar'ı engelle
    e.preventDefault();
    // Eventi sakla, butona basınca kullanacağız
    window.deferredPrompt = e;

    // Blazor'a haber ver: "Yüklenebilirim, butonu göster!"
    // DotNet referansı henüz hazır olmayabilir, global bir flag set edelim veya event fırlatalım
    // Ancak en kolayı, Blazor'dan bu durumu kontrol etmek.
    console.log("PWA installable event captured!");

    // UI'ı güncellemek için event dispatch et
    window.dispatchEvent(new Event('pwa-installable'));
});

window.installPwa = async () => {
    const promptEvent = window.deferredPrompt;
    if (!promptEvent) {
        return;
    }
    // Yükleme penceresini aç
    promptEvent.prompt();
    // Kullanıcının cevabını bekle
    const result = await promptEvent.userChoice;
    console.log('User choice:', result);

    window.deferredPrompt = null;
};

window.isPwaInstallable = () => {
    return window.deferredPrompt !== null;
};
