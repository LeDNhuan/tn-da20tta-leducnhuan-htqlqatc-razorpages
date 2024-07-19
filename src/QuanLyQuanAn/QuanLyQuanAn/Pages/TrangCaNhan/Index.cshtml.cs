using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyQuanAn.Pages.TrangCaNhan
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
        public List<ChiTietGoiMon> ChiTiets { get; set; } = new List<ChiTietGoiMon>();
        public List<CaLamViec> CaLams { get; set; } = new List<CaLamViec>();
        public string vaitro { get; set; }
        public double TongHH { get; set; }
        public NhanVien nhanvien { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu hiện tại")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Nhập lại mật khẩu mới")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
            public string ConfirmNewPassword { get; set; }

            public IFormFile AnhDaiDien { get; set; }

        }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (role != "NhanVien" && role != "QuanLyTong" && role != "ThuNgan" && role != "QuanLy")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                if (role == "NhanVien")
                {
                    vaitro = "Nhân viên";
                }
                else if (role == "ThuNgan")
                {
                    vaitro = "Thu ngân";
                }
                else if (role == "QuanLyTong")
                {
                    vaitro = "Quản lý chung";
                }
                else
                {
                    vaitro = "Quản lý";
                }

                nhanvien = _context.NhanViens
                    .Include(nv => nv.IdVaiTroNavigation)
                    .FirstOrDefault(nv => nv.Id == userId);

                var nvien = _context.NhanViens
                    .Include(nv => nv.CaLamViecs)
                    .FirstOrDefault(nv => nv.Id == userId);

                CaLams = nvien.CaLamViecs
                    .Where(cl => cl.ThoiGianKetThuc.HasValue)
                    .OrderByDescending(cl => cl.ThoiGianKetThuc)
                    .ToList();

                HoaDons = _context.HoaDons.Where(hd => hd.IdNhanVien == userId && hd.TrangThai == "ThanhToan").ToList();
                TongHH = (double)HoaDons.Sum(hd => hd.TienHoaHong);
            }
        }

        public IActionResult OnPost()
        {
            var userId = HttpContext.Session.GetString("UserId");
            nhanvien = _context.NhanViens
                    .Include(nv => nv.IdVaiTroNavigation)
                    .FirstOrDefault(nv => nv.Id == userId);

            var hashPass = ComputeMD5Hash(Input.Password);

            if (nhanvien.MatKhau != hashPass)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu hiện tại không chính xác.");
                return Page();
            }

            nhanvien.MatKhau = ComputeMD5Hash(Input.NewPassword);

            _context.NhanViens.Update(nhanvien);
            _context.SaveChanges();

            ViewData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công.";;

            return Page();
        }

        [HttpPost]
        public void OnPostCapNhatAnh()
        {
            var userId = HttpContext.Session.GetString("UserId");
            nhanvien = _context.NhanViens
                    .FirstOrDefault(nv => nv.Id == userId);

            if (nhanvien == null)
            {
                // Xử lý trường hợp không tìm thấy món ăn với monanId
                Response.Redirect("/CaNhan");
            }

            // Xử lý việc lưu trữ ảnh đại diện mới (nếu có)
            if (Input.AnhDaiDien != null)
            {
                // Lưu ảnh mới vào thư mục "wwwroot/uploads" hoặc thư mục mong muốn
                var uploadsFolder = Path.Combine(_env.WebRootPath, "customize/img/");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Input.AnhDaiDien.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Input.AnhDaiDien.CopyTo(fileStream);
                }

                // Xóa ảnh cũ nếu không phải là ảnh mặc định
                if (!string.IsNullOrEmpty(nhanvien.AnhDaiDien) && nhanvien.AnhDaiDien != "/customize/img/default_avatar.png")
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, nhanvien.AnhDaiDien.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                nhanvien.AnhDaiDien = $"/customize/img/{uniqueFileName}";
            }            

            _context.NhanViens.Update(nhanvien);
            _context.SaveChanges();

            Response.Redirect("/CaNhan");
        }

        private string ComputeMD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
