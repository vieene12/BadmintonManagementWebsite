-- ==========================================
-- SCRIPT TẠO DATABASE VÀ CHÈN DỮ LIỆU MẪU
-- Dự án: AquarSmartCourt (Badminton Management Website)
-- Hệ quản trị CSDL: Microsoft SQL Server
-- ==========================================

USE [master];
GO

-- 1. Xóa Database cũ nếu tồn tại
IF EXISTS (SELECT * FROM sys.databases WHERE name = N'AquarSmartCourt')
BEGIN
    ALTER DATABASE [AquarSmartCourt] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [AquarSmartCourt];
END
GO

-- Tạo Database mới
CREATE DATABASE [AquarSmartCourt];
GO

USE [AquarSmartCourt];
GO

-- 2. Tạo bảng Courts (Danh sách sân)
CREATE TABLE [Courts] (
    [CourtId] INT IDENTITY(1,1) NOT NULL,
    [CourtCode] NVARCHAR(100) NOT NULL,
    [CourtName] NVARCHAR(255) NOT NULL,
    [HourlyPrice] DECIMAL(18,2) NOT NULL,
    [Status] NVARCHAR(100) NOT NULL DEFAULT N'Available', -- Available, InUse, Maintenance
    CONSTRAINT [PK_Courts] PRIMARY KEY ([CourtId])
);
GO

-- 3. Tạo bảng Users (Tài khoản người dùng/nhân viên)
CREATE TABLE [Users] (
    [UserId] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(450) NOT NULL,
    [Password] NVARCHAR(MAX) NOT NULL,
    [FullName] NVARCHAR(255) NOT NULL,
    [Role] INT NOT NULL, -- 1: Customer, 2: Staff, 3: Manager
    [StaffCode] NVARCHAR(100) NULL,
    [DateOfBirth] DATETIME2 NULL,
    [PhoneNumber] NVARCHAR(50) NULL,
    [Position] NVARCHAR(100) NULL, -- Khách Hàng, Lễ tân, Quản lý
    [LoyaltyPoints] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);
GO

-- Tạo chỉ mục duy nhất cho Username để tránh trùng lặp tài khoản
CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
GO

