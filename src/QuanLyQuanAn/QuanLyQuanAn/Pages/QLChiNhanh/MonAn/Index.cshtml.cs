using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;


namespace QuanLyQuanAn.Pages.QuanLyChiNhanh.MonAn
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

        public List<LoaiMon> LoaiMons { get; set; } = new List<LoaiMon>();
        public List<MonGoi> MonAns { get; set; } = new List<MonGoi>();
        public List<NguyenLieu> NguyenLieus { get; set; } = new List<NguyenLieu>();
        public List<NguyenLieuMonGoi> NguyenLieuMonGois { get; set; } = new List<NguyenLieuMonGoi>();
        public int? IdChiNhanh { get; set; }

        public void OnGet(int chinhanhId)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "QuanLy" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            { 
                if(role == "QuanLyTong")
                {
                    IdChiNhanh = chinhanhId;
                    if (chinhanhId != 0)
                    {
                        LoaiMons = _context.LoaiMons.ToList();

                        MonAns = _context.MonGois
                            .Include(ma => ma.NguyenLieuMonGois)
                                .ThenInclude(nl => nl.NguyenLieu)
                            .Include(ma => ma.IdChiNhanhNavigation)
                            .Where(ma => ma.IdChiNhanh == chinhanhId)
                            .ToList();

                        NguyenLieuMonGois = _context.NguyenLieuMonGois.ToList();
                        NguyenLieus = _context.NguyenLieus.ToList();
                    }
                    else
                    {
                        LoaiMons = _context.LoaiMons.ToList();

                        MonAns = _context.MonGois
                            .Include(ma => ma.NguyenLieuMonGois)
                                .ThenInclude(nl => nl.NguyenLieu)
                            .Include(ma => ma.IdChiNhanhNavigation)                            
                            .ToList();

                        NguyenLieuMonGois = _context.NguyenLieuMonGois.ToList();
                        NguyenLieus = _context.NguyenLieus.ToList();
                    }
                    

                }
                else
                {
                    var branch = HttpContext.Session.GetInt32("BranchId");
                    IdChiNhanh = branch;
                    LoaiMons = _context.LoaiMons.ToList();

                    MonAns = _context.MonGois
                        .Include(ma => ma.NguyenLieuMonGois)
                            .ThenInclude(nl => nl.NguyenLieu)
                        .Include(ma => ma.IdChiNhanhNavigation)
                        .Where(ma => ma.IdChiNhanh == branch)
                        .ToList();

                    NguyenLieuMonGois = _context.NguyenLieuMonGois.ToList();
                    NguyenLieus = _context.NguyenLieus.ToList();
                }
                

                //var monan = _context.MonGois
                //    .Include(ma => ma.NguyenLieuMonGois)
                //    .FirstOrDefault(ma => ma.Id == userId);
            }                
        }

        public void OnPostXoaMonAn(int monanId)
        {
            var monan = _context.MonGois.Include(m => m.NguyenLieuMonGois)
                                        .FirstOrDefault(m => m.Id == monanId);

            if (monan == null)
            {
                Response.Redirect("/QLMonAn");
            }

            // Xóa toàn bộ nguyên liệu liên quan trước khi xóa món ăn
            foreach (var nguyenlieumongoi in monan.NguyenLieuMonGois.ToList())
            {
                _context.NguyenLieuMonGois.Remove(nguyenlieumongoi);
            }

            _context.MonGois.Remove(monan);
            _context.SaveChanges();

            Response.Redirect("/QLMonAn");
        }
    }
}
