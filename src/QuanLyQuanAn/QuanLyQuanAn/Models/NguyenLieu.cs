using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class NguyenLieu
{
    public int Id { get; set; }

    public string TenNguyenLieu { get; set; }

    public int? SoLuong { get; set; }

    public string AnhDaiDien { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<NguyenLieuMonGoi> NguyenLieuMonGois { get; set; } = new List<NguyenLieuMonGoi>();
}
