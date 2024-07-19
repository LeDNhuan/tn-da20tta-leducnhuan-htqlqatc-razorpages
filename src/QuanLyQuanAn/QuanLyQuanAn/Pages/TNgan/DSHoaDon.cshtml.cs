using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Pages.TNgan
{
    public class DSHoaDonModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DSHoaDonModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public List<ChiTietGoiMon> ChiTiets { get; set; } = new List<ChiTietGoiMon>();
        public string role { get; set; }


        public void OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");
            role = HttpContext.Session.GetString("Role");

            if (role != "NhanVien" && role != "QuanLyTong" && role != "ThuNgan")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                HoaDons = _context.HoaDons.Where(hd => hd.TrangThai == "ThanhToan").ToList();
                
                //foreach (var hoadon in HoaDons)
                //{
                //    ChiTiets = _context.ChiTietGoiMons.Where(ct => ct.IdHoaDon == hoadon.Id && ct.TrangThai == "Roi").ToList();
                //}
                ChiTiets = _context.ChiTietGoiMons
                    .Where(ct => ct.TrangThai == "Roi")
                    .Include(ct => ct.IdMonGoiNavigation)
                        .ThenInclude(m => m.IdLoaiMonNavigation)
                    .ToList();
            }
        }
    }
}
