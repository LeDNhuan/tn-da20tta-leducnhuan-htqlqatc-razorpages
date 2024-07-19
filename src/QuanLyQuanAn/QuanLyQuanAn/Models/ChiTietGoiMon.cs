using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class ChiTietGoiMon
{
    public int Id { get; set; }

    public int? SoLuong { get; set; }

    public string GhiChu { get; set; }

    public double? ThanhTien { get; set; }

    public string TrangThai { get; set; }

    public int? IdHoaDon { get; set; }

    public int? IdMonGoi { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual HoaDon IdHoaDonNavigation { get; set; }

    public virtual MonGoi IdMonGoiNavigation { get; set; }
}
