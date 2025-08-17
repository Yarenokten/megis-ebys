// Bu dosya, tüm sayfalarda çalışarak kullanıcı yetkilerini ve bildirimleri yönetir.

document.addEventListener('DOMContentLoaded', () => {
    // 1. Kullanıcının giriş yapıp yapmadığını kontrol et
    const user = checkLogin();
    
    // 2. Eğer kullanıcı giriş yapmışsa, yetkilerine göre arayüzü düzenle
    if (user) {
        applyPermissions(user);
        setupNotifications(user); // Bildirim Fonksiyonu
    }
});

/**
 * localStorage'dan kullanıcı bilgilerini alır ve giriş kontrolü yapar.
 * @returns {object|null} Giriş yapmış kullanıcı nesnesi veya null.
 */
function checkLogin() {
    const user = JSON.parse(localStorage.getItem('kullanici'));
    const pathname = window.location.pathname;
    const isLoginPage = pathname.endsWith('login.html');

    if (!user && !isLoginPage) {
        window.location.href = 'login.html';
        return null;
    }
    
    if (user && isLoginPage) {
        window.location.href = 'anasayfa.html';
    }
    
    return user;
}

/**
 * Kullanıcının yetkisine göre arayüzü düzenler.
 * @param {object} user - Giriş yapmış kullanıcı nesnesi.
 */
function applyPermissions(user) {
    // Yetki Tanımları: 0 = Memur, 1 = Şef, 2 = Admin (Müdür)
    const isMemur = user.yetki === 0;

    // --- 1. Kenar Çubuğu (Sidebar) Linklerini Yetkiye Göre Gizle ---
    if (isMemur) {
        // Memur ise tüm yönetim linklerini gizle
        const managementLinks = document.querySelectorAll('#link-kullanici-yonetimi, #link-kurum-yonetimi, #link-ayarlar');
        managementLinks.forEach(link => {
            if(link) link.style.display = 'none';
        });
    }

    // --- 2. Sayfa Erişimi Kontrolü (Sessiz Yönlendirme) ---
    const pathname = window.location.pathname;
    const isManagementPage = pathname.endsWith('kurum-yonetimi.html') || 
                             pathname.endsWith('ayarlar.html') ||
                             pathname.endsWith('kullanici-yonetimi.html');

    if (isMemur && isManagementPage) {
        window.location.href = 'anasayfa.html';
        return;
    }

    // --- 3. Kullanıcı Bilgilerini Arayüze Yazdır ---
    updateUserInfo(user);

    // --- 4. Çıkış Yap Butonunu Aktif Et ---
    const logoutIcon = document.querySelector('i[data-lucide="log-out"]');
    if (logoutIcon) {
        const logoutButton = logoutIcon.parentElement;
        logoutButton.addEventListener('click', logout);
    }
}

/**
 * Kenar çubuğundaki kullanıcı bilgilerini günceller.
 * @param {object} user - Giriş yapmış kullanıcı nesnesi.
 */
function updateUserInfo(user) {
    const yetkiMap = { 0: "Memur", 1: "Şef", 2: "Admin" };
    const sidebarUsername = document.querySelector('#sidebar .font-semibold');
    const sidebarRole = document.querySelector('#sidebar .text-xs');

    if (sidebarUsername) sidebarUsername.textContent = user.adSoyad;
    if (sidebarRole) sidebarRole.textContent = yetkiMap[user.yetki] || 'Bilinmeyen Yetki';
}

/**
 * Bildirimleri yükler ve dropdown menüsünü oluşturur.
 * @param {object} user - Giriş yapmış kullanıcı nesnesi.
 */
async function setupNotifications(user) {
    const API_BASE_URL = "https://localhost:7281";
    const notificationButton = document.getElementById('notification-button');
    const notificationBadge = document.getElementById('notification-badge');
    const notificationDropdown = document.getElementById('notification-dropdown');
    const notificationList = document.getElementById('notification-list');

    if (!notificationButton || !notificationBadge || !notificationDropdown || !notificationList) return;

    try {
        const response = await fetch(`${API_BASE_URL}/api/Evraklar`);
        if (!response.ok) throw new Error('Bildirimler yüklenemedi.');
        const evraklar = await response.json();

        // DÜZELTME: Bildirimleri sadece ilgili kullanıcıya gösterecek şekilde filtrele
        const notifications = evraklar.filter(e => {
            const isUnread = e.durum !== 2 && e.durum !== 4; // Cevaplanmamış veya arşivlenmemiş
            if (!isUnread) return false;

            // Kural 1: Kurum içi bir yazışma
            if (e.dahiliMi) {
                // Doğrudan bana gönderilmişse
                if (e.aliciKullaniciId === user.id) return true;
                // Birimime gönderilmişse (ve benim bir birimim varsa)
                if (user.birimId && e.aliciBirimId === user.birimId) return true;
            }
            // Kural 2: Dışarıdan gelen bir evrak
            else if (e.yonu === 0) { 
                // Benim birimime havale edilmişse (ve benim bir birimim varsa)
                if (user.birimId && e.sorumluBirimId === user.birimId) return true;
            }
            
            return false; // Diğer durumlar bildirim değildir
        });

        if (notifications.length > 0) {
            notificationBadge.textContent = notifications.length;
            notificationBadge.classList.remove('hidden');
        } else {
            notificationBadge.classList.add('hidden');
        }

        notificationList.innerHTML = '';
        if (notifications.length === 0) {
            notificationList.innerHTML = '<p class="px-4 py-2 text-sm text-gray-500">Yeni bildiriminiz yok.</p>';
        } else {
            notifications.forEach(evrak => {
                const gonderen = evrak.dahiliMi ? (evrak.gonderenKullanici ? evrak.gonderenKullanici.adSoyad : 'Bilinmeyen') : evrak.ilgiliKurum;
                const listItem = document.createElement('a');
                listItem.href = `evrak-detay.html?id=${evrak.id}`;
                listItem.className = 'block px-4 py-3 text-sm text-gray-700 hover:bg-gray-100 border-b';
                listItem.innerHTML = `
                    <p class="font-bold">${gonderen}</p>
                    <p class="truncate">${evrak.konu}</p>
                    <p class="text-xs text-gray-500 mt-1">${new Date(evrak.tarih).toLocaleDateString('tr-TR')}</p>
                `;
                notificationList.appendChild(listItem);
            });
        }

        notificationButton.addEventListener('click', (e) => {
            e.stopPropagation();
            notificationDropdown.classList.toggle('hidden');
        });

        document.addEventListener('click', () => {
            if (!notificationDropdown.classList.contains('hidden')) {
                notificationDropdown.classList.add('hidden');
            }
        });

    } catch (error) {
        console.error("Bildirim hatası:", error);
        notificationBadge.classList.add('hidden');
    }
}

/**
 * Kullanıcıyı sistemden çıkarır.
 */
function logout() {
    if (confirm("Sistemden çıkış yapmak istediğinizden emin misiniz?")) {
        localStorage.removeItem('kullanici');
        window.location.href = 'login.html';
    }
}
