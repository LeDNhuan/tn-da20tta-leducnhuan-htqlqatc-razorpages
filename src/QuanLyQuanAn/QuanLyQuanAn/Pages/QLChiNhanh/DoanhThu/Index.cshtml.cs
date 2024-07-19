using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using System.Data;
using System.Globalization;

namespace QuanLyQuanAn.Pages.QLChiNhanh.DoanhThu
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

        public int branchId { get; set; }

        public void OnGet(int chinhanhId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (role != "QuanLy" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                if(role == "QuanLyTong")
                {
                    var currentBranchId = HttpContext.Session.GetInt32("BranchId");
                    if (currentBranchId != chinhanhId)
                    {
                        HttpContext.Session.SetInt32("BranchId", chinhanhId);
                    }
                }
                
            }
        }
    }
}
