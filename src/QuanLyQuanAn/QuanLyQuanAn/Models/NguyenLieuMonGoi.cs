using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class NguyenLieuMonGoi
{
    public int NguyenLieuId { get; set; }

    public int MonGoiId { get; set; }

    public int? SoLuong { get; set; }

    public virtual MonGoi MonGoi { get; set; }

    public virtual NguyenLieu NguyenLieu { get; set; }
}
