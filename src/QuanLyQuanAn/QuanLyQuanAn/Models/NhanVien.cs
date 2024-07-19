using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class NhanVien
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

    public string AnhDaiDien { get; set; }

    public int? IdVaiTro { get; set; }

    public int? IdChiNhanh { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<HoaDonRutDiem> HoaDonRutDiems { get; set; } = new List<HoaDonRutDiem>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ChiNhanh IdChiNhanhNavigation { get; set; }

    public virtual VaiTro IdVaiTroNavigation { get; set; }

    public virtual ICollection<PhieuChi> PhieuChis { get; set; } = new List<PhieuChi>();

    public virtual ICollection<CaLamViec> CaLamViecs { get; set; } = new List<CaLamViec>();
}
