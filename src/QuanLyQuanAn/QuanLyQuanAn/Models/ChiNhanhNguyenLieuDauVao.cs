using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class ChiNhanhNguyenLieuDauVao
{
    public int ChiNhanhId { get; set; }

    public int NguyenLieuDauVaoId { get; set; }

    public int? SoLuong { get; set; }

    public DateTime? NgayNhap { get; set; }

    public virtual ChiNhanh ChiNhanh { get; set; }

    public virtual NguyenLieuDauVao NguyenLieuDauVao { get; set; }
}
