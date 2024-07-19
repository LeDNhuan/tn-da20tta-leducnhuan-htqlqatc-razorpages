using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using QuanLyQuanAn.ViewModel;
using System.Data;

namespace QuanLyQuanAn.Pages.QuanLyChiNhanh
{
    public class IndexModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public IndexModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public List<MonGoi> MonAns = new List<MonGoi>();
        public List<HoaDon> HoaDons = new List<HoaDon>();

        public List<HoaDon> HoaDonChuaTTHomNays = new List<HoaDon>();
        public List<HoaDon> HoaDonHomNays = new List<HoaDon>();
        public List<HoaDon> HoaDonHomQuas = new List<HoaDon>();
        public List<KhachHang> KhachHangHomNays = new List<KhachHang>();
        public List<KhachHang> KhachHangHomQuas = new List<KhachHang>();

        public List<ChiTietGoiMon> ChiTiets = new List<ChiTietGoiMon>();
        public List<ChiTietGoiMon> topmon = new List<ChiTietGoiMon>();

        public string role { get; set; }
        public double tongtienhomnay { get; set; }
        public double tongtienhomqua { get; set; }
        public double phantramdoanhthu { get; set; }
        public double hoadonhomnay { get; set; }
        public double hoadonhomqua { get; set; }
        public double phantramhoadon { get; set; }
        public double khachhanghomnay { get; set; }
        public double khachhanghomqua { get; set; }
        public double phantramkhachhang { get; set; }
        public DateTime today { get; set; }
        public DateTime yesterday { get; set; }

        public class MonAnBanChay
        {
            public MonGoi MonAn { get; set; }
            public int TongSL { get; set; }
        }

        public List<MonAnBanChay> top5monan = new List<MonAnBanChay>();

        public void OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");
            role = HttpContext.Session.GetString("Role");

            if (role != "QuanLy" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                
                MonAns = _context.MonGois
                    .Include(ma => ma.IdLoaiMonNavigation)
                    .ToList();

                HoaDons = _context.HoaDons.ToList();
                
                today = DateTime.Now.Date;
                yesterday = today.AddDays(-1);

                HoaDonHomNays = _context.HoaDons
                    .Where(hd => hd.GioVao.HasValue && hd.GioVao.Value.Date == today)
                    .ToList();

                HoaDonHomQuas = _context.HoaDons
                    .Where(hd => hd.GioVao.HasValue && hd.GioVao.Value.Date == yesterday)
                    .ToList();

                KhachHangHomNays = _context.KhachHangs
                    .Where(hd => hd.CreatedAt.HasValue && hd.CreatedAt.Value == today)
                    .ToList();

                KhachHangHomQuas = _context.KhachHangs
                    .Where(hd => hd.CreatedAt.HasValue && hd.CreatedAt.Value == yesterday)
                    .ToList();


                //--------------------Tính hóa đơn------------------------
                hoadonhomnay = HoaDonHomNays.Count();
                hoadonhomqua = HoaDonHomQuas.Count();
                phantramhoadon = 0;
                if (hoadonhomqua != 0)
                {
                    phantramhoadon = ((hoadonhomnay - hoadonhomqua) / hoadonhomqua) * 100;
                }


                //----------------------Tính doanh thu---------------------------
                tongtienhomnay = HoaDonHomNays.Sum(hd => hd.TongTien) ?? 0;
                tongtienhomqua = HoaDonHomQuas.Sum(hd => hd.TongTien) ?? 0;
                phantramdoanhthu = 0;
                if (tongtienhomqua != 0)
                {
                    phantramdoanhthu = ((tongtienhomnay - tongtienhomqua) / tongtienhomqua) * 100;
                }


                //----------------------Tính khách hàng-------------------------------
                khachhanghomnay = KhachHangHomNays.Count();
                khachhanghomqua = KhachHangHomQuas.Count();
                phantramkhachhang = 0;
                if (khachhanghomqua != 0)
                {
                    phantramkhachhang = ((khachhanghomnay - khachhanghomqua) / khachhanghomqua) * 100;
                }


                //-----------------------Dữ liệu biểu đồ---------------------------
                var lastweek = today.AddDays(-6);

                // Lấy dữ liệu hóa đơn trong vòng 7 ngày
                var HoaDon7Ngay = _context.HoaDons
                    .Where(hd => hd.GioVao.HasValue && hd.GioVao.Value.Date >= lastweek && hd.GioVao.Value.Date <= today)
                    .GroupBy(hd => hd.GioVao.Value.Date)
                    .Select(g => new {
                        Date = g.Key,
                        Count = g.Count(),
                        Total = g.Sum(hd => hd.TongTien) ?? 0
                    })
                    .ToList();

                // Lấy dữ liệu khách hàng trong vòng 7 ngày
                var KhachHang7Ngay = _context.KhachHangs
                    .Where(kh => kh.CreatedAt.HasValue && kh.CreatedAt.Value.Date >= lastweek && kh.CreatedAt.Value.Date <= today)
                    .GroupBy(kh => kh.CreatedAt.Value.Date)
                    .Select(g => new {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .ToList();

                // Chuẩn bị dữ liệu cho ViewBag
                var dates = Enumerable.Range(0, 7).Select(i => lastweek.AddDays(i)).ToList();
                var hoadonData = dates.Select(d => HoaDon7Ngay.FirstOrDefault(h => h.Date == d)?.Count ?? 0).ToList();
                var doanhthuData = dates.Select(d => HoaDon7Ngay.FirstOrDefault(h => h.Date == d)?.Total ?? 0).ToList();
                var khachhangData = dates.Select(d => KhachHang7Ngay.FirstOrDefault(k => k.Date == d)?.Count ?? 0).ToList();

                ViewData["Dates"] = dates.Select(d => d.ToString("yyyy-MM-dd")).ToList();
                ViewData["HoaDonData"]  = hoadonData;
                ViewData["DoanhThuData"] = doanhthuData;
                ViewData["KhachHangData"] = khachhangData;


                //---------------Top 5 bán chạy--------------
                var top5MonGoi = _context.ChiTietGoiMons
                    .Where(ct => ct.SoLuong.HasValue)
                    .GroupBy(ct => ct.IdMonGoi)
                    .Select(group => new TopMonGoiViewModel
                    {
                        IdMonGoi = group.Key.Value,
                        SoLuong = group.Sum(ct => ct.SoLuong)
                    })
                    .OrderByDescending(g => g.SoLuong)
                    .Take(5)
                    .ToList();

                ViewData["Top5MonGoi"] = top5MonGoi;
            }
        }        
    }
}
