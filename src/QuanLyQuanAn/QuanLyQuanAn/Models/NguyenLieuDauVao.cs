using System;
using System.Collections.Generic;

namespace QuanLyQuanAn.Models;

public partial class NguyenLieuDauVao
{
    public int Id { get; set; }

    public string TenNguyenLieuDauVao { get; set; }

    public int? SoLuong { get; set; }

    public string AnhDaiDien { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChiNhanhNguyenLieuDauVao> ChiNhanhNguyenLieuDauVaos { get; set; } = new List<ChiNhanhNguyenLieuDauVao>();
}
