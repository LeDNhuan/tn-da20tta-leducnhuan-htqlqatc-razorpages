using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyQuanAn.Models;
using System.ComponentModel.DataAnnotations;

namespace QuanLyQuanAn.Pages.TNgan.PChi
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

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public double SoChi { get; set; }

            public string GhiChu { get; set; }
        }

        public string userId { get; set; }

        public void OnGet()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "ThuNgan")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                userId = HttpContext.Session.GetString("UserId");
            }
        }

        public void OnPost()
        {
            var ngay = DateTime.Now.Date;
            var userId = HttpContext.Session.GetString("UserId");

            var phieuchi = new PhieuChi
            {
                IdNhanVien = userId,
                SoChi = Input.SoChi,
                GhiChu = Input.GhiChu,
                NgayLap = ngay,

            };

            _context.PhieuChis.Add(phieuchi);
            _context.SaveChanges();

            Response.Redirect("/BanAn");
        }
    }
}