-- 4. Tạo bảng Bookings (Đặt sân)
CREATE TABLE [Bookings] (
    [BookingId] INT IDENTITY(1,1) NOT NULL,
    [CourtId] INT NOT NULL,
    [CustomerName] NVARCHAR(255) NOT NULL,
    [CustomerPhone] NVARCHAR(50) NOT NULL,
    [StartTime] DATETIME2 NOT NULL,
    [EndTime] DATETIME2 NOT NULL,
    [Status] NVARCHAR(100) NOT NULL DEFAULT N'Confirmed', -- Confirmed, Completed, Cancelled
    [Notes] NVARCHAR(MAX) NULL,
    [UserId] INT NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([BookingId]),
    CONSTRAINT [FK_Bookings_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Bookings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Bookings_CourtId] ON [Bookings] ([CourtId]);
GO
CREATE INDEX [IX_Bookings_UserId] ON [Bookings] ([UserId]);
GO

-- 5. Tạo bảng ServiceItems (Sản phẩm / Dịch vụ đi kèm)
CREATE TABLE [ServiceItems] (
    [ServiceItemId] INT IDENTITY(1,1) NOT NULL,
    [ItemName] NVARCHAR(255) NOT NULL,
    [Unit] NVARCHAR(100) NOT NULL, -- Chai, Cặp/Giờ, Ống, Cái...
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    [Category] NVARCHAR(150) NOT NULL DEFAULT N'Khác', -- Nước uống, Thuê vợt, Phụ kiện
    CONSTRAINT [PK_ServiceItems] PRIMARY KEY ([ServiceItemId])
);
GO

-- 6. Tạo bảng ServiceOrders (Gọi đồ / Sử dụng dịch vụ cho sân đặt)
CREATE TABLE [ServiceOrders] (
    [ServiceOrderId] INT IDENTITY(1,1) NOT NULL,
    [CourtId] INT NOT NULL,
    [ServiceItemId] INT NOT NULL,
    [Quantity] INT NOT NULL,
    [BookingId] INT NULL,
    [OrderTime] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_ServiceOrders] PRIMARY KEY ([ServiceOrderId]),
    CONSTRAINT [FK_ServiceOrders_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ServiceOrders_ServiceItems_ServiceItemId] FOREIGN KEY ([ServiceItemId]) REFERENCES [ServiceItems] ([ServiceItemId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ServiceOrders_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([BookingId]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_ServiceOrders_CourtId] ON [ServiceOrders] ([CourtId]);
GO
CREATE INDEX [IX_ServiceOrders_ServiceItemId] ON [ServiceOrders] ([ServiceItemId]);
GO
CREATE INDEX [IX_ServiceOrders_BookingId] ON [ServiceOrders] ([BookingId]);
GO

-- 7. Tạo bảng Invoices (Hóa đơn thanh toán)
CREATE TABLE [Invoices] (
    [InvoiceId] INT IDENTITY(1,1) NOT NULL,
    [BookingId] INT NULL,
    [CourtId] INT NOT NULL,
    [CustomerName] NVARCHAR(255) NOT NULL,
    [CustomerPhone] NVARCHAR(50) NOT NULL,
    [PlayHours] FLOAT NOT NULL,
    [CourtFee] DECIMAL(18,2) NOT NULL,
    [ServiceFee] DECIMAL(18,2) NOT NULL,
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [PaymentTime] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [Status] NVARCHAR(100) NOT NULL DEFAULT N'Paid', -- Paid, Unpaid
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([InvoiceId]),
    CONSTRAINT [FK_Invoices_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([BookingId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Invoices_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Invoices_BookingId] ON [Invoices] ([BookingId]);
GO
CREATE INDEX [IX_Invoices_CourtId] ON [Invoices] ([CourtId]);
GO

-- 8. Tạo bảng MatchmakingGroups (Nhóm ghép cặp tìm đối thủ)
CREATE TABLE [MatchmakingGroups] (
    [MatchmakingGroupId] INT IDENTITY(1,1) NOT NULL,
    [SkillLevel] NVARCHAR(100) NOT NULL DEFAULT N'Intermediate', -- Beginner, Intermediate, Advanced
    [StartTime] DATETIME2 NOT NULL,
    [EndTime] DATETIME2 NOT NULL,
    [PlayersNeeded] INT NOT NULL,
    [PlayersJoined] INT NOT NULL DEFAULT 1,
    [Status] NVARCHAR(100) NOT NULL DEFAULT N'Open', -- Open, Matched, Cancelled
    [CourtId] INT NULL,
    [CreatorName] NVARCHAR(255) NOT NULL,
    [BookingId] INT NULL,
    CONSTRAINT [PK_MatchmakingGroups] PRIMARY KEY ([MatchmakingGroupId]),
    CONSTRAINT [FK_MatchmakingGroups_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MatchmakingGroups_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([BookingId]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_MatchmakingGroups_CourtId] ON [MatchmakingGroups] ([CourtId]);
GO
CREATE INDEX [IX_MatchmakingGroups_BookingId] ON [MatchmakingGroups] ([BookingId]);
GO

-- 9. Tạo bảng MatchmakingParticipants (Thành viên tham gia ghép cặp)
CREATE TABLE [MatchmakingParticipants] (
    [MatchmakingParticipantId] INT IDENTITY(1,1) NOT NULL,
    [MatchmakingGroupId] INT NOT NULL,
    [UserId] INT NULL,
    [FullName] NVARCHAR(255) NOT NULL,
    [PhoneNumber] NVARCHAR(50) NOT NULL,
    [JoinedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_MatchmakingParticipants] PRIMARY KEY ([MatchmakingParticipantId]),
    CONSTRAINT [FK_MatchmakingParticipants_MatchmakingGroups_MatchmakingGroupId] FOREIGN KEY ([MatchmakingGroupId]) REFERENCES [MatchmakingGroups] ([MatchmakingGroupId]) ON DELETE CASCADE,
    CONSTRAINT [FK_MatchmakingParticipants_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_MatchmakingParticipants_MatchmakingGroupId] ON [MatchmakingParticipants] ([MatchmakingGroupId]);
GO
CREATE INDEX [IX_MatchmakingParticipants_UserId] ON [MatchmakingParticipants] ([UserId]);
GO

-- 10. Tạo bảng SurveillanceVideos (Video lưu trữ từ Camera giám sát sân)
CREATE TABLE [SurveillanceVideos] (
    [VideoId] INT IDENTITY(1,1) NOT NULL,
    [VideoCode] NVARCHAR(100) NOT NULL,
    [CourtId] INT NOT NULL,
    [BookingId] INT NULL,
    [StartTime] DATETIME2 NOT NULL,
    [EndTime] DATETIME2 NOT NULL,
    [CustomerName] NVARCHAR(255) NOT NULL,
    [CustomerPhone] NVARCHAR(50) NOT NULL,
    [VideoUrl] NVARCHAR(MAX) NOT NULL,
    [FileSize] NVARCHAR(100) NOT NULL DEFAULT N'800MB',
    [Status] NVARCHAR(100) NOT NULL DEFAULT N'Pending', -- Pending, Saved, Deleted
    CONSTRAINT [PK_SurveillanceVideos] PRIMARY KEY ([VideoId]),
    CONSTRAINT [FK_SurveillanceVideos_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SurveillanceVideos_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([BookingId]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_SurveillanceVideos_CourtId] ON [SurveillanceVideos] ([CourtId]);
GO
CREATE INDEX [IX_SurveillanceVideos_BookingId] ON [SurveillanceVideos] ([BookingId]);
GO

-- 11. Tạo bảng Products (Cửa hàng phụ kiện liên kết)
CREATE TABLE [Products] (
    [ProductId] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    [Badge] NVARCHAR(50) NULL,
    [ImageUrl] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId])
);
GO

-- 12. Tạo bảng NewsArticles (Tin tức, Sự kiện & Highlight)
CREATE TABLE [NewsArticles] (
    [NewsArticleId] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Category] NVARCHAR(100) NULL,
    [BadgeColor] NVARCHAR(50) NULL,
    [ImageUrl] NVARCHAR(MAX) NULL,
    [VideoActionUrl] NVARCHAR(MAX) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [PK_NewsArticles] PRIMARY KEY ([NewsArticleId])
);
GO

-- ==========================================
-- CHÈN DỮ LIỆU MẪU (SEED DATA)
-- ==========================================

-- Chèn dữ liệu bảng Courts
SET IDENTITY_INSERT [Courts] ON;
INSERT INTO [Courts] ([CourtId], [CourtCode], [CourtName], [HourlyPrice], [Status]) VALUES
(1, 'S01', N'Sân Con 01', 80000.00, 'InUse'),
(2, 'S02', N'Sân Con 02', 80000.00, 'Available'),
(3, 'S03', N'Sân Con 03', 80000.00, 'InUse'),
(4, 'S04', N'Sân Con 04', 80000.00, 'Maintenance');
SET IDENTITY_INSERT [Courts] OFF;
GO

-- Chèn dữ liệu bảng ServiceItems
SET IDENTITY_INSERT [ServiceItems] ON;
INSERT INTO [ServiceItems] ([ServiceItemId], [ItemName], [Unit], [UnitPrice], [Category]) VALUES
(1, N'Nước suối Aquafina', N'Chai', 15000.00, N'Nước uống'),
(2, N'Nước tăng lực Sting', N'Chai', 20000.00, N'Nước uống'),
(3, N'Thuê vợt Yonex chính hãng', N'Cặp/Giờ', 50000.00, N'Thuê vợt'),
(4, N'Ống cầu lông Hải Yến', N'Ống', 250000.00, N'Phụ kiện'),
(5, N'Quấn cán vợt cao su', N'Cái', 30000.00, N'Phụ kiện');
SET IDENTITY_INSERT [ServiceItems] OFF;
GO

-- Chèn dữ liệu bảng Users
SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([UserId], [Username], [Password], [FullName], [Role], [StaffCode], [DateOfBirth], [PhoneNumber], [Position], [LoyaltyPoints], [IsActive]) VALUES
(1, 'customer', '123', N'Nguyễn Văn A', 1, NULL, NULL, '0987654321', N'Khách Hàng', 120, 1),
(2, 'staff', '123', N'Lê Hoàng Nam', 2, 'NV001', '1998-05-15 00:00:00', '0912345678', N'Lễ tân', 0, 1),
(3, 'manager', '123', N'Trần Thị B', 3, 'AD001', '1985-10-20 00:00:00', '0966554433', N'Quản lý', 0, 1);
SET IDENTITY_INSERT [Users] OFF;
GO

-- Chèn dữ liệu bảng Bookings (Mốc thời gian lấy theo ngày hiện tại)
SET IDENTITY_INSERT [Bookings] ON;
INSERT INTO [Bookings] ([BookingId], [CourtId], [CustomerName], [CustomerPhone], [StartTime], [EndTime], [Status], [Notes], [UserId]) VALUES
(1, 1, N'Nguyễn Văn A', '0987654321', DATEADD(hour, 17, CAST(CAST(GETDATE() AS DATE) AS DATETIME2)), DATEADD(hour, 19, CAST(CAST(GETDATE() AS DATE) AS DATETIME2)), 'Confirmed', NULL, 1),
(2, 3, N'Nhóm Trần Minh', '0912345678', DATEADD(hour, 18, CAST(CAST(GETDATE() AS DATE) AS DATETIME2)), DATEADD(hour, 20, CAST(CAST(GETDATE() AS DATE) AS DATETIME2)), 'Confirmed', NULL, NULL),
(3, 1, N'Khánh Lê', '0900112233', DATEADD(minute, 30, DATEADD(hour, 19, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(minute, 30, DATEADD(hour, 21, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 'Confirmed', NULL, NULL);
SET IDENTITY_INSERT [Bookings] OFF;
GO

-- Chèn dữ liệu bảng Invoices (Hóa đơn ghi nhận doanh thu các ngày trong tuần)
SET IDENTITY_INSERT [Invoices] ON;
INSERT INTO [Invoices] ([InvoiceId], [BookingId], [CourtId], [CustomerName], [CustomerPhone], [PlayHours], [CourtFee], [ServiceFee], [TotalAmount], [PaymentTime], [Status]) VALUES
(1, NULL, 2, N'Trần B', '0966554433', 1.5, 120000.00, 35000.00, 155000.00, '2026-06-14 10:00:00', 'Paid'),
(2, NULL, 1, N'Phan C', '0988776655', 2.0, 160000.00, 60000.00, 220000.00, '2026-06-14 11:30:00', 'Paid'),
(3, NULL, 3, N'Nguyễn D', '0911223344', 3.0, 240000.00, 1210000.00, 1450000.00, '2026-06-14 14:15:00', 'Paid'),
(4, NULL, 4, N'Lê E', '0977665544', 2.0, 160000.00, 2865000.00, 3025000.00, '2026-06-14 15:00:00', 'Paid'),
(5, NULL, 1, N'Hội viên F', '0955443322', 2.0, 160000.00, 1290000.00, 1450000.00, '2026-06-14 09:00:00', 'Paid'),
(6, NULL, 2, N'Hội viên G', '0944332211', 2.0, 160000.00, 940000.00, 1100000.00, '2026-06-13 17:30:00', 'Paid'),
(7, NULL, 1, N'Hội viên H', '0933221100', 1.0, 80000.00, 70000.00, 150000.00, '2026-06-13 19:00:00', 'Paid'),
(8, NULL, 3, N'Hội viên I', '0922110099', 2.0, 160000.00, 660000.00, 820000.00, '2026-06-12 18:00:00', 'Paid'),
(9, NULL, 2, N'Hội viên J', '0911009988', 2.0, 160000.00, 740000.00, 900000.00, '2026-06-11 20:00:00', 'Paid'),
(10, NULL, 1, N'Hội viên K', '0900998877', 2.0, 160000.00, 390000.00, 550000.00, '2026-06-10 18:00:00', 'Paid'),
(11, NULL, 3, N'Hội viên L', '0900887766', 2.0, 160000.00, 490000.00, 650000.00, '2026-06-09 19:00:00', 'Paid'),
(12, NULL, 4, N'Hội viên M', '0900776655', 2.0, 160000.00, 90000.00, 250000.00, '2026-06-08 17:00:00', 'Paid');
SET IDENTITY_INSERT [Invoices] OFF;
GO

-- Chèn dữ liệu bảng MatchmakingGroups
SET IDENTITY_INSERT [MatchmakingGroups] ON;
INSERT INTO [MatchmakingGroups] ([MatchmakingGroupId], [SkillLevel], [StartTime], [EndTime], [PlayersNeeded], [PlayersJoined], [Status], [CourtId], [CreatorName], [BookingId]) VALUES
(1, 'Intermediate', DATEADD(hour, 18, CAST(CAST(GETDATE() AS DATE) AS DATETIME2)), DATEADD(hour, 20, CAST(CAST(GETDATE() AS DATE) AS DATETIME2)), 4, 2, 'Open', 3, N'Trần Minh', 2),
(2, 'Advanced', DATEADD(minute, 30, DATEADD(hour, 19, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), DATEADD(minute, 30, DATEADD(hour, 21, CAST(CAST(GETDATE() AS DATE) AS DATETIME2))), 4, 3, 'Open', 1, N'Khánh Lê', 3);
SET IDENTITY_INSERT [MatchmakingGroups] OFF;
GO

-- Chèn dữ liệu bảng MatchmakingParticipants
SET IDENTITY_INSERT [MatchmakingParticipants] ON;
INSERT INTO [MatchmakingParticipants] ([MatchmakingParticipantId], [MatchmakingGroupId], [UserId], [FullName], [PhoneNumber], [JoinedAt]) VALUES
(1, 1, NULL, N'Trần Minh', '0912345678', GETDATE()),
(2, 1, NULL, N'Văn Hải', '0987654321', GETDATE()),
(3, 2, NULL, N'Khánh Lê', '0900112233', GETDATE()),
(4, 2, NULL, N'Linh Phạm', '0900112244', GETDATE()),
(5, 2, NULL, N'Hoàng An', '0900112255', GETDATE());
SET IDENTITY_INSERT [MatchmakingParticipants] OFF;
GO

-- Chèn dữ liệu bảng SurveillanceVideos
SET IDENTITY_INSERT [SurveillanceVideos] ON;
INSERT INTO [SurveillanceVideos] ([VideoId], [VideoCode], [CourtId], [BookingId], [StartTime], [EndTime], [CustomerName], [CustomerPhone], [VideoUrl], [FileSize], [Status]) VALUES
(1, 'CAM_VID_1092', 1, 1, '2026-06-14 17:00:00', '2026-06-14 19:00:00', N'Nguyễn Văn A', '0987654321', '/videos/sim_court1.mp4', '840MB', 'Pending'),
(2, 'CAM_VID_1091', 3, NULL, '2026-06-13 18:00:00', '2026-06-13 20:00:00', N'Trần Minh', '0912345678', '/videos/sim_court3.mp4', '920MB', 'Saved'),
(3, 'CAM_VID_1090', 2, NULL, '2026-06-13 20:00:00', '2026-06-13 21:00:00', N'Khách vãng lai', '0966554433', '', '410MB', 'Deleted');
SET IDENTITY_INSERT [SurveillanceVideos] OFF;
GO

-- Chèn dữ liệu bảng ServiceOrders
SET IDENTITY_INSERT [ServiceOrders] ON;
INSERT INTO [ServiceOrders] ([ServiceOrderId], [CourtId], [ServiceItemId], [Quantity], [BookingId], [OrderTime]) VALUES
(1, 1, 1, 2, 1, '2026-06-14 17:15:00'),
(2, 1, 3, 1, 1, '2026-06-14 17:30:00');
SET IDENTITY_INSERT [ServiceOrders] OFF;
GO

-- Chèn dữ liệu bảng Products
SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] ([ProductId], [Name], [Description], [Price], [Badge], [ImageUrl]) VALUES
(1, N'Vợt Cầu Lông Yonex Astrox 88D Pro', N'Dòng vợt cao cấp hỗ trợ những cú đập smash uy lực mạnh mẽ từ phía cuối sân.', 3990000.00, 'HOT', 'https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&w=150&q=80'),
(2, N'Giày Cầu Lông Victor P9200', N'Giày thi đấu chuyên nghiệp, chống lật cổ chân, đệm lót êm ái, bám sân hoàn hảo.', 2150000.00, '-15%', 'https://images.unsplash.com/photo-1608231387042-66d1773070a5?auto=format&fit=crop&w=150&q=80'),
(3, N'Vợt Cầu Lông Li-Ning Tectonic 7', N'Thiết kế hệ thống giảm chấn hiện đại giúp vung vợt nhanh, linh hoạt phòng thủ.', 3200000.00, 'NEW', 'https://images.unsplash.com/photo-1617083266344-0b1a039755b6?auto=format&fit=crop&w=150&q=80'),
(4, N'Balo Cầu Lông Yonex Pro Bag', N'Không gian rộng rãi, ngăn chứa giày riêng và lớp cách nhiệt chống nóng bảo vệ vợt.', 1450000.00, NULL, 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?auto=format&fit=crop&w=150&q=80'),
(5, N'Giày Cầu Lông Yonex Power Cushion 65Z3', N'Dòng giày huyền thoại mang lại độ êm ái tối đa và bảo vệ khớp gối tối ưu.', 2850000.00, 'HOT', 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=150&q=80'),
(6, N'Ống Cầu Lông Ba Sao (12 Quả)', N'Cầu bay đầm, quỹ đạo chuẩn xác, độ bền cao cho tập luyện và thi đấu phong trào.', 220000.00, NULL, 'https://images.unsplash.com/photo-1611252199277-2bc46617a6a4?auto=format&fit=crop&w=150&q=80'),
(7, N'Ống Cầu Lông Vina Star (12 Quả)', N'Công nghệ sản xuất hiện đại, lông cầu bền dai, quỹ đạo bay rất ổn định.', 240000.00, 'HOT', 'https://images.unsplash.com/photo-1611252199277-2bc46617a6a4?auto=format&fit=crop&w=150&q=80'),
(8, N'Quấn Cán Vợt VS Grip (Vỉ 10 Cái)', N'Chất liệu cao su non bám dính tốt, thấm hút mồ hôi hiệu quả, êm tay.', 120000.00, 'NEW', 'https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f?auto=format&fit=crop&w=150&q=80');
SET IDENTITY_INSERT [Products] OFF;
GO

-- Chèn dữ liệu bảng NewsArticles
SET IDENTITY_INSERT [NewsArticles] ON;
INSERT INTO [NewsArticles] ([NewsArticleId], [Title], [Description], [Category], [BadgeColor], [ImageUrl], [VideoActionUrl]) VALUES
(1, N'Chung kết Đơn Nam 2026', N'Kịch tính đến set thứ 3 giữa hai hạt giống...', 'NEW MATCH', 'bg-danger', 'https://images.unsplash.com/photo-1560089000-7433a4ebbd64?auto=format&fit=crop&q=80&w=350', 'Chung kết Đơn Nam CLB Aquar 2026'),
(2, N'Highlights Cú Đập Cầu Khủng', N'Tuyển tập những pha ghi điểm tốc độ cao...', 'BEST SMASHES', 'bg-primary', 'https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?auto=format&fit=crop&q=80&w=350', 'Highlights Smash Trận 3 Sân Con 2'),
(3, N'Giải Giao Hữu Mùa Hè 2026', N'Đăng ký tham gia ngay trước ngày 25/06...', 'EVENT', 'bg-warning', 'https://images.unsplash.com/photo-1521412644187-c49fa049e84d?auto=format&fit=crop&q=80&w=350', NULL);
SET IDENTITY_INSERT [NewsArticles] OFF;
GO
