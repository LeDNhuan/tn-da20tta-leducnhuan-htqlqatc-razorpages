using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using QuanLyQuanAn.Extensions;
using QuanLyQuanAn.Models;
using System.Data;

namespace QuanLyQuanAn.Pages.HoaDonAn
{
    public class IndexModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public List<MonGoi> MonAns { get; set; } = new List<MonGoi>();
        public List<LoaiMon> LoaiMons { get; set; } = new List<LoaiMon>();
        public List<ChiTietGoiMon> ChiTiets { get; set; } = new List<ChiTietGoiMon>();
        public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public HoaDon HoaDon { get; set;}
        public int HoaDonId { get; set; }
        public string nvien { get; set; }

        public IndexModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public void OnGet(int hoadonId)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "ThuNgan" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                HoaDonId = hoadonId;

                HoaDon = _context.HoaDons.FirstOrDefault(hd => hd.Id == hoadonId);
                nvien = HttpContext.Session.GetString("UserName");

                if (HoaDon != null)
                {
                    HoaDon.GioRa = DateTime.Now;
                    _context.HoaDons.Update(HoaDon);
                    _context.SaveChanges();
                }


                ChiTiets = _context.ChiTietGoiMons
                    .Where(ct => ct.IdHoaDon == hoadonId)
                    .Include(ct => ct.IdMonGoiNavigation )
                        .ThenInclude(ma => ma.IdLoaiMonNavigation)
                    .ToList();   
            }                     
        }

        public void OnPostThanhToan(int hoadonId)
        {
            if (ModelState.IsValid)
            {
                var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.Id == hoadonId);
                if (hoadon != null)
                {
                    ChiTiets = _context.ChiTietGoiMons
                        .Where(ct => ct.IdHoaDon == hoadonId)
                        .Include(ct => ct.IdMonGoiNavigation)
                            .ThenInclude(ma => ma.IdLoaiMonNavigation)
                        .ToList();

                    double? tonghoahong = 0;
                    foreach (var chitiet in ChiTiets)
                    {
                        var sl = chitiet.SoLuong;
                        var muchh = chitiet.IdMonGoiNavigation.IdLoaiMonNavigation.MucHoaHong / 100;
                        var gia = chitiet.IdMonGoiNavigation.GiaBan;
                        var hoahong = (muchh * gia * sl);
                        tonghoahong += hoahong;
                    }

                    hoadon.TienHoaHong = tonghoahong;
                    hoadon.TrangThai = "ThanhToan";
                    _context.HoaDons.Update(hoadon);
                    _context.SaveChanges();
                    
                }
                Response.Redirect($"/InHoaDon?hoadonId={hoadonId}");
            }            
        }
    }
}
