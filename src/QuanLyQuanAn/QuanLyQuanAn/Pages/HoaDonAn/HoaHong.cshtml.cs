using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using QuanLyQuanAn.ViewModel;

namespace QuanLyQuanAn.Pages.HoaDonAn
{
    public class HoaHongModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public HoaHongModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }
        public List<ChiTietGoiMon> ChiTietChuas { get; set; } = new List<ChiTietGoiMon>();
        public List<CaLamViec> CaLams { get; set; } = new List<CaLamViec>();
        public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public HoaDon HoaDon { get; set; }
        public double TongHH { get; set; }
        public string role { get; set; }

        public void OnGet()
        {

            role = HttpContext.Session.GetString("Role");

            if (role != "NhanVien" && role != "QuanLyTong" && role != "ThuNgan")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                var userId = HttpContext.Session.GetString("UserId");
                var nhanvien = _context.NhanViens
                    .Include(nv => nv.CaLamViecs)
                    .FirstOrDefault(nv => nv.Id == userId);

                CaLams = nhanvien.CaLamViecs
                    .Where(cl => cl.ThoiGianKetThuc.HasValue)
                    .OrderByDescending(cl => cl.ThoiGianKetThuc)
                    .ToList();

                HoaDons = _context.HoaDons.Where(hd => hd.IdNhanVien == userId && hd.TrangThai == "ThanhToan").ToList();
                TongHH = (double)HoaDons.Sum(hd => hd.TienHoaHong);
            }
        }
    }
}
