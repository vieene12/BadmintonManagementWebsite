using System.ComponentModel.DataAnnotations;

namespace AquarSmartCourt.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Vui long nhap ten dang nhap.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Ten dang nhap phai tu 3 den 50 ky tu.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Mat khau phai co it nhat 3 ky tu.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long xac nhan mat khau.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Mat khau xac nhan khong khop.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap ho va ten.")]
    [StringLength(100, ErrorMessage = "Ho va ten khong duoc vuot qua 100 ky tu.")]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "So dien thoai khong hop le.")]
    [StringLength(20, ErrorMessage = "So dien thoai khong duoc vuot qua 20 ky tu.")]
    public string? PhoneNumber { get; set; }
}
