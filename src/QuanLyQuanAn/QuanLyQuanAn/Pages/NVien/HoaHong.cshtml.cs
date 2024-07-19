using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using QuanLyQuanAn.ViewModel;

namespace QuanLyQuanAn.Pages.NVien
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

        public class CaLamViecViewModel
        {
            public DateTime? ThoiGianBatDau { get; set; }
            public DateTime? ThoiGianKetThuc { get; set; }
            public List<HoaDon> HoaDons { get; set; }
        }

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


                //CaLams = _context.CaLamViecs
                //    .Where(cl => cl.NhanViens.Contains(nhanvien))
                //    .ToList();

                CaLams = nhanvien.CaLamViecs
                    .Where(cl => cl.ThoiGianKetThuc.HasValue)
                    .OrderByDescending(cl => cl.ThoiGianKetThuc)
                    .ToList();

                //var CaLamViecViewModels = new List<CaLamViecViewModel>();


                //foreach (var calam in CaLams)
                //{
                //    var startTime = calam.ThoiGianBatDau;
                //    var endTime = calam.ThoiGianKetThuc;

                //    HoaDons = _context.HoaDons
                //        .Where(hd => hd.IdNhanVien == userId && hd.TrangThai == "ThanhToan" && hd.GioRa >= startTime && hd.GioVao <= endTime)
                //        .ToList();

                //    CaLamViecViewModels.Add(new CaLamViecViewModel
                //    {
                //        ThoiGianBatDau = startTime,
                //        ThoiGianKetThuc = endTime,
                //        HoaDons = HoaDons
                //    });

                //}

                HoaDons = _context.HoaDons.Where(hd => hd.IdNhanVien == userId && hd.TrangThai == "ThanhToan").ToList();
                TongHH = (double)HoaDons.Sum(hd => hd.TienHoaHong);
            }
        }
    }
}
