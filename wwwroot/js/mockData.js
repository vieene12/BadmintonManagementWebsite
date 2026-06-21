/**
 * MOCK DATA - HỆ THỐNG QUẢN LÝ SÂN CẦU LÔNG (AQUARSMARTCOURT)
 * File: wwwroot/js/mockData.js
 * 
 * Bộ dữ liệu mẫu đầy đủ phục vụ cho phát triển giao diện độc lập (Frontend).
 * Hỗ trợ 3 định dạng viết khóa (camelCase, snake_case, PascalCase) để dễ dàng đồng bộ với cả EF Core C# và JavaScript.
 * Các mốc thời gian được tính động dựa theo ngày chạy thử nghiệm thực tế.
 */

// Hàm tiện ích tạo mốc thời gian động theo ngày hiện tại
const getTodayAt = (hours, minutes = 0) => {
    const d = new Date();
    d.setHours(hours, minutes, 0, 0);
    return d.toISOString();
};

const getRelativeDate = (offsetDays, hours = 12, minutes = 0) => {
    const d = new Date();
    d.setDate(d.getDate() + offsetDays);
    d.setHours(hours, minutes, 0, 0);
    return d.toISOString();
};

// ==========================================
// 1. DỮ LIỆU TÀI KHOẢN (USERS)
// ==========================================
export const users = [
    {
        userId: 1,
        user_id: 1,
        UserId: 1,
        username: "customer",
        Username: "customer",
        fullName: "Nguyễn Văn A",
        full_name: "Nguyễn Văn A",
        FullName: "Nguyễn Văn A",
        email: "customerA@gmail.com",
        Email: "customerA@gmail.com",
        phoneNumber: "0987654321",
        phone_number: "0987654321",
        PhoneNumber: "0987654321",
        role: 1,
        Role: 1, // 1: Customer, 2: Staff, 3: Manager
        position: "Khách Hàng",
        Position: "Khách Hàng",
        loyaltyPoints: 120,
        loyalty_points: 120,
        LoyaltyPoints: 120,
        isActive: true,
        is_active: true,
        IsActive: true
    },
    {
        userId: 2,
        user_id: 2,
        UserId: 2,
        username: "staff",
        Username: "staff",
        fullName: "Lê Hoàng Nam",
        full_name: "Lê Hoàng Nam",
        FullName: "Lê Hoàng Nam",
        email: "nam.lh@aquasmartcourt.com",
        Email: "nam.lh@aquasmartcourt.com",
        phoneNumber: "0912345678",
        phone_number: "0912345678",
        PhoneNumber: "0912345678",
        role: 2,
        Role: 2, // Staff
        staffCode: "NV001",
        staff_code: "NV001",
        StaffCode: "NV001",
        dateOfBirth: "1998-05-15T00:00:00.000Z",
        date_of_birth: "1998-05-15T00:00:00.000Z",
        DateOfBirth: "1998-05-15T00:00:00.000Z",
        position: "Lễ tân",
        Position: "Lễ tân",
        loyaltyPoints: 0,
        LoyaltyPoints: 0,
        isActive: true,
        IsActive: true
    },
    {
        userId: 3,
        user_id: 3,
        UserId: 3,
        username: "manager",
        Username: "manager",
        fullName: "Trần Thị B",
        full_name: "Trần Thị B",
        FullName: "Trần Thị B",
        email: "b.tt@aquasmartcourt.com",
        Email: "b.tt@aquasmartcourt.com",
        phoneNumber: "0966554433",
        phone_number: "0966554433",
        PhoneNumber: "0966554433",
        role: 3,
        Role: 3, // Manager
        staffCode: "AD001",
        staff_code: "AD001",
        StaffCode: "AD001",
        dateOfBirth: "1985-10-20T00:00:00.000Z",
        date_of_birth: "1985-10-20T00:00:00.000Z",
        DateOfBirth: "1985-10-20T00:00:00.000Z",
        position: "Quản lý",
        Position: "Quản lý",
        loyaltyPoints: 0,
        LoyaltyPoints: 0,
        isActive: true,
        IsActive: true
    },
    {
        userId: 4,
        user_id: 4,
        UserId: 4,
        username: "customer_vip",
        Username: "customer_vip",
        fullName: "Phạm Minh Hoàng",
        full_name: "Phạm Minh Hoàng",
        FullName: "Phạm Minh Hoàng",
        email: "hoang.pm@gmail.com",
        Email: "hoang.pm@gmail.com",
        phoneNumber: "0933445566",
        phone_number: "0933445566",
        PhoneNumber: "0933445566",
        role: 1,
        Role: 1,
        position: "Khách Hàng VIP",
        Position: "Khách Hàng VIP",
        loyaltyPoints: 850,
        LoyaltyPoints: 850,
        isActive: true,
        IsActive: true
    },
    {
        userId: 5,
        user_id: 5,
        UserId: 5,
        username: "staff2",
        Username: "staff2",
        fullName: "Nguyễn Thị Mai",
        full_name: "Nguyễn Thị Mai",
        FullName: "Nguyễn Thị Mai",
        email: "mai.nt@aquasmartcourt.com",
        Email: "mai.nt@aquasmartcourt.com",
        phoneNumber: "0922889900",
        phone_number: "0922889900",
        PhoneNumber: "0922889900",
        role: 2,
        Role: 2,
        staffCode: "NV002",
        StaffCode: "NV002",
        position: "Thu ngân",
        Position: "Thu ngân",
        loyaltyPoints: 0,
        isActive: true
    }
];

