using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class KhuyenMai
{
    public int Id { get; set; }

    public string TenKhuyenMai { get; set; }

    public int? ChietKhau { get; set; }

    public string TrangThai { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
