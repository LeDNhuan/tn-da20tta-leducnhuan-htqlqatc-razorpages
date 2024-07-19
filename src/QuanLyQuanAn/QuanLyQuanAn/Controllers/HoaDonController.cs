using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers
{
    public class HoaDonController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly QuanLyQuanAnContext _context;

        public HoaDonController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _env = environment;
            _context = context;
        }

        public List<ChiTietGoiMon> ChiTiets { get; set; } = new List<ChiTietGoiMon>();


        [HttpGet]
        public IActionResult Details(int id)
        {
            var chitiets = _context.ChiTietGoiMons
                .Where(ct => ct.IdHoaDon == id)
                .Select(ct => new
                {
                    LoaiMon = ct.IdMonGoiNavigation.IdLoaiMonNavigation.TenLoaiMon,
                    TenMon = ct.IdMonGoiNavigation.TenMon,
                    Gia = ct.IdMonGoiNavigation.GiaBan,
                    SoLuong = ct.SoLuong,
                    ThanhTien = ct.ThanhTien
                })
                .ToList();

            return Json(chitiets);
        }

        
    }
}
