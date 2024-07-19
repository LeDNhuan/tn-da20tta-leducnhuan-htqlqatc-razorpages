using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyQuanAn.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyQuanAn.Pages.TKhoan
{
    public class DangKyModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly QuanLyQuanAnContext _context;

        public DangKyModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _env = environment;
            _context = context;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Bạn chưa nhập tên đầy đủ!")]
            [Display(Name = "Họ và tên")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Bạn chưa chọn giới tính!")]
            [Display(Name = "Giới tính")]
            public string Sex { get; set; }


            [Required(ErrorMessage = "Bạn chưa nhập số CCCD!")]
            [Display(Name = "Số căn cước công dân")]
            public string IdenNum { get; set; }

            [Required(ErrorMessage = "Bạn chưa chọn ngày sinh!")]
            [DataType(DataType.Date)]
            [Display(Name = "Ngày sinh")]
            public DateOnly DateOfBirth { get; set; }

            [Required(ErrorMessage = "Bạn chưa nhập địa chỉ!")]
            [Display(Name = "Địa chỉ")]
            public string Address { get; set; }

            [Required(ErrorMessage = "Bạn chưa nhập số ĐT!")]
            [Display(Name = "Số ĐT")]
            public string PhoneNum { get; set; }

            [Required(ErrorMessage = "Bạn chưa nhập gmail!")]
            [EmailAddress]
            [Display(Name = "Gmail")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Bạn chưa nhập tên đăng nhập!")]
            [Display(Name = "Tên đăng nhập")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Bạn chưa nhập mật khẩu!")]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Bạn chưa xác nhận mật khẩu!")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
            [Display(Name = "Xác nhận mật khẩu")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "ID người giới thiệu")]
            public string ReferrerId { get; set; }

            [Required(ErrorMessage = "Bạn chưa đồng ý điều khoản.")]
            [Display(Name = "Tôi đã đọc và đồng ý các điều khoản")]
            public bool AcceptTerms { get; set; }

            [Display(Name = "Ảnh đại diện")]
            public IFormFile ProfileImage { get; set; }

        }

        public void OnPost()
        {
            // Tạo ID khách hàng ngẫu nhiên gồm 6 chữ số
            var randomId = GenerateUniqueRandomId();

            // Mã hóa mật khẩu bằng MD5
            var hashedPassword = ComputeMD5Hash(Input.Password);

            // Xử lý việc lưu trữ ảnh đại diện
            string profileImagePath;
            if (Input.ProfileImage != null)
            {
                // Lưu ảnh vào thư mục "wwwroot/uploads" hoặc thư mục mong muốn
                var uploadsFolder = Path.Combine(_env.WebRootPath, "customize/img/");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Input.ProfileImage.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Input.ProfileImage.CopyTo(fileStream);
                }

                profileImagePath = $"/customize/img/{uniqueFileName}";
            }
            else
            {
                // Nếu không tải lên ảnh, sử dụng ảnh mặc định
                profileImagePath = "/customize/img/default_avatar.png"; // Đường dẫn ảnh mặc định trong dự án
            }

            // Tạo mới khách hàng
            var customer = new KhachHang
            {
                Id = randomId,
                HoTen = Input.FullName,
                GioiTinh = Input.Sex,
                Cccd = Input.IdenNum,
                NgaySinh = Input.DateOfBirth,
                DiaChi = Input.Address,
                Sdt = Input.PhoneNum,
                Gmail = Input.Email,
                NguoiGioiThieu = Input.ReferrerId ?? "",
                CreatedAt = DateTime.Now
            };

            //var staff = new NhanVien
            //{
            //    Id = randomId,
            //    HoTen = Input.FullName,
            //    GioiTinh = Input.Sex,
            //    Cccd = Input.IdenNum,
            //    NgaySinh = Input.DateOfBirth,
            //    DiaChi = Input.Address,
            //    Sdt = Input.PhoneNum,
            //    Gmail = Input.Email,
            //    MatKhau = hashedPassword,
            //    AnhDaiDien = profileImagePath,
            //    //NguoiGioiThieu = Input.ReferrerId ?? "",
            //    CreatedAt = DateTime.Now
            //};

            _context.KhachHangs.Add(customer);
            _context.SaveChanges();

            //var result = await _userManager.CreateAsync(user);

            //if (result.Succeeded)
            //{
            //    await _signInManager.SignInAsync(user, isPersistent: false);
            //    return RedirectToPage("/MoCaLam");
            //}

            //foreach (var error in result.Errors)
            //{
            //    ModelState.AddModelError(string.Empty, error.Description);
            //}
            Response.Redirect("/DangKy");

        }

        // Tạo ID khách hàng ngẫu nhiên gồm 6 chữ số không trùng lặp
        private string GenerateUniqueRandomId()
        {
            var random = new Random();
            string randomId;
            bool exists;

            do
            {
                randomId = random.Next(0, 1000000).ToString("D6");
                exists = _context.KhachHangs.Any(kh => kh.Id == randomId);
            } while (exists);

            return randomId;
        }

        // Mã hóa mật khẩu
        private string ComputeMD5Hash(string input)
        {
            // Sử dụng MD5 để mã hóa mật khẩu
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Chuyển đổi mảng byte thành chuỗi hexa
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
