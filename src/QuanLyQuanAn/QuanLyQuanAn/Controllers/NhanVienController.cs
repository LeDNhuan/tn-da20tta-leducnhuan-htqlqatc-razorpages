using Microsoft.AspNetCore.Mvc;
using QuanLyQuanAn.Models;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyQuanAn.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly QuanLyQuanAnContext _context;

        public NhanVienController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _env = environment;
            _context = context;
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

        [HttpPost]
        public async Task<IActionResult> Create(NhanVien nhanvien, string password)
        {
            if (ModelState.IsValid)
            {
                nhanvien.MatKhau = ComputeMD5Hash(password);
                _context.NhanViens.Add(nhanvien);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Update(NhanVien nhanvien)
        {
            if (ModelState.IsValid)
            {
                var existingNhanVien = await _context.NhanViens.FindAsync(nhanvien.Id);
                if (existingNhanVien != null)
                {
                    _context.Entry(existingNhanVien).CurrentValues.SetValues(nhanvien);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
            return Json(new { success = false });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var nhanvien = await _context.NhanViens.FindAsync(id);
            if (nhanvien != null)
            {
                _context.NhanViens.Remove(nhanvien);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
