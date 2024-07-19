using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyQuanAn.Pages.QLChiNhanh.MonAn
{
    public class ThemMonAnModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public ThemMonAnModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public List<LoaiMon> LoaiMons { get; set; } = new List<LoaiMon>();
        public List<ChiNhanh> ChiNhanhs { get; set; } = new List<ChiNhanh>();
        public LoaiMon loaimon { get; set; }
        public ChiNhanh chinhanh { get; set; }
        public int IdChiNhanh { get; set; }
        public string role { get; set; }

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

        public void OnGet(int chinhanhId, int loaimonId)
        {
            role = HttpContext.Session.GetString("Role");

            if (role != "QuanLy" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {               

                IdChiNhanh = chinhanhId;
                //LoadFormData(IdChiNhanh, loaimonId);

                if (role != "QuanLyTong")
                {
                    var branch = HttpContext.Session.GetInt32("BranchId");
                    chinhanh = _context.ChiNhanhs.FirstOrDefault(cn => cn.Id == branch);
                }
                else
                {
                    chinhanh = _context.ChiNhanhs.FirstOrDefault(cn => cn.Id == IdChiNhanh);
                }

                loaimon = _context.LoaiMons.FirstOrDefault(lm => lm.Id == loaimonId);
                LoaiMons = _context.LoaiMons.ToList();
            }
        }

        //private void LoadFormData(int chinhanhId, int loaimonId)
        //{
            
        //    if (role != "QuanLyTong")
        //    {
        //        var branch = HttpContext.Session.GetInt32("BranchId");
        //        chinhanh = _context.ChiNhanhs.FirstOrDefault(cn => cn.Id == branch);
        //    }
        //    else
        //    {
        //        chinhanh = _context.ChiNhanhs.FirstOrDefault(cn => cn.Id == chinhanhId);
        //    }

        //    loaimon = _context.LoaiMons.FirstOrDefault(lm => lm.Id == loaimonId);
        //    LoaiMons = _context.LoaiMons.ToList();
        //}

        public void OnPostThemMonAn()
        {
            role = HttpContext.Session.GetString("Role");
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

            if (role == "QuanLyTong")
            {
                // Kiểm tra nếu món ăn đã tồn tại
                var tontaimonan = _context.MonGois.FirstOrDefault(m => m.IdLoaiMon == Input.LoaiMon && m.TenMon == Input.TenMon && m.IdChiNhanh == Input.ChiNhanh);
                if (tontaimonan != null)
                {
                    // Thông báo lỗi cho người dùng nếu món ăn đã tồn tại
                    ModelState.AddModelError(string.Empty, "Món ăn với tên này đã tồn tại.");
                    //LoadFormData(IdChiNhanh, Input.LoaiMon);
                    return;
                }
                var monan = new MonGoi
                {
                    IdChiNhanh = Input.ChiNhanh,
                    TenMon = Input.TenMon,
                    GiaBan = Input.GiaBan,
                    AnhDaiDien = profileImagePath,
                    IdLoaiMon = Input.LoaiMon,
                    CreatedAt = DateTime.Now
                };

                _context.MonGois.Add(monan);
                _context.SaveChanges();
            }
            else
            {               

                var branch = HttpContext.Session.GetInt32("BranchId");
                // Kiểm tra nếu món ăn đã tồn tại
                var tontaimonan = _context.MonGois.FirstOrDefault(m => m.IdLoaiMon == Input.LoaiMon && m.TenMon == Input.TenMon);
                if (tontaimonan != null)
                {
                    // Thông báo lỗi cho người dùng nếu món ăn đã tồn tại
                    ModelState.AddModelError(string.Empty, "Món ăn với tên này đã tồn tại.");
                    //LoadFormData(IdChiNhanh, Input.LoaiMon);
                    return;

                }
                var monan = new MonGoi
                {
                    TenMon = Input.TenMon,
                    GiaBan = Input.GiaBan,
                    AnhDaiDien = profileImagePath,
                    IdLoaiMon = Input.LoaiMon,
                    IdChiNhanh = branch,
                    CreatedAt = DateTime.Now
                };

                _context.MonGois.Add(monan);
                _context.SaveChanges();
            }
            Response.Redirect("/QLMonAn");
        }
    }
}
