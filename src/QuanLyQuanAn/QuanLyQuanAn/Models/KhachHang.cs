using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class KhachHang
{
    public string Id { get; set; }

    public string HoTen { get; set; }

    public string GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string Cccd { get; set; }

    public string Sdt { get; set; }

    public string DiaChi { get; set; }

    public string Gmail { get; set; }

    public string MatKhau { get; set; }

    public int? DiemTichLuy { get; set; }

    public string AnhDaiDien { get; set; }

    public string HangKhachHang { get; set; }

    public string NguoiGioiThieu { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<HoaDonRutDiem> HoaDonRutDiems { get; set; } = new List<HoaDonRutDiem>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
