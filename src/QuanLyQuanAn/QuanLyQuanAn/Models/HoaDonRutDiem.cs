using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class HoaDonRutDiem
{
    public int Id { get; set; }

    public int? DiemRut { get; set; }

    public int? ChietKhau { get; set; }

    public double? TongTien { get; set; }

    public DateTime? GioVao { get; set; }

    public DateTime? GioRa { get; set; }

    public string TrangThai { get; set; }

    public string IdNhanVien { get; set; }

    public string IdKhachHang { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public virtual KhachHang IdKhachHangNavigation { get; set; }

    public virtual NhanVien IdNhanVienNavigation { get; set; }
}
