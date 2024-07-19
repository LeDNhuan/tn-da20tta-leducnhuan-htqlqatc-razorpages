
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using System.Globalization;

namespace QuanLyQuanAn.Controllers
{
    public class DoanhThuController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DoanhThuController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }


        //[HttpGet]
        //public JsonResult GetChartDataByDate(string date)
        //{
        //    // Chuyển đổi date từ định dạng dd/MM/yyyy sang định dạng DateTime
        //    DateTime selectedDate = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        //    var hoadonData = _context.HoaDons
        //        .Where(hd => hd.GioVao.Value.Date == selectedDate.Date)
        //        .Select(hd => new { hd.Id, hd.TongTien, hd.TienHoaHong, hd.GioVao })
        //        .ToList();

        //    var doanhthuData = hoadonData.Select(hd => hd.TongTien).ToList();
        //    var khachhangData = hoadonData.GroupBy(hd => hd.GioVao.Value.Hour).Select(g => g.Count()).ToList();
        //    var dates = hoadonData.Select(hd => hd.GioVao.Value.ToString("dd/MM/yyyy HH:mm")).ToList();

        //    var data = new
        //    {
        //        hoadonData = hoadonData.Select(hd => hd.TongTien).ToList(),
        //        doanhthuData = doanhthuData,
        //        khachhangData = khachhangData,
        //        dates = dates
        //    };

        //    return Json(data);
        //}

        [HttpGet]
        public JsonResult GetChartDataByDate(string date)
        {
            try
            {
                var branchId = HttpContext.Session.GetInt32("BranchId");
                DateTime selectedDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                if (branchId == 0)
                {
                    var hoadonData = _context.HoaDons
                        //.Include(hd => hd.IdBanNavigation)
                        .Where(hd => hd.GioVao.Value.Date == selectedDate.Date)
                        .Select(hd => new { hd.Id, hd.TongTien, hd.GioVao })
                        .OrderBy(hd => hd.GioVao)
                        .ToList();

                    var khachhangData = _context.KhachHangs
                    .Where(kh => kh.CreatedAt == selectedDate.Date)
                    .ToList();

                    var doanhthuData = hoadonData.Select(hd => hd.TongTien).ToList();
                    var hoadonCount = hoadonData.Count;
                    var khachhangCount = khachhangData.Count();
                    var dates = hoadonData.Select(hd => hd.GioVao.Value.ToString("yyyy-MM-dd HH:mm:ss")).ToList();

                    var data = new
                    {
                        hoadonCount = hoadonCount,
                        doanhthuData = doanhthuData,
                        khachhangCount = khachhangCount,
                        dates = dates
                    };

                    return Json(data);
                }
                else
                {
                    var hoadonData = _context.HoaDons
                        //.Include(hd => hd.IdBanNavigation)
                        .Where(hd => hd.GioVao.Value.Date == selectedDate.Date && hd.IdBanNavigation.IdChiNhanh == branchId)
                        .Select(hd => new { hd.Id, hd.TongTien, hd.GioVao })
                        .OrderBy(hd => hd.GioVao)
                        .ToList();

                    var khachhangData = _context.KhachHangs
                    .Where(kh => kh.CreatedAt == selectedDate.Date)
                    .ToList();

                    var doanhthuData = hoadonData.Select(hd => hd.TongTien).ToList();
                    var hoadonCount = hoadonData.Count;
                    var khachhangCount = khachhangData.Count();
                    var dates = hoadonData.Select(hd => hd.GioVao.Value.ToString("yyyy-MM-dd HH:mm:ss")).ToList();

                    var data = new
                    {
                        hoadonCount = hoadonCount,
                        doanhthuData = doanhthuData,
                        khachhangCount = khachhangCount,
                        dates = dates
                    };

                    return Json(data);
                }                

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetChartDataByDate: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }


        
        [HttpGet]
        public JsonResult GetMonthlyRevenue()
        {
            try
            {
                var branchId = HttpContext.Session.GetInt32("BranchId");

                if (branchId == 0)
                {
                    var monthlyRevenue = Enumerable.Range(1, 12).Select(month => new
                    {
                        Year = DateTime.Now.Year,
                        Month = month,
                        TotalInvoices = _context.HoaDons.Count(hd => hd.GioVao.Value.Month == month && hd.GioVao.Value.Year == DateTime.Now.Year),
                        TotalRevenue = _context.HoaDons.Where(hd => hd.GioVao.Value.Month == month && hd.GioVao.Value.Year == DateTime.Now.Year).Sum(hd => (decimal?)hd.TongTien) ?? 0,
                        TotalCustomers = _context.KhachHangs.Count(kh => kh.CreatedAt.Value.Month == month && kh.CreatedAt.Value.Year == DateTime.Now.Year)
                    }).ToList();

                    return Json(monthlyRevenue);
                }
                else
                {
                    var monthlyRevenue = Enumerable.Range(1, 12).Select(month => new
                    {
                        Year = DateTime.Now.Year,
                        Month = month,
                        TotalInvoices = _context.HoaDons.Count(hd => hd.GioVao.Value.Month == month && hd.GioVao.Value.Year == DateTime.Now.Year && hd.IdBanNavigation.IdChiNhanh == branchId),
                        TotalRevenue = _context.HoaDons.Where(hd => hd.GioVao.Value.Month == month && hd.GioVao.Value.Year == DateTime.Now.Year && hd.IdBanNavigation.IdChiNhanh == branchId).Sum(hd => (decimal?)hd.TongTien) ?? 0,
                        TotalCustomers = _context.KhachHangs.Count(kh => kh.CreatedAt.Value.Month == month && kh.CreatedAt.Value.Year == DateTime.Now.Year)
                    }).ToList();

                    return Json(monthlyRevenue);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetMonthlyRevenue: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetYearlyRevenue()
        {
            try
            {
                var branchId = HttpContext.Session.GetInt32("BranchId");

                if (branchId == 0)
                {
                    var yearlyRevenue = _context.HoaDons
                    .GroupBy(hd => hd.GioVao.Value.Year)
                    .Select(g => new
                    {
                        Year = g.Key,
                        TotalInvoices = g.Count(),
                        TotalRevenue = g.Sum(hd => (decimal?)hd.TongTien) ?? 0,
                        TotalCustomers = _context.KhachHangs.Count(kh => kh.CreatedAt.Value.Year == g.Key)
                    })
                    .OrderBy(g => g.Year)
                    .ToList();

                    return Json(yearlyRevenue);
                }
                else
                {
                    var yearlyRevenue = _context.HoaDons
                    .Where(hd => hd.IdBanNavigation.IdChiNhanh == branchId)
                    .GroupBy(hd => hd.GioVao.Value.Year)
                    .Select(g => new
                    {
                        Year = g.Key,
                        TotalInvoices = g.Count(),
                        TotalRevenue = g.Sum(hd => (decimal?)hd.TongTien) ?? 0,
                        TotalCustomers = _context.KhachHangs.Count(kh => kh.CreatedAt.Value.Year == g.Key)
                    })
                    .OrderBy(g => g.Year)
                    .ToList();

                    return Json(yearlyRevenue);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetYearlyRevenue: " + ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
