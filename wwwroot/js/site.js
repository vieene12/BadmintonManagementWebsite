// ====== Cấu hình Logic Chatbot Aquar AI ======

// 1. Hàm đóng/mở cửa sổ Chatbot
function toggleChatbot() {
    const chatbotWindow = document.getElementById('chatbot-window');
    if (chatbotWindow) {
        chatbotWindow.classList.toggle('active');
        if (chatbotWindow.classList.contains('active')) {
            // Tự động cuộn xuống đáy khi mở hộp thoại
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

    // Hiển thị tin nhắn của Người dùng (User) lên màn hình ngay lập tức
    appendMessage(messageText, 'user');
    inputField.value = ''; // Xóa trống ô nhập liệu
    scrollToBottom();

    // Hiển thị hiệu ứng ba dấu chấm đang tải (Loading...) của Bot
    const loadingId = 'loading-' + Date.now();
    const loadingHtml = `<div class="chat-loading" id="${loadingId}">Aquar AI đang suy nghĩ<span class="animated-dots"></span></div>`;
    messagesContainer.insertAdjacentHTML('beforeend', loadingHtml);
    scrollToBottom();

    try {
        // Lấy Anti-Forgery Token từ Form bất kỳ trên trang (tránh lỗi 400/403 bảo mật của ASP.NET Core)
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        const headers = {
            'Content-Type': 'application/json'
        };
        if (tokenElement) {
            headers['RequestVerificationToken'] = tokenElement.value;
        }

        // Gọi API bất đồng bộ lên Controller của ASP.NET Core
        const response = await fetch('/Chatbot/Ask', {
            method: 'POST',
            headers: headers,
            body: JSON.stringify({ message: messageText })
        });

        // Xóa bỏ dòng hiệu ứng Loading sau khi nhận được phản hồi
        const loadingElement = document.getElementById(loadingId);
        if (loadingElement) loadingElement.remove();

        if (response.ok) {
            const data = await response.json();
            // Hiển thị câu trả lời của Bot (Ưu tiên thuộc tính reply từ DTO mới)
            appendMessage(data.reply || data.response || "Tôi đã nhận được tín hiệu nhưng cấu trúc phản hồi không xác định.", 'bot');
        } else {
            appendMessage("Rất tiếc, hệ thống kết nối AI đang bận. Bạn vui lòng thử lại sau ít phút!", 'bot');
        }
    } catch (error) {
        console.error("Lỗi kết nối API Chatbot:", error);

        // Xóa dòng Loading một lần duy nhất nếu xảy ra lỗi Crash mạng
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