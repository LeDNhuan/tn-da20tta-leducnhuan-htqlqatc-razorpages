using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using System.Data;

namespace QuanLyQuanAn.Pages.QLChiNhanh.NVien
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly QuanLyQuanAnContext _context;

        public IndexModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _env = environment;
            _context = context;
        }

        public List<NhanVien> nhanviens { get; set; } = new List<NhanVien>();
        public string role { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");
            role = HttpContext.Session.GetString("Role");

            if (role != "QuanLy" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            //else
            //{
            //    nhanviens = _context.NhanViens
            //        .Include(nv => nv.IdVaiTroNavigation)
            //        .ToList();
            //}
        }
    }
}
