using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class MonGoi
{
    public int Id { get; set; }

    public string TenMon { get; set; }

    public double? GiaBan { get; set; }

    public string AnhDaiDien { get; set; }

    public int? IdLoaiMon { get; set; }

    public int? IdChiNhanh { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChiTietGoiMon> ChiTietGoiMons { get; set; } = new List<ChiTietGoiMon>();

    public virtual ChiNhanh IdChiNhanhNavigation { get; set; }

    public virtual LoaiMon IdLoaiMonNavigation { get; set; }

    public virtual ICollection<NguyenLieuMonGoi> NguyenLieuMonGois { get; set; } = new List<NguyenLieuMonGoi>();
}
