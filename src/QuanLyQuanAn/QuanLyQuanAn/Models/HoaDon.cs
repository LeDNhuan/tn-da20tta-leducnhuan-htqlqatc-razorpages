using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class HoaDon
{
    public int Id { get; set; }

    public double? TongTien { get; set; }

    public DateTime? GioVao { get; set; }

    public DateTime? GioRa { get; set; }

    public int? DiemTru { get; set; }

    public string TrangThai { get; set; }

    public double? TienHoaHong { get; set; }

    public int? MucDoCho { get; set; }

    public string IdNhanVien { get; set; }

    public string IdKhachHang { get; set; }

    public int? IdKhuyenMai { get; set; }

    public int? IdBan { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public virtual ICollection<ChiTietGoiMon> ChiTietGoiMons { get; set; } = new List<ChiTietGoiMon>();

    public virtual Ban IdBanNavigation { get; set; }

    public virtual KhachHang IdKhachHangNavigation { get; set; }

    public virtual KhuyenMai IdKhuyenMaiNavigation { get; set; }

    public virtual NhanVien IdNhanVienNavigation { get; set; }
}
