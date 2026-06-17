CREATE TABLE [Courts] (
    [CourtId] int NOT NULL IDENTITY,
    [CourtCode] nvarchar(max) NOT NULL,
    [CourtName] nvarchar(max) NOT NULL,
    [HourlyPrice] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Courts] PRIMARY KEY ([CourtId])
);
GO


CREATE TABLE [ServiceItems] (
    [ServiceItemId] int NOT NULL IDENTITY,
    [ItemName] nvarchar(max) NOT NULL,
    [Unit] nvarchar(max) NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ServiceItems] PRIMARY KEY ([ServiceItemId])
);
GO


CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [Username] nvarchar(450) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Role] int NOT NULL,
    [StaffCode] nvarchar(max) NULL,
    [DateOfBirth] datetime2 NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Position] nvarchar(max) NULL,
    [LoyaltyPoints] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId])
);
GO


CREATE TABLE [MatchmakingGroups] (
    [MatchmakingGroupId] int NOT NULL IDENTITY,
    [SkillLevel] nvarchar(max) NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NOT NULL,
    [PlayersNeeded] int NOT NULL,
    [PlayersJoined] int NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CourtId] int NULL,
    [CreatorName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_MatchmakingGroups] PRIMARY KEY ([MatchmakingGroupId]),
    CONSTRAINT [FK_MatchmakingGroups_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId])
);
GO


CREATE TABLE [Bookings] (
    [BookingId] int NOT NULL IDENTITY,
    [CourtId] int NOT NULL,
    [CustomerName] nvarchar(max) NOT NULL,
    [CustomerPhone] nvarchar(max) NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Notes] nvarchar(max) NULL,
    [UserId] int NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([BookingId]),
    CONSTRAINT [FK_Bookings_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Bookings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
);
GO


