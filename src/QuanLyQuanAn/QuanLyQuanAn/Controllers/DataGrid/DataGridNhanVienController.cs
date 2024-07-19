using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyQuanAn.Models;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridNhanVienController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DataGridNhanVienController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public string role { get; set; }
        public NhanVien nv { get; set; }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            role  = HttpContext.Session.GetString("Role");
            if (role == "QuanLyTong")
            {
                var nhanvien = _context.NhanViens;

                var loadResult = await DataSourceLoader.LoadAsync(nhanvien, loadOptions);
                return Json(loadResult);
            }
            else
            {
                var branch = HttpContext.Session.GetInt32("BranchId");
                var nhanvien = _context.NhanViens
                    .Where(nv => nv.IdChiNhanh == branch);

                var loadResult = await DataSourceLoader.LoadAsync(nhanvien, loadOptions);
                return Json(loadResult);
            }

            
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            var newNhanVien = new NhanVien();
            JsonConvert.PopulateObject(values, newNhanVien);

            // Generate unique ID and hash the password
            newNhanVien.Id = GenerateUniqueRandomId();
            newNhanVien.MatKhau = ComputeMD5Hash(newNhanVien.MatKhau);
            newNhanVien.CreatedAt = DateTime.Now;
            newNhanVien.AnhDaiDien = "/customize/img/default_avatar.png";

            if (!TryValidateModel(newNhanVien))
                return BadRequest(ModelState);

            _context.NhanViens.Add(newNhanVien);
            await _context.SaveChangesAsync();
            return Ok(newNhanVien);

            
            
        }

        [HttpPut]
        public async Task<IActionResult> Put(string key, string values)
        {
            var nhanvien = await _context.NhanViens.FirstOrDefaultAsync(nv => nv.Id == key);
            if (nhanvien == null)
                return StatusCode(409, "Nhân viên không tồn tại.");

            //var updatedNhanVien = new NhanVien();
            JsonConvert.PopulateObject(values, nhanvien);

            // Check if the password has changed and hash it
            if (!string.IsNullOrEmpty(nhanvien.MatKhau))
            {
                nhanvien.MatKhau = ComputeMD5Hash(nhanvien.MatKhau);
            }
            
            if (!string.IsNullOrEmpty(nhanvien.NgaySinh.Value.ToString()))
            {                
                nhanvien.NgaySinh = DateOnly.Parse(nhanvien.NgaySinh.Value.ToString());                
            }

            

            if (!TryValidateModel(nhanvien))
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(nhanvien);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string key)
        {
            var nhanvien = await _context.NhanViens.FirstOrDefaultAsync(nv => nv.Id == key);
            if (nhanvien == null)
                return StatusCode(409, "Nhân viên không tồn tại.");

            _context.NhanViens.Remove(nhanvien);
            await _context.SaveChangesAsync();
            return Ok();
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

