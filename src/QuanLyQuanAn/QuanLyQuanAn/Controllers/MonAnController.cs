using Azure;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers
{
    public class MonAnController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public MonAnController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public List<LoaiMon> LoaiMons { get; set; } = new List<LoaiMon>();
        public List<MonGoi> MonAns { get; set; } = new List<MonGoi>();
        public List<ChiNhanh> ChiNhanhs { get; set; } = new List<ChiNhanh>();
        public LoaiMon loaimon { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MonanId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? LoaimonId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Action { get; set; }

        public MonGoi MonAn { get; set; }
        public LoaiMon LoaiMon { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            public string TenMon { get; set; }

            public double GiaBan { get; set; }

            public IFormFile AnhDaiDien { get; set; }

            public int LoaiMon { get; set; }

            public int ChiNhanh { get; set; }
        }

        [HttpPost]
        public IActionResult ThemMonAnMoi(int loaimonId, MonGoi model)
        {
            // Xử lý việc lưu trữ ảnh đại diện
            string profileImagePath;
            if (Input.AnhDaiDien != null)
            {
                // Lưu ảnh vào thư mục "wwwroot/uploads" hoặc thư mục mong muốn
                var uploadsFolder = Path.Combine(_env.WebRootPath, "customize/img/");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Input.AnhDaiDien.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    Input.AnhDaiDien.CopyTo(fileStream);
                }

                profileImagePath = $"/customize/img/{uniqueFileName}";
            }
            else
            {
                // Nếu không tải lên ảnh, sử dụng ảnh mặc định
                profileImagePath = "/customize/img/default_monan.png"; // Đường dẫn ảnh mặc định trong dự án
            }

            var monan = new MonGoi
            {
                TenMon = Input.TenMon,
                GiaBan = Input.GiaBan,
                AnhDaiDien = profileImagePath,
                IdLoaiMon = Input.LoaiMon,
                IdChiNhanh = Input.ChiNhanh,
                CreatedAt = DateTime.Now
            };

            _context.MonGois.Add(monan);
            _context.SaveChanges();

            return Redirect("/QLMonAn");
        }

        [HttpPost]
        public IActionResult SuaMonAn(int monanId, MonGoi model)
        {
            var monan = _context.MonGois.FirstOrDefault(m => m.Id == monanId);
            if (monan == null)
            {
                // Xử lý trường hợp không tìm thấy món ăn với monanId
                Response.Redirect("/QLMonAn");
                
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
                if (!string.IsNullOrEmpty(monan.AnhDaiDien) && monan.AnhDaiDien != "/customize/img/default_monan.png")
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, monan.AnhDaiDien.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                monan.AnhDaiDien = $"/customize/img/{uniqueFileName}";
            }

            // Cập nhật thông tin món ăn
            monan.TenMon = Input.TenMon;
            monan.GiaBan = Input.GiaBan;
            monan.IdLoaiMon = Input.LoaiMon;
            monan.IdChiNhanh = Input.ChiNhanh;

            _context.MonGois.Update(monan);
            _context.SaveChanges();

            return Redirect("/QLMonAn");
        }

        [HttpPost]
        public IActionResult XoaMonAn(int monanId)
        {
            // Logic để xóa món ăn với monanId
            return RedirectToPage("DanhSachMonAn");
        }
    }
}
