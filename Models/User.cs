using System;

namespace AquarSmartCourt.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; // Plaintext for demo/local purposes
    public string FullName { get; set; } = string.Empty;
    public int Role { get; set; } // 1: Customer, 2: Staff, 3: Manager
    
    // Additional staff information from the system description
    public string? StaffCode { get; set; } // e.g., NV001, NV002, AD001
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Position { get; set; } // "Lễ Tân", "Quản Lý", "Khách Hàng"
    
    // Membership & Status
    public int LoyaltyPoints { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
