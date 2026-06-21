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
    // Khởi tạo và áp dụng bảo vệ Route Guard/Trạng thái đăng nhập trên Frontend
    initializeFrontendAuth();

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

// ====== HỖ TRỢ ĐIỀU HƯỚNG VÀ KIỂM SOÁT TRUY CẬP TRÊN FRONTEND (RBAC) ======

// Hiển thị màn hình Loading chuyển trang mượt mà
function showLoadingScreen(callback) {
    let loader = document.getElementById('frontend-loader');
    if (!loader) {
        loader = document.createElement('div');
        loader.id = 'frontend-loader';
        loader.innerHTML = `
            <div class="loader-content">
                <div class="spinner-border text-success" role="status" style="width: 3rem; height: 3rem; margin-bottom: 15px;"></div>
                <div style="font-weight: 600; font-size: 1.1rem; letter-spacing: 0.5px;">Đang xác thực quyền truy cập...</div>
            </div>
        `;
        document.body.appendChild(loader);
    }
    loader.classList.add('show');
    setTimeout(() => {
        if (callback) callback();
    }, 600); // Tạo độ trễ chuyển tiếp 600ms mượt mà
}

// Hàm chính khởi tạo xác thực và bảo vệ tuyến đường phía Frontend
function initializeFrontendAuth() {
    const config = window.authConfig || { isAuthenticated: false, role: '', username: 'Khách', position: 'Thành viên' };
    
    // 1. Đồng bộ hóa Session từ Server xuống LocalStorage
    if (config.isAuthenticated && config.role) {
        localStorage.setItem('role', config.role);
        localStorage.setItem('token', 'mock_jwt_token_for_' + config.role);
        localStorage.setItem('user', JSON.stringify({
            username: config.username,
            role: config.role,
            position: config.position,
            staffCode: config.staffCode
        }));
    } else {
        // Nếu Server báo chưa đăng nhập, kiểm tra xem Frontend có session giả lập trước đó không
        const hasLocalToken = !!localStorage.getItem('token');
        if (!hasLocalToken) {
            localStorage.removeItem('role');
            localStorage.removeItem('user');
            localStorage.removeItem('token');
        }
    }

    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');

    // 2. Thiết lập Giao diện Khách (Guest View) khi chưa đăng nhập
    const isGuest = !token;
    if (isGuest) {
        document.documentElement.classList.add('guest-view');
        
        // Ẩn các khu vực cá nhân trên trang chủ của Customer
        const activeSession = document.getElementById('active-session-section');
        if (activeSession) activeSession.style.display = 'none';

        const myBookings = document.getElementById('my-bookings-section');
        if (myBookings) myBookings.style.display = 'none';

        // Thay đổi nút Đăng xuất trên Header thành Đăng nhập / Đăng ký
        const headerActionContainer = document.querySelector('.navbar-collapse .d-flex.align-items-center.gap-3');
        if (headerActionContainer) {
            headerActionContainer.innerHTML = `
                <a class="btn btn-success btn-sm px-4 fw-bold" href="/Account/Login" id="btn-login-header" style="border-radius: 6px; background: linear-gradient(135deg, #10b981 0%, #059669 100%); border: none; box-shadow: 0 4px 10px rgba(16, 185, 129, 0.25);">
                    <i class="bi bi-box-arrow-in-right me-2"></i> Đăng nhập / Đăng ký
                </a>
            `;
        }
    }

    // 3. Auth Guard - Chặn các hành động yêu cầu quyền khi ở chế độ Khách (Guest)
    document.addEventListener('click', function(e) {
        if (!isGuest) return; // Đã đăng nhập -> Cho qua bình thường

        // Xác định các hành động của khách cần chặn
        const targetEmptyCell = e.target.closest('.timetable-table td.empty');
        const targetMatchingBtn = e.target.closest('#btn-toggle-matching-desktop, #btn-toggle-matching-mobile, [href*="matchmaking"]');
        const targetOrderBtn = e.target.closest('#btn-toggle-order-desktop, #btn-toggle-order-mobile, [href*="order"]');
        const targetVideoAction = e.target.closest('[onclick*="simulateVideoPlay"], [onclick*="alert"], [onclick*="openRegisterMatchmakingModal"]');
        const targetChatInput = e.target.closest('#chat-input, .chat-send-btn, .chat-chip');

        if (targetEmptyCell || targetMatchingBtn || targetOrderBtn || targetVideoAction || targetChatInput) {
            e.preventDefault();
            e.stopPropagation();

            showLoadingScreen(() => {
                const currentPath = window.location.pathname + window.location.search;
                window.location.href = `/Account/Login?returnUrl=${encodeURIComponent(currentPath)}`;
            });
        }
    }, true);

    // 4. Đánh chặn việc nộp Form Đăng nhập để lưu trạng thái cục bộ ngay lập tức (Post-Login Handling)
    document.addEventListener('submit', function(e) {
        const loginForm = e.target.closest('form[action*="Login"]');
        if (loginForm) {
            const usernameInput = loginForm.querySelector('input[name="username"]');
            if (usernameInput) {
                const username = usernameInput.value.trim().toLowerCase();
                let mockRole = '1'; // Mặc định là Khách Hàng
                let position = 'Khách Hàng';
                
                // Dự đoán vai trò trước khi Backend chuyển hướng
                if (username === 'manager' || username.includes('admin')) {
                    mockRole = '3';
                    position = 'Quản lý';
                } else if (username === 'staff' || username.includes('receptionist') || username === 'nv001') {
                    mockRole = '2';
                    position = 'Lễ tân';
                }

                localStorage.setItem('role', mockRole);
                localStorage.setItem('token', 'mock_jwt_token_' + Date.now());
                localStorage.setItem('user', JSON.stringify({
                    username: usernameInput.value,
                    role: mockRole,
                    position: position
                }));
            }
        }
    });
}

