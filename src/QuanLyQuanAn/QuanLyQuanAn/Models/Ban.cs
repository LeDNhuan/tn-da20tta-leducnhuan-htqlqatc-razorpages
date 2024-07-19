using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class Ban
{
    public int Id { get; set; }

    public string TinhTrang { get; set; }

    public string LoaiBan { get; set; }

    public int? IdChiNhanh { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ChiNhanh IdChiNhanhNavigation { get; set; }
}