CREATE TABLE [MatchmakingParticipants] (
    [MatchmakingParticipantId] int NOT NULL IDENTITY,
    [MatchmakingGroupId] int NOT NULL,
    [UserId] int NULL,
    [FullName] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [JoinedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_MatchmakingParticipants] PRIMARY KEY ([MatchmakingParticipantId]),
    CONSTRAINT [FK_MatchmakingParticipants_MatchmakingGroups_MatchmakingGroupId] FOREIGN KEY ([MatchmakingGroupId]) REFERENCES [MatchmakingGroups] ([MatchmakingGroupId]) ON DELETE CASCADE,
    CONSTRAINT [FK_MatchmakingParticipants_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
);
GO


CREATE TABLE [Invoices] (
    [InvoiceId] int NOT NULL IDENTITY,
    [BookingId] int NULL,
    [CourtId] int NOT NULL,
    [CustomerName] nvarchar(max) NOT NULL,
    [CustomerPhone] nvarchar(max) NOT NULL,
    [PlayHours] float NOT NULL,
    [CourtFee] decimal(18,2) NOT NULL,
    [ServiceFee] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [PaymentTime] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY ([InvoiceId]),
    CONSTRAINT [FK_Invoices_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([BookingId]),
    CONSTRAINT [FK_Invoices_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId]) ON DELETE CASCADE
);
GO


CREATE TABLE [ServiceOrders] (
    [ServiceOrderId] int NOT NULL IDENTITY,
    [CourtId] int NOT NULL,
    [ServiceItemId] int NOT NULL,
    [Quantity] int NOT NULL,
    [BookingId] int NULL,
    [OrderTime] datetime2 NOT NULL,
    CONSTRAINT [PK_ServiceOrders] PRIMARY KEY ([ServiceOrderId]),
    CONSTRAINT [FK_ServiceOrders_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([BookingId]),
    CONSTRAINT [FK_ServiceOrders_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ServiceOrders_ServiceItems_ServiceItemId] FOREIGN KEY ([ServiceItemId]) REFERENCES [ServiceItems] ([ServiceItemId]) ON DELETE CASCADE
);
GO


CREATE TABLE [SurveillanceVideos] (
    [VideoId] int NOT NULL IDENTITY,
    [VideoCode] nvarchar(max) NOT NULL,
    [CourtId] int NOT NULL,
    [BookingId] int NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NOT NULL,
    [CustomerName] nvarchar(max) NOT NULL,
    [CustomerPhone] nvarchar(max) NOT NULL,
    [VideoUrl] nvarchar(max) NOT NULL,
    [FileSize] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_SurveillanceVideos] PRIMARY KEY ([VideoId]),
    CONSTRAINT [FK_SurveillanceVideos_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([BookingId]),
    CONSTRAINT [FK_SurveillanceVideos_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [Courts] ([CourtId]) ON DELETE CASCADE
);
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourtId', N'CourtCode', N'CourtName', N'HourlyPrice', N'Status') AND [object_id] = OBJECT_ID(N'[Courts]'))
    SET IDENTITY_INSERT [Courts] ON;
INSERT INTO [Courts] ([CourtId], [CourtCode], [CourtName], [HourlyPrice], [Status])
VALUES (1, N'S01', N'Sân Con 01', 80000.0, N'InUse'),
(2, N'S02', N'Sân Con 02', 80000.0, N'Available'),
(3, N'S03', N'Sân Con 03', 80000.0, N'InUse'),
(4, N'S04', N'Sân Con 04', 80000.0, N'Maintenance');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CourtId', N'CourtCode', N'CourtName', N'HourlyPrice', N'Status') AND [object_id] = OBJECT_ID(N'[Courts]'))
    SET IDENTITY_INSERT [Courts] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ServiceItemId', N'Category', N'ItemName', N'Unit', N'UnitPrice') AND [object_id] = OBJECT_ID(N'[ServiceItems]'))
    SET IDENTITY_INSERT [ServiceItems] ON;
INSERT INTO [ServiceItems] ([ServiceItemId], [Category], [ItemName], [Unit], [UnitPrice])
VALUES (1, N'Nước uống', N'Nước suối Aquafina', N'Chai', 15000.0),
(2, N'Nước uống', N'Nước tăng lực Sting', N'Chai', 20000.0),
(3, N'Thuê vợt', N'Thuê vợt Yonex chính hãng', N'Cặp/Giờ', 50000.0),
(4, N'Phụ kiện', N'Ống cầu lông Hải Yến', N'Ống', 250000.0),
(5, N'Phụ kiện', N'Quấn cán vợt cao su', N'Cái', 30000.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ServiceItemId', N'Category', N'ItemName', N'Unit', N'UnitPrice') AND [object_id] = OBJECT_ID(N'[ServiceItems]'))
    SET IDENTITY_INSERT [ServiceItems] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'DateOfBirth', N'FullName', N'IsActive', N'LoyaltyPoints', N'Password', N'PhoneNumber', N'Position', N'Role', N'StaffCode', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([UserId], [DateOfBirth], [FullName], [IsActive], [LoyaltyPoints], [Password], [PhoneNumber], [Position], [Role], [StaffCode], [Username])
VALUES (1, NULL, N'Nguyễn Văn A', CAST(1 AS bit), 120, N'123', N'0987654321', N'Khách Hàng', 1, NULL, N'customer'),
(2, '1998-05-15T00:00:00.0000000', N'Lê Hoàng Nam', CAST(1 AS bit), 0, N'123', N'0912345678', N'Lễ tân', 2, N'NV001', N'staff'),
(3, '1985-10-20T00:00:00.0000000', N'Trần Thị B', CAST(1 AS bit), 0, N'123', N'0966554433', N'Quản lý', 3, N'AD001', N'manager');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'UserId', N'DateOfBirth', N'FullName', N'IsActive', N'LoyaltyPoints', N'Password', N'PhoneNumber', N'Position', N'Role', N'StaffCode', N'Username') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'BookingId', N'CourtId', N'CustomerName', N'CustomerPhone', N'EndTime', N'Notes', N'StartTime', N'Status', N'UserId') AND [object_id] = OBJECT_ID(N'[Bookings]'))
    SET IDENTITY_INSERT [Bookings] ON;
INSERT INTO [Bookings] ([BookingId], [CourtId], [CustomerName], [CustomerPhone], [EndTime], [Notes], [StartTime], [Status], [UserId])
VALUES (1, 1, N'Nguyễn Văn A', N'0987654321', '2026-06-14T19:00:00.0000000', NULL, '2026-06-14T17:00:00.0000000', N'Confirmed', 1),
(2, 3, N'Nhóm Trần Minh', N'0912345678', '2026-06-14T20:00:00.0000000', NULL, '2026-06-14T18:00:00.0000000', N'Confirmed', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'BookingId', N'CourtId', N'CustomerName', N'CustomerPhone', N'EndTime', N'Notes', N'StartTime', N'Status', N'UserId') AND [object_id] = OBJECT_ID(N'[Bookings]'))
    SET IDENTITY_INSERT [Bookings] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'InvoiceId', N'BookingId', N'CourtFee', N'CourtId', N'CustomerName', N'CustomerPhone', N'PaymentTime', N'PlayHours', N'ServiceFee', N'Status', N'TotalAmount') AND [object_id] = OBJECT_ID(N'[Invoices]'))
    SET IDENTITY_INSERT [Invoices] ON;