// ==========================================
// 2. DỮ LIỆU DANH MỤC (CATALOG: COURTS & SERVICES)
// ==========================================
export const courts = [
    {
        courtId: 1,
        court_id: 1,
        CourtId: 1,
        courtCode: "S01",
        court_code: "S01",
        CourtCode: "S01",
        courtName: "Sân Thường 01",
        court_name: "Sân Thường 01",
        CourtName: "Sân Thường 01",
        courtType: "Standard",
        court_type: "Standard",
        HourlyPrice: 80000,
        hourlyPrice: 80000,
        hourly_price: 80000,
        status: "InUse", // Available, InUse, Maintenance
        Status: "InUse",
        imageUrl: "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=600&q=80"
    },
    {
        courtId: 2,
        court_id: 2,
        CourtId: 2,
        courtCode: "S02",
        CourtCode: "S02",
        courtName: "Sân Thường 02",
        CourtName: "Sân Thường 02",
        courtType: "Standard",
        HourlyPrice: 80000,
        hourlyPrice: 80000,
        status: "Available",
        Status: "Available",
        imageUrl: "https://images.unsplash.com/photo-1554068865-24cecd4e34b8?auto=format&fit=crop&w=600&q=80"
    },
    {
        courtId: 3,
        court_id: 3,
        CourtId: 3,
        courtCode: "S03",
        CourtCode: "S03",
        courtName: "Sân Thường 03",
        CourtName: "Sân Thường 03",
        courtType: "Standard",
        HourlyPrice: 80000,
        hourlyPrice: 80000,
        status: "InUse",
        Status: "InUse",
        imageUrl: "https://images.unsplash.com/photo-1613918431208-67520e55478d?auto=format&fit=crop&w=600&q=80"
    },
    {
        courtId: 4,
        court_id: 4,
        CourtId: 4,
        courtCode: "S04",
        CourtCode: "S04",
        courtName: "Sân Thường 04",
        CourtName: "Sân Thường 04",
        courtType: "Standard",
        HourlyPrice: 80000,
        hourlyPrice: 80000,
        status: "Maintenance",
        Status: "Maintenance",
        imageUrl: "https://images.unsplash.com/photo-1521537634581-0dced2fee2e9?auto=format&fit=crop&w=600&q=80"
    },
    {
        courtId: 5,
        court_id: 5,
        CourtId: 5,
        courtCode: "VIP01",
        CourtCode: "VIP01",
        courtName: "Sân Premium VIP 01",
        CourtName: "Sân Premium VIP 01",
        courtType: "VIP",
        HourlyPrice: 120000,
        hourlyPrice: 120000,
        status: "Available",
        Status: "Available",
        imageUrl: "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=600&q=80"
    },
    {
        courtId: 6,
        court_id: 6,
        CourtId: 6,
        courtCode: "VIP02",
        CourtCode: "VIP02",
        courtName: "Sân Premium VIP 02",
        CourtName: "Sân Premium VIP 02",
        courtType: "VIP",
        HourlyPrice: 120000,
        hourlyPrice: 120000,
        status: "Available",
        Status: "Available",
        imageUrl: "https://images.unsplash.com/photo-1554068865-24cecd4e34b8?auto=format&fit=crop&w=600&q=80"
    }
];

