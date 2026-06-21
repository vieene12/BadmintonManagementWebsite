using Microsoft.EntityFrameworkCore;
using System;

namespace AquarSmartCourt.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Court> Courts { get; set; } = null!;
    public DbSet<ServiceItem> ServiceItems { get; set; } = null!;
    public DbSet<ServiceOrder> ServiceOrders { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<MatchmakingGroup> MatchmakingGroups { get; set; } = null!;
    public DbSet<MatchmakingParticipant> MatchmakingParticipants { get; set; } = null!;
    public DbSet<SurveillanceVideo> SurveillanceVideos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Username)
            .IsUnique();

        modelBuilder.Entity<Court>()
            .Property(court => court.HourlyPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ServiceItem>()
            .Property(item => item.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Invoice>()
            .Property(invoice => invoice.CourtFee)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Invoice>()
            .Property(invoice => invoice.ServiceFee)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Invoice>()
            .Property(invoice => invoice.TotalAmount)
            .HasColumnType("decimal(18,2)");

        // Seed initial courts
        modelBuilder.Entity<Court>().HasData(
            new Court { CourtId = 1, CourtCode = "S01", CourtName = "Sân Con 01", HourlyPrice = 80000, Status = "InUse" },
            new Court { CourtId = 2, CourtCode = "S02", CourtName = "Sân Con 02", HourlyPrice = 80000, Status = "Available" },
            new Court { CourtId = 3, CourtCode = "S03", CourtName = "Sân Con 03", HourlyPrice = 80000, Status = "InUse" },
            new Court { CourtId = 4, CourtCode = "S04", CourtName = "Sân Con 04", HourlyPrice = 80000, Status = "Maintenance" }
        );

        // Seed initial service items
        modelBuilder.Entity<ServiceItem>().HasData(
            new ServiceItem { ServiceItemId = 1, ItemName = "Nước suối Aquafina", Unit = "Chai", UnitPrice = 15000, Category = "Nước uống" },
            new ServiceItem { ServiceItemId = 2, ItemName = "Nước tăng lực Sting", Unit = "Chai", UnitPrice = 20000, Category = "Nước uống" },
            new ServiceItem { ServiceItemId = 3, ItemName = "Thuê vợt Yonex chính hãng", Unit = "Cặp/Giờ", UnitPrice = 50000, Category = "Thuê vợt" },
            new ServiceItem { ServiceItemId = 4, ItemName = "Ống cầu lông Hải Yến", Unit = "Ống", UnitPrice = 250000, Category = "Phụ kiện" },
            new ServiceItem { ServiceItemId = 5, ItemName = "Quấn cán vợt cao su", Unit = "Cái", UnitPrice = 30000, Category = "Phụ kiện" }
        );

        // Seed initial users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = 1,
                Username = "customer",
                Password = "123",
                FullName = "Nguyễn Văn A",
                Role = 1,
                PhoneNumber = "0987654321",
                Position = "Khách Hàng",
                LoyaltyPoints = 120,
                IsActive = true
            },
            new User
            {
                UserId = 2,
                Username = "staff",
                Password = "123",
                FullName = "Lê Hoàng Nam",
                Role = 2,
                StaffCode = "NV001",
                DateOfBirth = new DateTime(1998, 5, 15),
                PhoneNumber = "0912345678",
                Position = "Lễ tân",
                LoyaltyPoints = 0,
                IsActive = true
            },
            new User
            {
                UserId = 3,
                Username = "manager",
                Password = "123",
                FullName = "Trần Thị B",
                Role = 3,
                StaffCode = "AD001",
                DateOfBirth = new DateTime(1985, 10, 20),
                PhoneNumber = "0966554433",
                Position = "Quản lý",
                LoyaltyPoints = 0,
                IsActive = true
            }
        );

        // Seed Bookings
        modelBuilder.Entity<Booking>().HasData(
            new Booking
            {
                BookingId = 1,
                CourtId = 1,
                CustomerName = "Nguyễn Văn A",
                CustomerPhone = "0987654321",
                StartTime = DateTime.Today.AddHours(17),
                EndTime = DateTime.Today.AddHours(19),
                Status = "Confirmed",
                UserId = 1
            },
            new Booking
            {
                BookingId = 2,
                CourtId = 3,
                CustomerName = "Nhóm Trần Minh",
                CustomerPhone = "0912345678",
                StartTime = DateTime.Today.AddHours(18),
                EndTime = DateTime.Today.AddHours(20),
                Status = "Confirmed"
            },
            new Booking
            {
                BookingId = 3,
                CourtId = 1,
                CustomerName = "Khánh Lê",
                CustomerPhone = "0900112233",
                StartTime = DateTime.Today.AddHours(19).AddMinutes(30),
                EndTime = DateTime.Today.AddHours(21).AddMinutes(30),
                Status = "Confirmed"
            }
        );

        // Seed Invoices (Today's revenue sum = 4,850,000đ; Weekly revenue matches chart values)
        modelBuilder.Entity<Invoice>().HasData(
            new Invoice
            {
                InvoiceId = 1,
                CourtId = 2,
                CustomerName = "Trần B",
                CustomerPhone = "0966554433",
                PlayHours = 1.5,
                CourtFee = 120000,
                ServiceFee = 35000,
                TotalAmount = 155000,
                PaymentTime = new DateTime(2026, 6, 14, 10, 0, 0),
                Status = "Paid"
            },
            new Invoice
            {
                InvoiceId = 2,
                CourtId = 1,
                CustomerName = "Phan C",
                CustomerPhone = "0988776655",
                PlayHours = 2.0,
                CourtFee = 160000,
                ServiceFee = 60000,
                TotalAmount = 220000,
                PaymentTime = new DateTime(2026, 6, 14, 11, 30, 0),
                Status = "Paid"
            },
            new Invoice
            {
                InvoiceId = 3,
                CourtId = 3,
                CustomerName = "Nguyễn D",
                CustomerPhone = "0911223344",
                PlayHours = 3.0,
                CourtFee = 240000,
                ServiceFee = 1210000,
                TotalAmount = 1450000,
                PaymentTime = new DateTime(2026, 6, 14, 14, 15, 0),
                Status = "Paid"
            },
            new Invoice
            {
                InvoiceId = 4,
                CourtId = 4,
                CustomerName = "Lê E",
                CustomerPhone = "0977665544",
                PlayHours = 2.0,
                CourtFee = 160000,
                ServiceFee = 2865000,
                TotalAmount = 3025000,
                PaymentTime = new DateTime(2026, 6, 14, 15, 0, 0),
                Status = "Paid"
            }, // Today's total: 155000 + 220000 + 1450000 + 3025000 = 4,850,000 VNĐ
            new Invoice
            {
                InvoiceId = 5,
                CourtId = 1,
                CustomerName = "Hội viên F",
                CustomerPhone = "0955443322",
                PlayHours = 2.0,
                CourtFee = 160000,
                ServiceFee = 1290000,
                TotalAmount = 1450000,
                PaymentTime = new DateTime(2026, 6, 14, 9, 0, 0),
                Status = "Paid"
            }, // Overrides to represent other days if filtered or just general past invoices
            new Invoice
            {
                InvoiceId = 6,
                CourtId = 2,
                CustomerName = "Hội viên G",
                CustomerPhone = "0944332211",
                PlayHours = 2.0,
                CourtFee = 160000,
                ServiceFee = 940000,
                TotalAmount = 1100000,
                PaymentTime = new DateTime(2026, 6, 13, 17, 30, 0),
                Status = "Paid"
            }, // T7 (yesterday): 1,100,000 + 150,000 = 1,250,000đ (matches T7 chart)
            new Invoice
            {
                InvoiceId = 7,
                CourtId = 1,
                CustomerName = "Hội viên H",
                CustomerPhone = "0933221100",
                PlayHours = 1.0,
                CourtFee = 80000,
                ServiceFee = 70000,
                TotalAmount = 150000,
                PaymentTime = new DateTime(2026, 6, 13, 19, 0, 0),
                Status = "Paid"
            },
            new Invoice
            {
                InvoiceId = 8,
                CourtId = 3,
                CustomerName = "Hội viên I",
                CustomerPhone = "0922110099",
                PlayHours = 2.0,
                CourtFee = 160000,
                ServiceFee = 660000,
                TotalAmount = 820000,
                PaymentTime = new DateTime(2026, 6, 12, 18, 0, 0),
                Status = "Paid"
            }, // T6: 820,000đ
            new Invoice
            {
                InvoiceId = 9,
                CourtId = 2,
                CustomerName = "Hội viên J",
                CustomerPhone = "0911009988",
                PlayHours = 2.0,
                CourtFee = 160000,
                ServiceFee = 740000,
                TotalAmount = 900000,
                PaymentTime = new DateTime(2026, 6, 11, 20, 0, 0),
                Status = "Paid"
            }, // T5: 900,000đ
            new Invoice
            {
                InvoiceId = 10,
                CourtId = 1,
                CustomerName = "Hội viên K",
                CustomerPhone = "0900998877",
                PlayHours = 2.0,
                CourtFee = 160000,
                ServiceFee = 390000,
                TotalAmount = 550000,
                PaymentTime = new DateTime(2026, 6, 10, 18, 0, 0),
                Status = "Paid"
            }, // T4: 550,000đ
            new Invoice
            {
                InvoiceId = 11,
                CourtId = 3,
                CustomerName = "Hội viên L",
                CustomerPhone = "0900887766",
                PlayHours = 2.0,
                CourtFee = 160000,
                ServiceFee = 490000,
                TotalAmount = 650000,
                PaymentTime = new DateTime(2026, 6, 9, 19, 0, 0),
                Status = "Paid"
            }, // T3: 650,000đ
            new Invoice
            {
                InvoiceId = 12,
                CourtId = 4,
                CustomerName = "Hội viên M",
                CustomerPhone = "0900776655",
                PlayHours = 2.0,
                CourtFee = 160000,
                ServiceFee = 90000,
                TotalAmount = 250000,
                PaymentTime = new DateTime(2026, 6, 8, 17, 0, 0),
                Status = "Paid"
            } // T2: 250,000đ
        );

        // Seed MatchmakingGroups
        modelBuilder.Entity<MatchmakingGroup>().HasData(
            new MatchmakingGroup
            {
                MatchmakingGroupId = 1,
                SkillLevel = "Intermediate",
                StartTime = DateTime.Today.AddHours(18),
                EndTime = DateTime.Today.AddHours(20),
                PlayersNeeded = 4,
                PlayersJoined = 2,
                Status = "Open",
                CourtId = 3,
                CreatorName = "Trần Minh",
                BookingId = 2
            },
            new MatchmakingGroup
            {
                MatchmakingGroupId = 2,
                SkillLevel = "Advanced",
                StartTime = DateTime.Today.AddHours(19).AddMinutes(30),
                EndTime = DateTime.Today.AddHours(21).AddMinutes(30),
                PlayersNeeded = 4,
                PlayersJoined = 3,
                Status = "Open",
                CourtId = 1,
                CreatorName = "Khánh Lê",
                BookingId = 3
            }
        );

        // Seed MatchmakingParticipants
        modelBuilder.Entity<MatchmakingParticipant>().HasData(
            new MatchmakingParticipant { MatchmakingParticipantId = 1, MatchmakingGroupId = 1, FullName = "Trần Minh", PhoneNumber = "0912345678" },
            new MatchmakingParticipant { MatchmakingParticipantId = 2, MatchmakingGroupId = 1, FullName = "Văn Hải", PhoneNumber = "0987654321" },
            new MatchmakingParticipant { MatchmakingParticipantId = 3, MatchmakingGroupId = 2, FullName = "Khánh Lê", PhoneNumber = "0900112233" },
            new MatchmakingParticipant { MatchmakingParticipantId = 4, MatchmakingGroupId = 2, FullName = "Linh Phạm", PhoneNumber = "0900112244" },
            new MatchmakingParticipant { MatchmakingParticipantId = 5, MatchmakingGroupId = 2, FullName = "Hoàng An", PhoneNumber = "0900112255" }
        );

        // Seed SurveillanceVideos
        modelBuilder.Entity<SurveillanceVideo>().HasData(
            new SurveillanceVideo
            {
                VideoId = 1,
                VideoCode = "CAM_VID_1092",
                CourtId = 1,
                BookingId = 1,
                StartTime = new DateTime(2026, 6, 14, 17, 0, 0),
                EndTime = new DateTime(2026, 6, 14, 19, 0, 0),
                CustomerName = "Nguyễn Văn A",
                CustomerPhone = "0987654321",
                VideoUrl = "/videos/sim_court1.mp4",
                FileSize = "840MB",
                Status = "Pending"
            },
            new SurveillanceVideo
            {
                VideoId = 2,
                VideoCode = "CAM_VID_1091",
                CourtId = 3,
                StartTime = new DateTime(2026, 6, 13, 18, 0, 0),
                EndTime = new DateTime(2026, 6, 13, 20, 0, 0),
                CustomerName = "Trần Minh",
                CustomerPhone = "0912345678",
                VideoUrl = "/videos/sim_court3.mp4",
                FileSize = "920MB",
                Status = "Saved"
            },
            new SurveillanceVideo
            {
                VideoId = 3,
                VideoCode = "CAM_VID_1090",
                CourtId = 2,
                StartTime = new DateTime(2026, 6, 13, 20, 0, 0),
                EndTime = new DateTime(2026, 6, 13, 21, 0, 0),
                CustomerName = "Khách vãng lai",
                CustomerPhone = "0966554433",
                VideoUrl = "",
                FileSize = "410MB",
                Status = "Deleted"
            }
        );

        // Seed ServiceOrders for Sân Con 01 (associated with BookingId = 1)
        modelBuilder.Entity<ServiceOrder>().HasData(
            new ServiceOrder { ServiceOrderId = 1, CourtId = 1, ServiceItemId = 1, Quantity = 2, BookingId = 1, OrderTime = new DateTime(2026, 6, 14, 17, 15, 0) },
            new ServiceOrder { ServiceOrderId = 2, CourtId = 1, ServiceItemId = 3, Quantity = 1, BookingId = 1, OrderTime = new DateTime(2026, 6, 14, 17, 30, 0) }
        );
    }
}
