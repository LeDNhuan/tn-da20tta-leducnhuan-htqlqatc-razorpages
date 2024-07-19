using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Pages.HoaDonAn
{
    public class DSHoaDonModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DSHoaDonModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public List<ChiTietGoiMon> ChiTiets { get; set; } = new List<ChiTietGoiMon>();
        public string role { get; set; }
        public int branch { get; set; }


        public void OnGet(int chinhanhId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            role = HttpContext.Session.GetString("Role");
            

            if (role != "NhanVien" && role != "QuanLyTong" && role != "ThuNgan" && role != "QuanLy")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                if( role == "QuanLyTong")
                {
                    if (chinhanhId != 0)
                    {
                        HoaDons = _context.HoaDons
                            .Include(hd => hd.IdBanNavigation)
                                .ThenInclude(b => b.IdChiNhanhNavigation)
                            .Where(hd => hd.TrangThai == "ThanhToan" && hd.IdBanNavigation.IdChiNhanh == chinhanhId)
                            .ToList();
                    }
                    else
                    {
                        HoaDons = _context.HoaDons
                            .Include(hd => hd.IdBanNavigation)
                                .ThenInclude(b => b.IdChiNhanhNavigation)
                            .Where(hd => hd.TrangThai == "ThanhToan")
                            .ToList();
                    }
                    

                }
                else
                {
                    branch = (int)HttpContext.Session.GetInt32("BranchId");
                    HoaDons = _context.HoaDons
                        .Include(hd => hd.IdBanNavigation)
                            .ThenInclude(b => b.IdChiNhanhNavigation)
                        .Where(hd => hd.TrangThai == "ThanhToan" && hd.IdBanNavigation.IdChiNhanh == branch)
                        .ToList();
                }
                
                //foreach (var hoadon in HoaDons)
                //{
                //    ChiTiets = _context.ChiTietGoiMons.Where(ct => ct.IdHoaDon == hoadon.Id && ct.TrangThai == "Roi").ToList();
                //}
                ChiTiets = _context.ChiTietGoiMons
                    .Where(ct => ct.TrangThai == "Roi")
                    .Include(ct => ct.IdMonGoiNavigation)
                        .ThenInclude(m => m.IdLoaiMonNavigation)
                    .ToList();
            }
        }

    }
}