export const services = [
    {
        serviceId: 1,
        service_id: 1,
        serviceItemId: 1,
        ServiceItemId: 1,
        itemName: "Nước suối Aquafina",
        item_name: "Nước suối Aquafina",
        ItemName: "Nước suối Aquafina",
        category: "F&B", // Nước uống / Đồ ăn
        Category: "Nước uống",
        unit: "Chai",
        Unit: "Chai",
        unitPrice: 15000,
        unit_price: 15000,
        UnitPrice: 15000,
        stock: 120,
        quantity_in_stock: 120,
        imageUrl: "https://images.unsplash.com/photo-1608885898957-a599fb1b467a?auto=format&fit=crop&w=600&q=80"
    },
    {
        serviceId: 2,
        service_id: 2,
        serviceItemId: 2,
        ServiceItemId: 2,
        itemName: "Nước bù khoáng Revive",
        ItemName: "Nước tăng lực Sting",
        category: "F&B",
        Category: "Nước uống",
        unit: "Chai",
        Unit: "Chai",
        unitPrice: 20000,
        UnitPrice: 20000,
        stock: 85,
        imageUrl: "https://images.unsplash.com/photo-1622543954782-75c75542c35f?auto=format&fit=crop&w=600&q=80"
    },
    {
        serviceId: 3,
        service_id: 3,
        serviceItemId: 3,
        ServiceItemId: 3,
        itemName: "Thuê vợt Yonex Astrox 88D",
        ItemName: "Thuê vợt Yonex chính hãng",
        category: "Equipment", // Thiết bị / Phụ kiện thuê
        Category: "Thuê vợt",
        unit: "Cặp/Giờ",
        Unit: "Cặp/Giờ",
        unitPrice: 50000,
        UnitPrice: 50000,
        stock: 10,
        imageUrl: "https://images.unsplash.com/photo-1617083266344-0b1a039755b6?auto=format&fit=crop&w=600&q=80"
    },
    {
        serviceId: 4,
        service_id: 4,
        serviceItemId: 4,
        ServiceItemId: 4,
        itemName: "Ống cầu lông Hải Yến",
        ItemName: "Ống cầu lông Hải Yến",
        category: "Equipment",
        Category: "Phụ kiện",
        unit: "Ống",
        Unit: "Ống",
        unitPrice: 250000,
        UnitPrice: 250000,
        stock: 30,
        imageUrl: "https://images.unsplash.com/photo-1611252199277-2bc46617a6a4?auto=format&fit=crop&w=600&q=80"
    },
    {
        serviceId: 5,
        service_id: 5,
        serviceItemId: 5,
        ServiceItemId: 5,
        itemName: "Thuê giày Victor chính hãng",
        ItemName: "Quấn cán vợt cao su",
        category: "Equipment",
        Category: "Phụ kiện",
        unit: "Đôi/Lượt",
        Unit: "Cái",
        unitPrice: 30000,
        UnitPrice: 30000,
        stock: 15,
        imageUrl: "https://images.unsplash.com/photo-1608231387042-66d1773070a5?auto=format&fit=crop&w=600&q=80"
    }
];

// ==========================================
// 3. DỮ LIỆU HOẠT ĐỘNG & GIAO DỊCH (OPERATIONS)
// ==========================================

