using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Pages.CaTruc
{
    public class MoCaLamModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;
        public MoCaLamModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        [BindProperty]
        public float TienDauCa { get; set; }
        public string IdNhanVien { get; set; }
        public NhanVien NhanVien { get; set; }
        public CaLamViec CaLam { get; set; }


        public void OnGet()
        {
            var role = HttpContext.Session.GetString("Role");
            var userId = HttpContext.Session.GetString("UserId");

            if (role != "ThuNgan" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                IdNhanVien = userId;

                NhanVien = _context.NhanViens.FirstOrDefault(nv => nv.Id == IdNhanVien);
                
            }
        }

        public IActionResult OnPost()
        {
            var role = HttpContext.Session.GetString("Role");
            

            if (role != "ThuNgan" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (ModelState.IsValid)
                {
                    // Tạo một đối tượng CaLamViec mới
                    var CaLamViec = new CaLamViec
                    {
                        
                        TienDauCa = TienDauCa,
                        ThoiGianBatDau = DateTime.Now,
                    };

                    // Lấy đối tượng NhanVien từ DbContext
                    var nhanvien = _context.NhanViens.FirstOrDefault(nv => nv.Id == userId);

                    if (nhanvien != null)
                    {
                        // Thêm đối tượng NhanVien vào CaLamViec
                        CaLamViec.NhanViens.Add(nhanvien);

                        // Thêm đối tượng CaLamViec mới vào DbContext
                        _context.CaLamViecs.Add(CaLamViec);
                        _context.SaveChanges();

                        // Lưu ID của CaLamViec vào session
                        HttpContext.Session.SetInt32("ShiftId", CaLamViec.Id);

                        // Chuyển hướng đến trang thành công hoặc trang khác tùy thuộc vào yêu cầu
                        Response.Redirect("/CaNhan");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy nhân viên.");
                    }                    
                }
            }          

            // Nếu không hợp lệ, hiển thị lại form với thông báo lỗi
            return Page();
        }
    }
}
