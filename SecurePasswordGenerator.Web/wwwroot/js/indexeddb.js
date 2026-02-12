// IndexedDB Yönetim Sistemi
// Tarayıcıda şifre geçmişini saklar

const DB_NAME = 'SecurePasswordGeneratorDB';
const DB_VERSION = 1;
const STORE_NAME = 'passwords';

let db = null;

/**
 * Veritabanını başlatır
 */
window.initializeDb = async function () {
    return new Promise((resolve, reject) => {
        // IndexedDB açma isteği
        const request = indexedDB.open(DB_NAME, DB_VERSION);

        // Veritabanı ilk kez oluşturuluyorsa veya versiyon güncelleniyorsa
        request.onupgradeneeded = function (event) {
            db = event.target.result;

            // Object store (tablo) oluştur
            if (!db.objectStoreNames.contains(STORE_NAME)) {
                const objectStore = db.createObjectStore(STORE_NAME, { keyPath: 'id' });

                // İndeksler oluştur (hızlı arama için)
                objectStore.createIndex('createdAt', 'createdAt', { unique: false });
                objectStore.createIndex('isFavorite', 'isFavorite', { unique: false });
                objectStore.createIndex('strengthLevel', 'strengthLevel', { unique: false });

                console.log('IndexedDB object store created');
            }
        };

        request.onsuccess = function (event) {
            db = event.target.result;
            console.log('IndexedDB initialized successfully');
            resolve(true);
        };

        request.onerror = function (event) {
            console.error('IndexedDB error:', event.target.error);
            reject(event.target.error);
        };
    });
};

/**
 * Şifreyi veritabanına kaydeder
 */
window.savePassword = async function (password) {
    return new Promise((resolve, reject) => {
        if (!db) {
            reject('Database not initialized');
            return;
        }

        const transaction = db.transaction([STORE_NAME], 'readwrite');
        const objectStore = transaction.objectStore(STORE_NAME);
        const request = objectStore.add(password);

        request.onsuccess = function () {
            console.log('Password saved to IndexedDB');
            resolve(true);
        };

        request.onerror = function (event) {
            console.error('Error saving password:', event.target.error);
            reject(event.target.error);
        };
    });
};

/**
 * Tüm şifreleri getirir
 */
window.getAllPasswords = async function () {
    return new Promise((resolve, reject) => {
        if (!db) {
            reject('Database not initialized');
            return;
        }

        const transaction = db.transaction([STORE_NAME], 'readonly');
        const objectStore = transaction.objectStore(STORE_NAME);
        const request = objectStore.getAll();

        request.onsuccess = function (event) {
            const passwords = event.target.result;
            console.log(`Retrieved ${passwords.length} passwords from IndexedDB`);
            resolve(passwords);
        };

        request.onerror = function (event) {
            console.error('Error getting passwords:', event.target.error);
            reject(event.target.error);
        };
    });
};

/**
 * ID'ye göre şifre getirir
 */
window.getPasswordById = async function (id) {
    return new Promise((resolve, reject) => {
        if (!db) {
            reject('Database not initialized');
            return;
        }

        const transaction = db.transaction([STORE_NAME], 'readonly');
        const objectStore = transaction.objectStore(STORE_NAME);
        const request = objectStore.get(id);

        request.onsuccess = function (event) {
            resolve(event.target.result);
        };

        request.onerror = function (event) {
            console.error('Error getting password:', event.target.error);
            reject(event.target.error);
        };
    });
};

/**
 * Şifreyi günceller
 */
window.updatePassword = async function (password) {
    return new Promise((resolve, reject) => {
        if (!db) {
            reject('Database not initialized');
            return;
        }

        const transaction = db.transaction([STORE_NAME], 'readwrite');
        const objectStore = transaction.objectStore(STORE_NAME);
        const request = objectStore.put(password);

        request.onsuccess = function () {
            console.log('Password updated in IndexedDB');
            resolve(true);
        };

        request.onerror = function (event) {
            console.error('Error updating password:', event.target.error);
            reject(event.target.error);
        };
    });
};

/**
 * Şifreyi siler
 */
window.deletePassword = async function (id) {
    return new Promise((resolve, reject) => {
        if (!db) {
            reject('Database not initialized');
            return;
        }

        const transaction = db.transaction([STORE_NAME], 'readwrite');
        const objectStore = transaction.objectStore(STORE_NAME);
        const request = objectStore.delete(id);

        request.onsuccess = function () {
            console.log('Password deleted from IndexedDB');
            resolve(true);
        };

        request.onerror = function (event) {
            console.error('Error deleting password:', event.target.error);
            reject(event.target.error);
        };
    });
};

/**
 * Tüm şifreleri siler
 */
window.clearAllPasswords = async function () {
    return new Promise((resolve, reject) => {
        if (!db) {
            reject('Database not initialized');
            return;
        }

        const transaction = db.transaction([STORE_NAME], 'readwrite');
        const objectStore = transaction.objectStore(STORE_NAME);
        const request = objectStore.clear();

        request.onsuccess = function () {
            console.log('All passwords cleared from IndexedDB');
            resolve(true);
        };

        request.onerror = function (event) {
            console.error('Error clearing passwords:', event.target.error);
            reject(event.target.error);
        };
    });
};

/**
 * Veritabanını dışa aktarır (JSON)
 */
window.exportPasswords = async function () {
    try {
        const passwords = await window.getAllPasswords();
        return JSON.stringify(passwords, null, 2);
    } catch (error) {
        console.error('Error exporting passwords:', error);
        throw error;
    }
};

/**
 * Veritabanını içe aktarır (JSON)
 */
window.importPasswords = async function (jsonData) {
    return new Promise(async (resolve, reject) => {
        try {
            const passwords = JSON.parse(jsonData);

            if (!Array.isArray(passwords)) {
                reject('Invalid data format');
                return;
            }

            // Mevcut verileri temizle
            await window.clearAllPasswords();

            // Yeni verileri ekle
            const transaction = db.transaction([STORE_NAME], 'readwrite');
            const objectStore = transaction.objectStore(STORE_NAME);

            let successCount = 0;

            for (const password of passwords) {
                const request = objectStore.add(password);
                request.onsuccess = function () {
                    successCount++;
                };
            }

            transaction.oncomplete = function () {
                console.log(`Imported ${successCount} passwords`);
                resolve(successCount);
            };

            transaction.onerror = function (event) {
                console.error('Error importing passwords:', event.target.error);
                reject(event.target.error);
            };

        } catch (error) {
            console.error('Error parsing JSON:', error);
            reject(error);
        }
    });
};

/**
 * Dosya indirme (Export için)
 */
window.downloadFile = (filename, content, contentType) => {
    const blob = new Blob([content], { type: contentType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

// Sayfa yüklendiğinde veritabanını başlat
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        window.initializeDb().catch(console.error);
    });
} else {
    window.initializeDb().catch(console.error);
}