// Bookings (Đặt sân)
export const bookings = [
    {
        bookingId: 1,
        booking_id: 1,
        BookingId: 1,
        courtId: 1,
        court_id: 1,
        CourtId: 1,
        userId: 1,
        user_id: 1,
        UserId: 1,
        customerName: "Nguyễn Văn A",
        customer_name: "Nguyễn Văn A",
        CustomerName: "Nguyễn Văn A",
        customerPhone: "0987654321",
        customer_phone: "0987654321",
        CustomerPhone: "0987654321",
        startTime: getTodayAt(17), // 17:00 ngày hôm nay
        start_time: getTodayAt(17),
        StartTime: getTodayAt(17),
        endTime: getTodayAt(19),   // 19:00 ngày hôm nay
        end_time: getTodayAt(19),
        EndTime: getTodayAt(19),
        status: "Confirmed", // Confirmed, Completed, Cancelled
        Status: "Confirmed",
        notes: "Cần thuê thêm 1 cặp vợt Yonex",
        Notes: "Cần thuê thêm 1 cặp vợt Yonex",
        totalPrice: 160000, // 2 giờ x 80.000 VNĐ
        total_price: 160000,
        TotalPrice: 160000,
        isPaid: false,
        paymentStatus: "Unpaid"
    },
    {
        bookingId: 2,
        booking_id: 2,
        BookingId: 2,
        courtId: 3,
        court_id: 3,
        CourtId: 3,
        userId: null,
        user_id: null,
        UserId: null,
        customerName: "Nhóm Trần Minh",
        CustomerName: "Nhóm Trần Minh",
        customerPhone: "0912345678",
        CustomerPhone: "0912345678",
        startTime: getTodayAt(18),
        endTime: getTodayAt(20),
        status: "Confirmed",
        notes: "Khách lẻ ghép nhóm tìm đối thủ",
        totalPrice: 160000,
        isPaid: false,
        paymentStatus: "Unpaid"
    },
    {
        bookingId: 3,
        booking_id: 3,
        BookingId: 3,
        courtId: 1,
        CourtId: 1,
        userId: null,
        customerName: "Khánh Lê",
        customerPhone: "0900112233",
        startTime: getTodayAt(19, 30),
        endTime: getTodayAt(21, 30),
        status: "Confirmed",
        notes: "Ghi hình lại trận đấu",
        totalPrice: 160000,
        isPaid: true,
        paymentStatus: "Paid"
    },
    {
        bookingId: 4,
        booking_id: 4,
        BookingId: 4,
        courtId: 5, // Sân VIP
        CourtId: 5,
        userId: 4, // Khách VIP
        customerName: "Phạm Minh Hoàng",
        customerPhone: "0933445566",
        startTime: getRelativeDate(1, 8), // 8:00 Sáng mai
        endTime: getRelativeDate(1, 10),  // 10:00 Sáng mai
        status: "Confirmed",
        notes: "Sân VIP chuẩn bị nước suối lạnh",
        totalPrice: 240000, // 2 giờ x 120.000đ
        isPaid: true,
        paymentStatus: "Paid"
    },
    {
        bookingId: 5,
        booking_id: 5,
        BookingId: 5,
        courtId: 2,
        CourtId: 2,
        userId: null,
        customerName: "Nguyễn Thu Trang",
        customerPhone: "0977665544",
        startTime: getRelativeDate(-1, 15), // Hôm qua
        endTime: getRelativeDate(-1, 17),
        status: "Completed",
        notes: "",
        totalPrice: 160000,
        isPaid: true,
        paymentStatus: "Paid"
    }
];

