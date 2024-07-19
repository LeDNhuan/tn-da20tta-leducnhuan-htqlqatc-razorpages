using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class ChiNhanh
{
    public int Id { get; set; }

    public string TenChiNhanh { get; set; }

    public string DiaChi { get; set; }

    public string Sdt { get; set; }

    public string AnhDaiDien { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Ban> Bans { get; set; } = new List<Ban>();

    public virtual ICollection<ChiNhanhNguyenLieuDauVao> ChiNhanhNguyenLieuDauVaos { get; set; } = new List<ChiNhanhNguyenLieuDauVao>();

    public virtual ICollection<MonGoi> MonGois { get; set; } = new List<MonGoi>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
