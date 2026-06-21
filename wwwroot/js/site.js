// ====== Cấu hình Logic Layout Admin & Customer ======

// 1. Mobile Sidebar Navigation Drawer toggle
function toggleSidebar() {
    const sidebar = document.getElementById('adminSidebar');
    const overlay = document.getElementById('sidebarOverlay');
    if (sidebar && overlay) {
        sidebar.classList.toggle('open');
        overlay.classList.toggle('show');
    }
}

// 2. Tab switching logic for Admin & Staff Views
document.addEventListener('DOMContentLoaded', function() {
    const sidebarLinks = document.querySelectorAll('.admin-sidebar-link');
    sidebarLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetTabId = this.getAttribute('data-tab');
            if (!targetTabId) return;

            // Update active state on links
            sidebarLinks.forEach(l => l.classList.remove('active'));
            this.classList.add('active');

            // Update active state on tab views
            const tabContents = document.querySelectorAll('.admin-tab-content');
            tabContents.forEach(tab => tab.classList.remove('active'));
            
            const targetTab = document.getElementById(targetTabId);
            if (targetTab) {
                targetTab.classList.add('active');
            }

            // Close sidebar on mobile
            const sidebar = document.getElementById('adminSidebar');
            if (sidebar && sidebar.classList.contains('open')) {
                toggleSidebar();
            }
        });
    });

    // Check URL parameters for customer view redirects on load
    const urlParams = new URLSearchParams(window.location.search);
    const viewParam = urlParams.get('view');
    if (viewParam === 'order') {
        setTimeout(() => showCustomerView('order'), 100);
    } else if (viewParam === 'matchmaking') {
        setTimeout(() => showCustomerView('matchmaking'), 100);
    }

    // Intercept Logout clicks to safely clear session states
    document.addEventListener('click', function(e) {
        const logoutLink = e.target.closest('a[href*="Logout"]');
        if (logoutLink) {
            localStorage.removeItem('token');
            localStorage.removeItem('jwtToken');
            localStorage.removeItem('authToken');
            localStorage.removeItem('user');
            localStorage.removeItem('role');
            localStorage.removeItem('search_history');
            sessionStorage.clear();
            console.log("Cleared session credentials/tokens on Logout. Preserved local images/themes.");
        }
    });

    // Animate Manager Weekly Revenue SVG Chart
    const path = document.querySelector('.chart-line-path');
    if (path) {
        const length = path.getTotalLength();
        path.style.transition = 'none';
        path.style.strokeDasharray = length;
        path.style.strokeDashoffset = length;
        path.getBoundingClientRect(); // trigger reflow
        path.style.transition = 'stroke-dashoffset 1.8s ease-in-out';
        path.style.strokeDashoffset = '0';
    }
});

// Centralized Customer View switching logic
function showCustomerView(viewName) {
    const mainView = document.getElementById('customer-main-view');
    const orderView = document.getElementById('customer-order-view');
    const matchmakingView = document.getElementById('customer-matchmaking-view');

    const btnOrderDesktop = document.getElementById('btn-toggle-order-desktop');
    const btnOrderMobile = document.getElementById('btn-toggle-order-mobile');
    const btnMatchDesktop = document.getElementById('btn-toggle-matching-desktop');
    const btnMatchMobile = document.getElementById('btn-toggle-matching-mobile');

    if (mainView) mainView.style.display = viewName === 'main' ? 'block' : 'none';
    if (orderView) orderView.style.display = viewName === 'order' ? 'block' : 'none';
    if (matchmakingView) matchmakingView.style.display = viewName === 'matchmaking' ? 'block' : 'none';

    // Update Order buttons text
    if (viewName === 'order') {
        if (btnOrderDesktop) {
            btnOrderDesktop.innerHTML = '<i class="bi bi-house-door-fill me-2"></i> Về trang chủ';
            btnOrderDesktop.classList.add('active');
        }
        if (btnOrderMobile) {
            btnOrderMobile.innerHTML = '<i class="bi bi-house-door-fill"></i>';
            btnOrderMobile.classList.add('active');
        }
    } else {
        if (btnOrderDesktop) {
            btnOrderDesktop.innerHTML = '<i class="bi bi-cart-fill me-2"></i> Gọi nước & thuê vợt';
            btnOrderDesktop.classList.remove('active');
        }
        if (btnOrderMobile) {
            btnOrderMobile.innerHTML = '<i class="bi bi-cart-fill"></i>';
            btnOrderMobile.classList.remove('active');
        }
    }

    // Update Matchmaking buttons text
    if (viewName === 'matchmaking') {
        if (btnMatchDesktop) {
            btnMatchDesktop.innerHTML = '<i class="bi bi-house-door-fill me-2"></i> Về trang chủ';
            btnMatchDesktop.classList.add('active');
        }
        if (btnMatchMobile) {
            btnMatchMobile.innerHTML = '<i class="bi bi-house-door-fill"></i>';
            btnMatchMobile.classList.add('active');
        }
    } else {
        if (btnMatchDesktop) {
            btnMatchDesktop.innerHTML = '<i class="bi bi-people-fill me-2"></i> Bắt cặp ghép sân';
            btnMatchDesktop.classList.remove('active');
        }
        if (btnMatchMobile) {
            btnMatchMobile.innerHTML = '<i class="bi bi-people-fill"></i>';
            btnMatchMobile.classList.remove('active');
        }
    }

    // Sync URL parameter
    const url = new URL(window.location);
    if (viewName === 'main') {
        url.searchParams.delete('view');
    } else {
        url.searchParams.set('view', viewName);
    }
    window.history.pushState({}, '', url);
}