INSERT INTO [Invoices] ([InvoiceId], [BookingId], [CourtFee], [CourtId], [CustomerName], [CustomerPhone], [PaymentTime], [PlayHours], [ServiceFee], [Status], [TotalAmount])
VALUES (1, NULL, 120000.0, 2, N'Trần B', N'0966554433', '2026-06-14T10:00:00.0000000', 1.5E0, 35000.0, N'Paid', 155000.0),
(2, NULL, 160000.0, 1, N'Phan C', N'0988776655', '2026-06-14T11:30:00.0000000', 2.0E0, 60000.0, N'Paid', 220000.0),
(3, NULL, 240000.0, 3, N'Nguyễn D', N'0911223344', '2026-06-14T14:15:00.0000000', 3.0E0, 1210000.0, N'Paid', 1450000.0),
(4, NULL, 160000.0, 4, N'Lê E', N'0977665544', '2026-06-14T15:00:00.0000000', 2.0E0, 2865000.0, N'Paid', 3025000.0),
(5, NULL, 160000.0, 1, N'Hội viên F', N'0955443322', '2026-06-14T09:00:00.0000000', 2.0E0, 1290000.0, N'Paid', 1450000.0),
(6, NULL, 160000.0, 2, N'Hội viên G', N'0944332211', '2026-06-13T17:30:00.0000000', 2.0E0, 940000.0, N'Paid', 1100000.0),
(7, NULL, 80000.0, 1, N'Hội viên H', N'0933221100', '2026-06-13T19:00:00.0000000', 1.0E0, 70000.0, N'Paid', 150000.0),
(8, NULL, 160000.0, 3, N'Hội viên I', N'0922110099', '2026-06-12T18:00:00.0000000', 2.0E0, 660000.0, N'Paid', 820000.0),
(9, NULL, 160000.0, 2, N'Hội viên J', N'0911009988', '2026-06-11T20:00:00.0000000', 2.0E0, 740000.0, N'Paid', 900000.0),
(10, NULL, 160000.0, 1, N'Hội viên K', N'0900998877', '2026-06-10T18:00:00.0000000', 2.0E0, 390000.0, N'Paid', 550000.0),
(11, NULL, 160000.0, 3, N'Hội viên L', N'0900887766', '2026-06-09T19:00:00.0000000', 2.0E0, 490000.0, N'Paid', 650000.0),
(12, NULL, 160000.0, 4, N'Hội viên M', N'0900776655', '2026-06-08T17:00:00.0000000', 2.0E0, 90000.0, N'Paid', 250000.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'InvoiceId', N'BookingId', N'CourtFee', N'CourtId', N'CustomerName', N'CustomerPhone', N'PaymentTime', N'PlayHours', N'ServiceFee', N'Status', N'TotalAmount') AND [object_id] = OBJECT_ID(N'[Invoices]'))
    SET IDENTITY_INSERT [Invoices] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MatchmakingGroupId', N'CourtId', N'CreatorName', N'EndTime', N'PlayersJoined', N'PlayersNeeded', N'SkillLevel', N'StartTime', N'Status') AND [object_id] = OBJECT_ID(N'[MatchmakingGroups]'))
    SET IDENTITY_INSERT [MatchmakingGroups] ON;