// Service Orders (Đơn hàng gọi nước, thuê dịch vụ đi kèm)
export const serviceOrders = [
    {
        serviceOrderId: 1,
        service_order_id: 1,
        ServiceOrderId: 1,
        courtId: 1,
        court_id: 1,
        CourtId: 1,
        serviceItemId: 1, // Aquafina
        service_id: 1,
        ServiceItemId: 1,
        quantity: 2,
        Quantity: 2,
        bookingId: 1, // Liên kết với BookingId = 1
        booking_id: 1,
        BookingId: 1,
        orderTime: getTodayAt(17, 15),
        order_time: getTodayAt(17, 15),
        OrderTime: getTodayAt(17, 15),
        totalAmount: 30000 // 2 chai x 15k
    },
    {
        serviceOrderId: 2,
        service_order_id: 2,
        courtId: 1,
        serviceItemId: 3, // Thuê vợt
        service_id: 3,
        quantity: 1,
        bookingId: 1,
        orderTime: getTodayAt(17, 30),
        totalAmount: 50000 // 1 x 50k
    },
    {
        serviceOrderId: 3,
        service_order_id: 3,
        courtId: 3,
        serviceItemId: 2, // Revive
        service_id: 2,
        quantity: 4,
        bookingId: 2,
        orderTime: getTodayAt(18, 5),
        totalAmount: 80000 // 4 chai x 20k
    },
    {
        serviceOrderId: 4,
        service_order_id: 4,
        courtId: 5,
        serviceItemId: 4, // Ống cầu lông
        service_id: 4,
        quantity: 1,
        bookingId: 4,
        orderTime: getRelativeDate(1, 8, 15),
        totalAmount: 250000
    }
];

// Matchmaking (Ghép sân, tìm đối thủ)
export const matchmakingGroups = [
    {
        matchmakingGroupId: 1,
        match_id: 1,
        MatchmakingGroupId: 1,
        courtId: 3,
        court_id: 3,
        CourtId: 3,
        skillLevel: "Intermediate", // Trung bình
        skill_level: "Intermediate",
        SkillLevel: "Intermediate",
        startTime: getTodayAt(18),
        endTime: getTodayAt(20),
        playersNeeded: 4,
        players_needed: 4,
        PlayersNeeded: 4,
        playersJoined: 2,
        players_joined: 2,
        PlayersJoined: 2,
        status: "Open", // Open, Matched, Cancelled
        Status: "Open",
        creatorName: "Trần Minh",
        creator_name: "Trần Minh",
        CreatorName: "Trần Minh",
        bookingId: 2,
        booking_id: 2,
        BookingId: 2,
        participants: [
            { id: 1, fullName: "Trần Minh", phoneNumber: "0912345678" },
            { id: 2, fullName: "Văn Hải", phoneNumber: "0987654321" }
        ]
    },
    {
        matchmakingGroupId: 2,
        match_id: 2,
        courtId: 1,
        skillLevel: "Advanced", // Khá/Giỏi
        startTime: getTodayAt(19, 30),
        endTime: getTodayAt(21, 30),
        playersNeeded: 4,
        playersJoined: 3,
        status: "Open",
        creatorName: "Khánh Lê",
        bookingId: 3,
        participants: [
            { id: 3, fullName: "Khánh Lê", phoneNumber: "0900112233" },
            { id: 4, fullName: "Linh Phạm", phoneNumber: "0900112244" },
            { id: 5, fullName: "Hoàng An", phoneNumber: "0900112255" }
        ]
    },
    {
        matchmakingGroupId: 3,
        match_id: 3,
        courtId: 5,
        skillLevel: "Beginner", // Mới chơi
        startTime: getRelativeDate(1, 17, 0), // Chiều mai
        endTime: getRelativeDate(1, 19, 0),
        playersNeeded: 4,
        playersJoined: 1,
        status: "Open",
        creatorName: "Phạm Minh Hoàng",
        bookingId: 4,
        participants: [
            { id: 6, fullName: "Phạm Minh Hoàng", phoneNumber: "0933445566" }
        ]
    }
];

