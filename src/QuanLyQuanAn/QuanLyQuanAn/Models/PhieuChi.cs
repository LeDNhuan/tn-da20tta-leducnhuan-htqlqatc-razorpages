using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class PhieuChi
{
    public int Id { get; set; }

    public double? SoChi { get; set; }

    public DateTime? NgayLap { get; set; }

    public string GhiChu { get; set; }

    public string IdNhanVien { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual NhanVien IdNhanVienNavigation { get; set; }
}
