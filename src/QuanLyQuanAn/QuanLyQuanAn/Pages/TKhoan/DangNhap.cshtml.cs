using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyQuanAn.Pages.TKhoan
{
    public class DangNhapModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly QuanLyQuanAnContext _context;

        public DangNhapModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _env = environment;
            _context = context;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public class LoginInputModel
        {
            [Required]
            public string Phonenum { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            
            public string Role { get; set; }

            public int? BranchId { get; set; }
        }


        public IActionResult OnPost()
        {
            var user = _context.NhanViens
                .Include(u => u.IdVaiTroNavigation)
                .FirstOrDefault(u =>  u.Sdt == Input.Phonenum &&
                                      u.MatKhau == ComputeMD5Hash(Input.Password) &&
                                      u.Id != null);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Số điện thoại hoặc mật khẩu không đúng.");
                return Page();
            }
            else
            {
                if (user.IdVaiTroNavigation.TenVaiTro == "ThuNgan")
                {
                    HttpContext.Session.SetString("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.HoTen);
                    HttpContext.Session.SetString("Role", user.IdVaiTroNavigation.TenVaiTro);
                    HttpContext.Session.SetInt32("BranchId", (int)user.IdChiNhanh);
                    Response.Redirect("/MoCaLam");
                }                
                else if (user.IdVaiTroNavigation.TenVaiTro == "QuanLyTong")
                {
                    //var nhanvien = _context.NhanViens.FirstOrDefault(nv => nv.Id == user.IdNhanVien);

                    HttpContext.Session.SetString("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.HoTen);
                    HttpContext.Session.SetString("Role", user.IdVaiTroNavigation.TenVaiTro);
                    //HttpContext.Session.SetInt32("BranchId", (int)user.IdChiNhanh);

                    // Tạo một đối tượng CaLamViec mới
                    var CaLamViec = new CaLamViec
                    {

                        TienDauCa = 0,
                        ThoiGianBatDau = DateTime.Now,
                    };

                    // Lấy đối tượng NhanVien từ DbContext
                    var nhanvien = _context.NhanViens.FirstOrDefault(nv => nv.Id == user.Id);

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
                }
                else
                {
                    //var nhanvien = _context.NhanViens.FirstOrDefault(nv => nv.Id == user.IdNhanVien);

                    HttpContext.Session.SetString("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", user.HoTen);
                    HttpContext.Session.SetString("Role", user.IdVaiTroNavigation.TenVaiTro);
                    HttpContext.Session.SetInt32("BranchId", (int)user.IdChiNhanh);

                    // Tạo một đối tượng CaLamViec mới
                    var CaLamViec = new CaLamViec
                    {

                        TienDauCa = 0,
                        ThoiGianBatDau = DateTime.Now,
                    };

                    // Lấy đối tượng NhanVien từ DbContext
                    var nhanvien = _context.NhanViens.FirstOrDefault(nv => nv.Id == user.Id);

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
                }
            }

            

            //    if (user.VaiTro == "staff"))
            //    {
            //        // Kiểm tra đăng nhập cho nhân viên
            //        var staff = _context.TaiKhoans
            //            .FirstOrDefault(u => u.TenDangNhap == Input.Username &&
            //                                 u.MatKhau == Input.Password &&
            //                                 //u.MatKhau == ComputeMD5Hash(Input.Password) &&
            //                                 u.IdNhanVien != null);


            //        if (staff != null)
            //        {
            //            // Kiểm tra xem nhân viên có đúng chi nhánh không
            //            var nhanvien = _context.NhanViens.FirstOrDefault(nv => nv.Id == staff.IdNhanVien && nv.IdChiNhanh == Input.BranchId);

            //            if (nhanvien == null)
            //            {
            //                ModelState.AddModelError(string.Empty, "Nhân viên không thuộc chi nhánh đã chọn.");
            //                return Page();
            //            }

            //            // Gán vai trò phù hợp dựa trên thông tin nhân viên
            //            /*var role = nhanvien.;*/ // Giả sử VaiTro là thuộc tính xác định vai trò của nhân viên

            //            HttpContext.Session.SetString("UserId", staff.IdNhanVien.ToString());
            //            HttpContext.Session.SetString("UserName", staff.TenDangNhap);
            //            HttpContext.Session.SetString("Role", role);
            //            HttpContext.Session.SetInt32("BranchId", (int)nhanvien.IdChiNhanh);
            //            Response.Redirect("/BanAn");

            //            //if (role == "NhanVien")
            //            //{
            //            //    Response.Redirect("/BanAn");
            //            //}
            //            ////else if (role == "NhanVien")
            //            ////{
            //            ////    Response.Redirect("/BanAn");
            //            ////}
            //            //else
            //            //{
            //            //    Response.Redirect("/Index");
            //            //}

            //        }
            //    }
            //    else if (Input.Role == "customer")
            //    {
            //        // Kiểm tra đăng nhập cho khách hàng
            //        var customer = _context.TaiKhoans
            //            .FirstOrDefault(u => u.TenDangNhap == Input.Username &&
            //                                 u.MatKhau == ComputeMD5Hash(Input.Password) &&
            //                                 u.IdKhachHang != null);

            //        if (customer != null)
            //        {
            //            HttpContext.Session.SetString("UserId", customer.IdKhachHang.ToString());
            //            HttpContext.Session.SetString("UserName", customer.TenDangNhap);
            //            HttpContext.Session.SetString("Role", "KhachHang");
            //            Response.Redirect("/CaNhan");
            //        }
            //    }

            // Nếu vai trò của người dùng không khớp với bất kỳ vai trò nào ở trên, thêm lỗi vào ModelState
            //ModelState.AddModelError(string.Empty, "Vai trò người dùng không hợp lệ.");
            return Page();

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
