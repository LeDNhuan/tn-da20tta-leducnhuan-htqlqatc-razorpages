using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class VaiTro
{
    public int Id { get; set; }

    public string TenVaiTro { get; set; }

    public double? MucLuong { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
