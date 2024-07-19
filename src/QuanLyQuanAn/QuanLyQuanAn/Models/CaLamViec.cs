using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class CaLamViec
{
    public int Id { get; set; }

    public double? TienDauCa { get; set; }

    public double? TienTrongCa { get; set; }

    public double? TienBanGiao { get; set; }

    public double? TienChenhLech { get; set; }

    public DateTime? ThoiGianBatDau { get; set; }

    public DateTime? ThoiGianKetThuc { get; set; }

    public string GhiChu { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