INSERT INTO [MatchmakingGroups] ([MatchmakingGroupId], [CourtId], [CreatorName], [EndTime], [PlayersJoined], [PlayersNeeded], [SkillLevel], [StartTime], [Status])
VALUES (1, 3, N'Trần Minh', '2026-06-14T20:00:00.0000000', 2, 4, N'Intermediate', '2026-06-14T18:00:00.0000000', N'Open'),
(2, 1, N'Khánh Lê', '2026-06-14T21:30:00.0000000', 3, 4, N'Advanced', '2026-06-14T19:30:00.0000000', N'Open');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MatchmakingGroupId', N'CourtId', N'CreatorName', N'EndTime', N'PlayersJoined', N'PlayersNeeded', N'SkillLevel', N'StartTime', N'Status') AND [object_id] = OBJECT_ID(N'[MatchmakingGroups]'))
    SET IDENTITY_INSERT [MatchmakingGroups] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'VideoId', N'BookingId', N'CourtId', N'CustomerName', N'CustomerPhone', N'EndTime', N'FileSize', N'StartTime', N'Status', N'VideoCode', N'VideoUrl') AND [object_id] = OBJECT_ID(N'[SurveillanceVideos]'))
    SET IDENTITY_INSERT [SurveillanceVideos] ON;
INSERT INTO [SurveillanceVideos] ([VideoId], [BookingId], [CourtId], [CustomerName], [CustomerPhone], [EndTime], [FileSize], [StartTime], [Status], [VideoCode], [VideoUrl])
VALUES (2, NULL, 3, N'Trần Minh', N'0912345678', '2026-06-13T20:00:00.0000000', N'920MB', '2026-06-13T18:00:00.0000000', N'Saved', N'CAM_VID_1091', N'/videos/sim_court3.mp4'),
(3, NULL, 2, N'Khách vãng lai', N'0966554433', '2026-06-13T21:00:00.0000000', N'410MB', '2026-06-13T20:00:00.0000000', N'Deleted', N'CAM_VID_1090', N'');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'VideoId', N'BookingId', N'CourtId', N'CustomerName', N'CustomerPhone', N'EndTime', N'FileSize', N'StartTime', N'Status', N'VideoCode', N'VideoUrl') AND [object_id] = OBJECT_ID(N'[SurveillanceVideos]'))
    SET IDENTITY_INSERT [SurveillanceVideos] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MatchmakingParticipantId', N'FullName', N'JoinedAt', N'MatchmakingGroupId', N'PhoneNumber', N'UserId') AND [object_id] = OBJECT_ID(N'[MatchmakingParticipants]'))
    SET IDENTITY_INSERT [MatchmakingParticipants] ON;
INSERT INTO [MatchmakingParticipants] ([MatchmakingParticipantId], [FullName], [JoinedAt], [MatchmakingGroupId], [PhoneNumber], [UserId])
VALUES (1, N'Trần Minh', '2026-06-17T13:25:42.6125470+07:00', 1, N'0912345678', NULL),
(2, N'Văn Hải', '2026-06-17T13:25:42.6126657+07:00', 1, N'0987654321', NULL),
(3, N'Khánh Lê', '2026-06-17T13:25:42.6126661+07:00', 2, N'0900112233', NULL),
(4, N'Linh Phạm', '2026-06-17T13:25:42.6126663+07:00', 2, N'0900112244', NULL),
(5, N'Hoàng An', '2026-06-17T13:25:42.6126665+07:00', 2, N'0900112255', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'MatchmakingParticipantId', N'FullName', N'JoinedAt', N'MatchmakingGroupId', N'PhoneNumber', N'UserId') AND [object_id] = OBJECT_ID(N'[MatchmakingParticipants]'))
    SET IDENTITY_INSERT [MatchmakingParticipants] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ServiceOrderId', N'BookingId', N'CourtId', N'OrderTime', N'Quantity', N'ServiceItemId') AND [object_id] = OBJECT_ID(N'[ServiceOrders]'))
    SET IDENTITY_INSERT [ServiceOrders] ON;
