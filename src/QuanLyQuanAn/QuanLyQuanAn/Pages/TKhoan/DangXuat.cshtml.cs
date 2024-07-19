using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Pages.TKhoan
{
    public class DangXuatModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;
        public DangXuatModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public NhanVien NhanVien { get; set; }
        public CaLamViec CaLam { get; set; }

        public void OnGet()
        {          

            HttpContext.Session.Clear(); // Xóa tất cả session

            Response.Redirect("/");
        }
    }
}