// Camera Highlights / Live Match Highlights (Xem lại trận đấu)
export const cameraHighlights = [
    {
        highlightId: 1,
        highlight_id: 1,
        videoId: 1,
        VideoId: 1,
        videoCode: "CAM_VID_1092",
        video_code: "CAM_VID_1092",
        VideoCode: "CAM_VID_1092",
        courtId: 1,
        court_id: 1,
        CourtId: 1,
        bookingId: 1,
        booking_id: 1,
        BookingId: 1,
        title: "Trận giao hữu Sân 01 - Nguyễn Văn A",
        Title: "Trận giao hữu Sân 01 - Nguyễn Văn A",
        startTime: "2026-06-14T17:00:00.000Z",
        endTime: "2026-06-14T19:00:00.000Z",
        customerName: "Nguyễn Văn A",
        customerPhone: "0987654321",
        videoUrl: "https://www.w3schools.com/html/mov_bbb.mp4", // Mock video URL
        thumbnailUrl: "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=600&q=80",
        fileSize: "840MB",
        status: "Pending" // Pending, Saved, Deleted
    },
    {
        highlightId: 2,
        highlight_id: 2,
        videoId: 2,
        courtId: 3,
        bookingId: 2,
        title: "Pha đập cầu cắm sân đỉnh cao - Nhóm Trần Minh",
        startTime: "2026-06-13T18:00:00.000Z",
        endTime: "2026-06-13T20:00:00.000Z",
        customerName: "Trần Minh",
        customerPhone: "0912345678",
        videoUrl: "https://www.w3schools.com/html/movie.mp4",
        thumbnailUrl: "https://images.unsplash.com/photo-1613918431208-67520e55478d?auto=format&fit=crop&w=600&q=80",
        fileSize: "920MB",
        status: "Saved"
    },
    {
        highlightId: 3,
        highlight_id: 3,
        videoId: 3,
        courtId: 2,
        bookingId: null,
        title: "Tổng hợp pha cứu cầu xuất sắc - Khách vãng lai",
        startTime: "2026-06-13T20:00:00.000Z",
        endTime: "2026-06-13T21:00:00.000Z",
        customerName: "Khách vãng lai",
        customerPhone: "0966554433",
        videoUrl: "",
        thumbnailUrl: "https://images.unsplash.com/photo-1554068865-24cecd4e34b8?auto=format&fit=crop&w=600&q=80",
        fileSize: "410MB",
        status: "Deleted"
    }
];

// ==========================================
// 4. DỮ LIỆU THỐNG KÊ (DASHBOARD ANALYTICS)
// ==========================================

// Doanh thu tuần bóc tách tiền Sân vs tiền Dịch vụ (Monday -> Sunday)
export const weeklyRevenue = [
    {
        dayOfWeek: "Thứ 2",
        day_of_week: "Thứ 2",
        DayOfWeek: "Thứ 2",
        courtRevenue: 160000,
        court_revenue: 160000,
        CourtRevenue: 160000,
        serviceRevenue: 90000,
        service_revenue: 90000,
        ServiceRevenue: 90000,
        totalRevenue: 250000,
        total_revenue: 250000,
        TotalRevenue: 250000
    },
    {
        dayOfWeek: "Thứ 3",
        courtRevenue: 160000,
        serviceRevenue: 490000,
        totalRevenue: 650000
    },
    {
        dayOfWeek: "Thứ 4",
        courtRevenue: 160000,
        serviceRevenue: 390000,
        totalRevenue: 550000
    },
    {
        dayOfWeek: "Thứ 5",
        courtRevenue: 160000,
        serviceRevenue: 740000,
        totalRevenue: 900000
    },
    {
        dayOfWeek: "Thứ 6",
        courtRevenue: 160000,
        serviceRevenue: 660000,
        totalRevenue: 820000
    },
    {
        dayOfWeek: "Thứ 7",
        courtRevenue: 240000,
        serviceRevenue: 1010000,
        totalRevenue: 1250000
    },
    {
        dayOfWeek: "Chủ Nhật",
        courtRevenue: 800000,
        serviceRevenue: 4050000,
        totalRevenue: 4850000 // Tổng khớp với seed data doanh thu ngày Chủ Nhật: 155k + 220k + 1450k + 3025k = 4.85M
    }
];

// Xuất mặc định tất cả dữ liệu
export default {
    users,
    courts,
    services,
    bookings,
    serviceOrders,
    matchmakingGroups,
    cameraHighlights,
    weeklyRevenue
};
