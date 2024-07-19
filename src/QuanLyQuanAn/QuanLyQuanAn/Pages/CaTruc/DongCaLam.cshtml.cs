using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using QuanLyQuanAn.Models;
using System.ComponentModel.DataAnnotations;

namespace QuanLyQuanAn.Pages.CaTruc
{
    public class DongCaLamModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;
        public DongCaLamModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public NhanVien NhanVien { get; set; }
        public CaLamViec CaLam { get; set; }
        public string vaitro { get; set; }

        [BindProperty]
        public DongCaLamInputModel Input { get; set; }

        public class DongCaLamInputModel
        {
            [Required]
            public float TienBanGiao { get; set; }            

            public string GhiChu { get; set; }

        }

        public void OnGet()
        {
            var role = HttpContext.Session.GetString("Role");            

            if (role != "NhanVien" && role != "QuanLyTong" && role != "ThuNgan" && role != "QuanLy")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                vaitro = role;
                var userId = HttpContext.Session.GetString("UserId");
                int? shiftId = HttpContext.Session.GetInt32("ShiftId");

                NhanVien = _context.NhanViens.FirstOrDefault(nv => nv.Id == userId);
                CaLam = _context.CaLamViecs.FirstOrDefault(lv => lv.Id == shiftId);

                if (CaLam != null)
                {
                    var startTime = CaLam.ThoiGianBatDau;
                    var endTime = DateTime.Now;

                    // Tính toán tiền trong ca từ các hóa đơn
                    CaLam.TienTrongCa = _context.HoaDons
                        .Where(hd => hd.GioRa >= startTime && hd.GioVao <= endTime)
                        .Sum(hd => hd.TongTien);

                    _context.SaveChanges();
                }
            }
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetString("UserId");
            int? shiftId = HttpContext.Session.GetInt32("ShiftId");


            var CaLamViec = _context.CaLamViecs.FirstOrDefault(lv => lv.Id == shiftId);
            var NhanVien = _context.NhanViens.FirstOrDefault(nv => nv.Id == userId);

            if (CaLamViec != null)
            {
                var TienBanGiao = Input.TienBanGiao;
                var TienChenhLech = ((CaLamViec.TienDauCa ?? 0) + (CaLamViec.TienTrongCa ?? 0)) - TienBanGiao;

                if (TienChenhLech == 0)
                {
                    //var vaitro = HttpContext.Session.GetString("VaiTro");
                    if (string.IsNullOrEmpty(vaitro) && vaitro != "ThuNgan")
                    {
                        CaLamViec.TienTrongCa = 0;
                        CaLamViec.TienBanGiao = 0;
                        CaLamViec.TienChenhLech = 0;
                        CaLamViec.ThoiGianKetThuc = DateTime.Now;
                        CaLamViec.GhiChu = null;

                        
                        _context.SaveChanges();

                        // Xóa ShiftId khỏi session
                        HttpContext.Session.Remove("ShiftId");

                        Response.Redirect("/DangXuat");
                    }
                    else
                    {
                        CaLamViec.TienBanGiao = TienBanGiao;
                        CaLamViec.TienChenhLech = TienChenhLech;
                        CaLamViec.ThoiGianKetThuc = DateTime.Now;
                        CaLamViec.GhiChu = null;

                        _context.SaveChanges();

                        // Xóa ShiftId khỏi session
                        HttpContext.Session.Remove("ShiftId");

                        Response.Redirect("/DangXuat");
                    }
                }
                else
                {
                    ViewData["NhanVien"] = NhanVien?.HoTen;
                    ViewData["TienDauCa"] = CaLamViec?.TienDauCa;
                    ViewData["TienTrongCa"] = CaLamViec?.TienTrongCa;
                    ViewData["TienBanGiao"] = TienBanGiao;
                    ViewData["TienChenhLech"] = TienChenhLech;

                    ModelState.AddModelError("Input.GhiChu", "Ghi chú là bắt buộc khi có tiền chênh lệch.");
                    return Page(); // Hiển thị lại trang với thông báo lỗi
                }                
            }

            // Nếu không tìm thấy ca làm việc, hiển thị lại form với thông báo lỗi
            return Page();
        }

    }
}