INSERT INTO [ServiceOrders] ([ServiceOrderId], [BookingId], [CourtId], [OrderTime], [Quantity], [ServiceItemId])
VALUES (1, 1, 1, '2026-06-14T17:15:00.0000000', 2, 1),
(2, 1, 1, '2026-06-14T17:30:00.0000000', 1, 3);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ServiceOrderId', N'BookingId', N'CourtId', N'OrderTime', N'Quantity', N'ServiceItemId') AND [object_id] = OBJECT_ID(N'[ServiceOrders]'))
    SET IDENTITY_INSERT [ServiceOrders] OFF;
GO


IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'VideoId', N'BookingId', N'CourtId', N'CustomerName', N'CustomerPhone', N'EndTime', N'FileSize', N'StartTime', N'Status', N'VideoCode', N'VideoUrl') AND [object_id] = OBJECT_ID(N'[SurveillanceVideos]'))
    SET IDENTITY_INSERT [SurveillanceVideos] ON;
INSERT INTO [SurveillanceVideos] ([VideoId], [BookingId], [CourtId], [CustomerName], [CustomerPhone], [EndTime], [FileSize], [StartTime], [Status], [VideoCode], [VideoUrl])
VALUES (1, 1, 1, N'Nguyễn Văn A', N'0987654321', '2026-06-14T19:00:00.0000000', N'840MB', '2026-06-14T17:00:00.0000000', N'Pending', N'CAM_VID_1092', N'/videos/sim_court1.mp4');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'VideoId', N'BookingId', N'CourtId', N'CustomerName', N'CustomerPhone', N'EndTime', N'FileSize', N'StartTime', N'Status', N'VideoCode', N'VideoUrl') AND [object_id] = OBJECT_ID(N'[SurveillanceVideos]'))
    SET IDENTITY_INSERT [SurveillanceVideos] OFF;
GO


CREATE INDEX [IX_Bookings_CourtId] ON [Bookings] ([CourtId]);
GO


CREATE INDEX [IX_Bookings_UserId] ON [Bookings] ([UserId]);
GO


CREATE INDEX [IX_Invoices_BookingId] ON [Invoices] ([BookingId]);
GO


CREATE INDEX [IX_Invoices_CourtId] ON [Invoices] ([CourtId]);
GO


CREATE INDEX [IX_MatchmakingGroups_CourtId] ON [MatchmakingGroups] ([CourtId]);
GO


CREATE INDEX [IX_MatchmakingParticipants_MatchmakingGroupId] ON [MatchmakingParticipants] ([MatchmakingGroupId]);
GO


CREATE INDEX [IX_MatchmakingParticipants_UserId] ON [MatchmakingParticipants] ([UserId]);
GO


CREATE INDEX [IX_ServiceOrders_BookingId] ON [ServiceOrders] ([BookingId]);
GO


CREATE INDEX [IX_ServiceOrders_CourtId] ON [ServiceOrders] ([CourtId]);
GO


CREATE INDEX [IX_ServiceOrders_ServiceItemId] ON [ServiceOrders] ([ServiceItemId]);
GO


CREATE INDEX [IX_SurveillanceVideos_BookingId] ON [SurveillanceVideos] ([BookingId]);
GO


CREATE INDEX [IX_SurveillanceVideos_CourtId] ON [SurveillanceVideos] ([CourtId]);
GO


CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
GO


