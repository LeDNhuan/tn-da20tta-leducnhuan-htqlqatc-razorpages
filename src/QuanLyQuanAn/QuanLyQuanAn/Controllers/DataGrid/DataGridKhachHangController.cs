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
    public class DataGridKhachHangController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DataGridKhachHangController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public KhachHang kh { get; set; }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var khachhang = _context.KhachHangs;

            var loadResult = await DataSourceLoader.LoadAsync(khachhang, loadOptions);
            return Json(loadResult);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {

            var newKhachHang = new KhachHang();
            JsonConvert.PopulateObject(values, newKhachHang);

            if (!TryValidateModel(newKhachHang))
                return BadRequest(ModelState);

            _context.KhachHangs.Add(newKhachHang);
            await _context.SaveChangesAsync();
            return Ok(newKhachHang);
        }

        [HttpPut]
        public async Task<IActionResult> Put(string key, string values)
        {
            var khachhang = await _context.KhachHangs.FirstOrDefaultAsync(kh => kh.Id == key);
            if (khachhang == null)
                return StatusCode(409, "Khách hàng không tồn tại.");

            //var updatedNhanVien = new NhanVien();
            JsonConvert.PopulateObject(values, khachhang);

            if (!string.IsNullOrEmpty(khachhang.NgaySinh.Value.ToString()))
            {
                khachhang.NgaySinh = DateOnly.Parse(khachhang.NgaySinh.Value.ToString());
            }



            if (!TryValidateModel(khachhang))
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(khachhang);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string key)
        {
            var khachhang = await _context.KhachHangs.FirstOrDefaultAsync(kh => kh.Id == key);
            if (khachhang == null)
                return StatusCode(409, "Khách hàng không tồn tại.");

            _context.KhachHangs.Remove(khachhang);
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