function toggleCustomerOrderView() {
    const orderView = document.getElementById('customer-order-view');
    if (orderView && (orderView.style.display === 'block')) {
        showCustomerView('main');
    } else {
        showCustomerView('order');
    }
}

function toggleCustomerMatchmakingView() {
    const matchmakingView = document.getElementById('customer-matchmaking-view');
    if (matchmakingView && (matchmakingView.style.display === 'block')) {
        showCustomerView('main');
    } else {
        showCustomerView('matchmaking');
    }
}

// Media Highlight Floating Player control
function closeFloatingWidget(e) {
    if (e) {
        e.preventDefault();
        e.stopPropagation();
    }
    const widget = document.getElementById('floatingMatchHighlightWidget');
    if (widget) {
        widget.classList.remove('show');
    }
}




// ====== Cấu hình Logic Chatbot Aquar AI ======

// 1. Hàm đóng/mở cửa sổ Chatbot
function toggleChatbot() {
    const chatbotWindow = document.getElementById('chatbot-window');
    if (chatbotWindow) {
        chatbotWindow.classList.toggle('active');
        if (chatbotWindow.classList.contains('active')) {
            scrollToBottom();
        }
    }
}

// 2. Hàm bắt sự kiện nhấn nút Enter khi nhập tin nhắn
function handleChatKeyPress(event) {
    if (event.key === 'Enter') {
        sendChatMessage();
    }
}

// 3. Hàm gửi tin nhắn chính (Xử lý bất đồng bộ gọi về API của C#)
async function sendChatMessage() {
    const inputField = document.getElementById('chat-input');
    const messagesContainer = document.getElementById('chat-messages');

    if (!inputField || !messagesContainer) return;

    const messageText = inputField.value.trim();
    if (messageText === '') return; // Không gửi tin nhắn rỗng

    appendMessage(messageText, 'user');
    inputField.value = ''; // Xóa trống ô nhập liệu
    scrollToBottom();

    const loadingId = 'loading-' + Date.now();
    const loadingHtml = `<div class="chat-loading" id="${loadingId}">Aquar AI đang suy nghĩ<span class="animated-dots"></span></div>`;
    messagesContainer.insertAdjacentHTML('beforeend', loadingHtml);
    scrollToBottom();

    try {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        const headers = {
            'Content-Type': 'application/json'
        };
        if (tokenElement) {
            headers['RequestVerificationToken'] = tokenElement.value;
        }

        const response = await fetch('/Chatbot/Ask', {
            method: 'POST',
            headers: headers,
            body: JSON.stringify({ message: messageText })
        });

        const loadingElement = document.getElementById(loadingId);
        if (loadingElement) loadingElement.remove();

        if (response.ok) {
            const data = await response.json();
            appendMessage(data.reply || data.response || "Tôi đã nhận được tín hiệu nhưng cấu trúc phản hồi không xác định.", 'bot');
        } else {
            appendMessage("Rất tiếc, hệ thống kết nối AI đang bận. Bạn vui lòng thử lại sau ít phút!", 'bot');
        }
    } catch (error) {
        console.error("Lỗi kết nối API Chatbot:", error);

        const loadingElement = document.getElementById(loadingId);
        if (loadingElement) loadingElement.remove();

        appendMessage("Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại ChatbotController hoặc kết nối mạng!", 'bot');
    }

    scrollToBottom();
}

// 4. Hàm phụ trợ: Thêm thẻ HTML chứa tin nhắn vào khung chat
function appendMessage(text, sender) {
    const messagesContainer = document.getElementById('chat-messages');
    if (!messagesContainer) return;

    const messageHtml = `<div class="chat-message ${sender}">${text}</div>`;
    messagesContainer.insertAdjacentHTML('beforeend', messageHtml);
}

// 5. Hàm phụ trợ: Luôn cuộn thanh scroll xuống tin nhắn mới nhất
function scrollToBottom() {
    const messagesContainer = document.getElementById('chat-messages');
    if (messagesContainer) {
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }
}