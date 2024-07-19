using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class LoaiMon
{
    public int Id { get; set; }

    public string TenLoaiMon { get; set; }

    public double? MucHoaHong { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<MonGoi> MonGois { get; set; } = new List<MonGoi>();
}
