using Microsoft.EntityFrameworkCore;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
                Position = "Khách Hàng"
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
                Position = "Lễ tân"
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
                Position = "Quản lý"
            }
        );
    }
}
