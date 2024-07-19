using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using System.Data;

namespace QuanLyQuanAn.Pages.TNgan.DoanhThu
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

        public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public string role { get; set; }
        public int branch { get; set; }
        public double profit { get; set; }
        public CaLamViec CaLam { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");
            role = HttpContext.Session.GetString("Role");
            if (role != "QuanLyTong" && role != "ThuNgan" && role != "QuanLy")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                branch = (int)HttpContext.Session.GetInt32("BranchId");
                var nvien = _context.NhanViens
                    .Include(nv => nv.CaLamViecs)
                    .FirstOrDefault(nv => nv.Id == userId);

                if (nvien != null)
                {
                    var today = DateTime.Now;

                    CaLam = nvien.CaLamViecs.FirstOrDefault(c => c.ThoiGianBatDau.HasValue && c.ThoiGianBatDau.Value.Date == today.Date && !c.ThoiGianKetThuc.HasValue);


                    HoaDons = _context.HoaDons
                        .Include(hd => hd.IdBanNavigation)
                            .ThenInclude(b => b.IdChiNhanhNavigation)
                        .Where(hd => hd.GioVao.Value.Date == today.Date)
                        .ToList();

                    profit = (double)HoaDons.Sum(hd => hd.TongTien);
                }
                
                
            }
        }
    }
}



