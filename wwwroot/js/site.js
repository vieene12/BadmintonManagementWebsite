// Chatbot Toggler
function toggleChatbot() {
    const chatWin = document.getElementById('chatbot-window');
    chatWin.classList.toggle('active');
}

// Handle Enter key in Chat input
function handleChatKeyPress(event) {
    if (event.key === 'Enter') {
        sendChatMessage();
    }
}

// Send Chat Message & Process Logic
function sendChatMessage() {
    const input = document.getElementById('chat-input');
    const text = input.value.trim();
    if (!text) return;

    // Append User message
    appendChatMessage(text, 'user');
    input.value = '';

    // Scroll to bottom
    const msgContainer = document.getElementById('chat-messages');
    msgContainer.scrollTop = msgContainer.scrollHeight;

    // Simulate thinking and answer
    setTimeout(() => {
        const isManager = document.getElementById('chatbot-context').innerText.includes('Quản trị');
        const reply = generateBotReply(text, isManager);
        appendChatMessage(reply, 'bot');
        msgContainer.scrollTop = msgContainer.scrollHeight;
    }, 600);
}

// Append message to Chat Area
function appendChatMessage(text, sender) {
    const msgContainer = document.getElementById('chat-messages');
    const msgDiv = document.createElement('div');
    msgDiv.classList.add('chat-message', sender);
    msgDiv.innerHTML = text;
    msgContainer.appendChild(msgDiv);
}

// Simple Rule-based Local Bot Brain
function generateBotReply(query, isManager) {
    const q = query.toLowerCase();

    if (isManager) {
        // Manager Admin Chatbot Portal Responses
        if (q.includes('doanh thu') || q.includes('tiền') || q.includes('báo cáo') || q.includes('báo cáo tài chính')) {
            return `<strong>[Trợ Lý Admin]</strong> Báo cáo tài chính ngày hôm nay:<br>
                    - Tổng doanh thu: <strong>4.850.000đ</strong><br>
                    - Tiền thuê sân: 3.500.000đ<br>
                    - Tiền dịch vụ nước uống/dụng cụ: 1.350.000đ<br>
                    - Trạng thái dòng tiền: 90% đã thanh toán hóa đơn.`;
        }
        if (q.includes('công suất') || q.includes('tần suất') || q.includes('sử dụng')) {
            return `<strong>[Trợ Lý Admin]</strong> Công suất khai thác sân đạt <strong>82.5%</strong>. Khung giờ cao điểm hôm nay (17:00 - 21:00) đã được lấp đầy 100% trên tất cả 4 sân con.`;
        }
        if (q.includes('nhân viên') || q.includes('lễ tân')) {
            return `<strong>[Trợ Lý Admin]</strong> Danh sách ca trực hôm nay:<br>
                    - Ca sáng (6h - 14h): Nguyễn Văn A (Lễ tân)<br>
                    - Ca chiều (14h - 22h): Lê Hoàng Nam (Lễ tân)<br>
                    Tất cả ca trực hoạt động bình thường, không ghi nhận sự cố bảo mật nào.`;
        }
        return `<strong>[Trợ Lý Admin]</strong> Tôi đã xác thực quyền Quản lý (Admin) của bạn. Tôi có thể hỗ trợ các lệnh: <em>"Xem doanh thu hôm nay"</em>, <em>"Công suất sử dụng sân"</em> hoặc <em>"Báo cáo nhân viên trực"</em>.`;
    } else {
        // Customer Chatbot Portal Responses
        if (q.includes('sân trống') || q.includes('lịch trống') || q.includes('đặt sân') || q.includes('giờ trống')) {
            return `<strong>[Trợ lý Khách hàng]</strong> Qua tra cứu thời gian thực:<br>
                    - Sân con 02 đang trống từ <strong>18:00 đến 20:00</strong> tối nay.<br>
                    - Sân con 04 trống từ <strong>20:00 đến 22:00</strong>.<br>
                    Bạn có thể nhấp chọn trực tiếp khung giờ trên lưới lịch sân của trang chủ để đặt ngay!`;
        }
        if (q.includes('phụ kiện') || q.includes('vợt') || q.includes('giày') || q.includes('áo') || q.includes('mua sắm')) {
            return `<strong>[Trợ lý Khách hàng]</strong> Nếu bạn cần tìm phụ kiện cầu lông:<br>
                    - Vợt tấn công: <strong>Yonex Astrox 88D Pro</strong> đang có liên kết giảm 10% ở mục mua sắm bên dưới.<br>
                    - Giày bám sân: <strong>Victor P9200</strong>.<br>
                    Bạn có thể kéo xuống phân hệ <strong>"Liên Kết Tiếp Thị"</strong> để xem trực tiếp và đặt hàng nhé!`;
        }
        if (q.includes('ghép') || q.includes('tìm bạn') || q.includes('đánh chung')) {
            return `<strong>[Trợ lý Khách hàng]</strong> Hiện tại đang có yêu cầu ghép sau phù hợp:<br>
                    - <strong>Nhóm số #102</strong>: Trình độ Trung bình, cần ghép thêm 2 người, chơi lúc 19:00 tại Sân 03.<br>
                    Bạn hãy nhấp nút <strong>"Tham gia nhóm"</strong> trong khung ghép sân trên giao diện để kết nối ngay.`;
        }
        return `<strong>[Trợ lý Khách hàng]</strong> Xin chào! Tôi có thể hỗ trợ bạn:<br>
                1. Tra cứu sân trống: gõ <em>"sân trống"</em><br>
                2. Tìm nhóm ghép bạn chơi: gõ <em>"tìm bạn đánh cùng"</em><br>
                3. Tư vấn mua sắm vợt/giày phù hợp: gõ <em>"vợt cầu lông nào tốt"</em>`;
    }
}