// ==========================================
// SCROLL & RANDOM ANIMATION OBSERVERS (CHIBI SVG)
// ==========================================
document.addEventListener("DOMContentLoaded", function() {
    // 1. Scroll Observer (Fade-in-up / Bounce-in when scrolling)
    const observerOptions = {
        root: null,
        rootMargin: '0px',
        threshold: 0.1
    };

    const scrollObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('scroll-show');
                // observer.unobserve(entry.target); // Optional: if we want to animate only once
            } else {
                entry.target.classList.remove('scroll-show');
            }
        });
    }, observerOptions);

    const hiddenElements = document.querySelectorAll('.scroll-hidden');
    hiddenElements.forEach((el) => scrollObserver.observe(el));

    // 2. Random Floating/Shooting Effect for Shuttlecock
    const shuttleSticker = document.getElementById('chibi-shuttlecock-sticker');
    if (shuttleSticker) {
        setInterval(() => {
            // Trigger random shoot every 10-15s
            if (Math.random() > 0.5) {
                shuttleSticker.classList.add('random-shoot');
                setTimeout(() => {
                    shuttleSticker.classList.remove('random-shoot');
                }, 1500); // Wait for animation to finish before removing
            }
        }, 8000); // Checks every 8 seconds
    }
});

// ==========================================
// RIPPLE EFFECT FOR SYSTEM BUTTONS
// ==========================================
document.addEventListener("click", function (e) {
    try {
        // Tìm button cha gần nhất có class .sys-btn
        if (!e.target || typeof e.target.closest !== 'function') return;
        const btn = e.target.closest('.sys-btn');
        if (!btn || btn.disabled) return;

        // Lấy tọa độ click tương đối với button
        const rect = btn.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;

        // Tạo span chứa hiệu ứng gợn sóng
        const ripple = document.createElement("span");
        ripple.className = "sys-ripple";
        ripple.style.width = ripple.style.height = size + "px";
        ripple.style.left = x + "px";
        ripple.style.top = y + "px";

        // Xóa ripple cũ nếu người dùng click liên tục nhanh
        const existingRipple = btn.querySelector('.sys-ripple');
        if (existingRipple) {
            existingRipple.remove();
        }

        btn.appendChild(ripple);

        // Dọn dẹp DOM sau khi hiệu ứng kết thúc
        setTimeout(() => {
            if (ripple && ripple.parentNode) ripple.remove();
        }, 600);
    } catch (err) {
        console.error("Ripple effect error:", err);
    }
});