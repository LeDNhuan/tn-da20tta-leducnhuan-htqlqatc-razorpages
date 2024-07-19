using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Pages.QLChiNhanh.MonAn
{
    public class SuaMonAnModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public SuaMonAnModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public List<LoaiMon> LoaiMons { get; set; } = new List<LoaiMon>();
        public List<ChiNhanh> ChiNhanhs { get; set; } = new List<ChiNhanh>();
        public List<NguyenLieu> nguyenlieus { get; set; } = new List<NguyenLieu>();
        public List<NguyenLieuMonGoi> nguyenlieumonans { get; set; } = new List<NguyenLieuMonGoi>();
        public MonGoi monan { get; set; }
        public ChiNhanh chinhanh { get; set; }
        public string role { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            //-----------------Món ăn---------------
            public string TenMon { get; set; }

            public double GiaBan { get; set; }

            public IFormFile AnhDaiDien { get; set; }

            public int ChiNhanh { get; set; }

            //-------------Nguyên liệu--------------
            //public int Id { get; set; }
            //public int SoLuong { get; set; }

        }

        [BindProperty]
        public List<NguyenLieuInputModel> NguyenLieuThems { get; set; }

        [BindProperty]
        public List<NguyenLieuInputModel> NguyenLieuSuas { get; set; }

        public class NguyenLieuInputModel
        {
            public int Id { get; set; }
            public string TenNguyenLieu { get; set; }
            public int? SoLuong { get; set; }
        }


        public void OnGet(int? monanId)
        {
            role = HttpContext.Session.GetString("Role");

            if (role != "QuanLy" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                if (role != "QuanLyTong")
                {
                    var branch = HttpContext.Session.GetInt32("BranchId");
                    if (monanId.HasValue)
                    {
                        monan = _context.MonGois
                            .Include(m => m.IdChiNhanhNavigation)
                            .Include(m => m.IdLoaiMonNavigation)
                            .Include(m => m.NguyenLieuMonGois)
                                .ThenInclude(nlm => nlm.NguyenLieu)
                            .FirstOrDefault(m => m.Id == monanId);

                        chinhanh = _context.ChiNhanhs.FirstOrDefault(cn => cn.Id == branch);                        
                    }
                    else
                    {
                        // Xử lý khi monanId không có giá trị
                        RedirectToPage("/QLMonAn");
                    }
                }
                else
                {
                    if (monanId.HasValue)
                    {
                        monan = _context.MonGois
                            .Include(m => m.IdChiNhanhNavigation)
                            .Include(m => m.IdLoaiMonNavigation)
                            .Include(m => m.NguyenLieuMonGois)
                                .ThenInclude(nlm => nlm.NguyenLieu)
                            .FirstOrDefault(m => m.Id == monanId);

                    }
                    else
                    {
                        // Xử lý khi monanId không có giá trị
                        RedirectToPage("/QLMonAn");
                    }
                }

                LoaiMons = _context.LoaiMons.ToList();
                nguyenlieus = _context.NguyenLieus.ToList();

                if (monan != null)
                {
                    // Lấy danh sách nguyên liệu đã có trong món ăn
                    NguyenLieuSuas = monan.NguyenLieuMonGois
                        .Select(nlm => new NguyenLieuInputModel
                        {
                            Id = nlm.NguyenLieuId,
                            TenNguyenLieu = nlm.NguyenLieu.TenNguyenLieu,
                            SoLuong = nlm.SoLuong
                        }).ToList();

                    // Lấy danh sách nguyên liệu còn lại
                    var usedNguyenLieuIds = NguyenLieuSuas.Select(nl => nl.Id).ToList();
                    NguyenLieuThems = _context.NguyenLieus
                        .Where(nl => !usedNguyenLieuIds.Contains(nl.Id))
                        .Select(nl => new NguyenLieuInputModel
                        {
                            Id = nl.Id,
                            TenNguyenLieu = nl.TenNguyenLieu,
                            SoLuong = 0 // Default value for new input
                        }).ToList();
                }

                //NguyenLieuSuas = nguyenlieumonans.Select(nl => new NguyenLieuInputModel
                //{
                //    Id = nl.NguyenLieuId,
                //    TenNguyenLieu = nl.NguyenLieu.TenNguyenLieu,
                //    SoLuong = nguyenlieumonans.FirstOrDefault(nlm => nlm.NguyenLieuId == nl.NguyenLieuId)?.SoLuong
                //}).ToList();

                //NguyenLieuSuas = nguyenlieumonans.Where(nl => nl.);

                //NguyenLieuThems = nguyenlieus
                //    .Where(nl => !NguyenLieuSuas.Any(nls => nls.Id == nl.Id))
                //    .ToList();

            }
        }

        public void OnPostSuaMonAn(int? monanId)
        {
            if (monanId.HasValue)
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
                //monan.IdLoaiMon = Input.LoaiMon;
                monan.IdChiNhanh = Input.ChiNhanh;

                _context.MonGois.Update(monan);
                _context.SaveChanges();

                Response.Redirect($"/QLMonAn?chinhanhId={Input.ChiNhanh}");
            }
            else
            {
                Response.Redirect("/QLMonAn");
            }            
        }

        public void OnPostThemNguyenLieu(int monanId)
        {
            var monan = _context.MonGois.FirstOrDefault(m => m.Id == monanId);
            if (monan == null)
            {
                Response.Redirect("/QLMonAn");
            }

            if (NguyenLieuThems != null)
            {
                foreach (var nguyenlieu in NguyenLieuThems)
                {
                    if (nguyenlieu.SoLuong > 0)
                    {
                        var nguyenlieumongoi = new NguyenLieuMonGoi
                        {
                            MonGoiId = monanId,
                            NguyenLieuId = nguyenlieu.Id,
                            SoLuong = nguyenlieu.SoLuong
                        };
                        _context.NguyenLieuMonGois.Add(nguyenlieumongoi);
                    }
                }

                _context.SaveChanges();
            }

            Response.Redirect("/QLMonAn");
        }

        //-------------------Sửa nguyên liệu-------------------
        public void OnPostSuaNguyenLieu(int monanId)
        {
            //var monan = _context.MonGois.FirstOrDefault(m => m.Id == monanId);
            //if (monan == null)
            //{
            //    Response.Redirect("/QLMonAn");
            //}

            //if (nguyenlieumonans != null)
            //{
            //    foreach (var nguyenlieu in nguyenlieumonans)
            //    {
            //        var nglieumon = _context.NguyenLieuMonGois.FirstOrDefault(nlm => nlm.NguyenLieuId == nguyenlieu.NguyenLieuId && nlm.MonGoiId == monanId);
            //        if (nguyenlieu != null && nguyenlieu.SoLuong > 0)
            //        {
            //            nglieumon.SoLuong = nguyenlieu.SoLuong;                        
            //            _context.NguyenLieuMonGois.Update(nglieumon);
            //        }
            //    }
            //}

            //_context.SaveChanges();

            var monan = _context.MonGois.Include(m => m.NguyenLieuMonGois).FirstOrDefault(m => m.Id == monanId);

            if (monan == null)
            {
                Response.Redirect("/QLMonAn");
            }

            if (NguyenLieuSuas != null)
            {
                foreach (var nguyenlieu in NguyenLieuSuas)
                {
                    var nglieumon = _context.NguyenLieuMonGois.FirstOrDefault(nlm => nlm.NguyenLieuId == nguyenlieu.Id && nlm.MonGoiId == monanId);

                    if (nglieumon != null)
                    {
                        if (nguyenlieu.SoLuong > 0)
                        {
                            nglieumon.SoLuong = nguyenlieu.SoLuong;
                            _context.NguyenLieuMonGois.Update(nglieumon);
                        }
                        else
                        {
                            _context.NguyenLieuMonGois.Remove(nglieumon);
                        }
                    }
                }

                _context.SaveChanges();
            }

            Response.Redirect("/QLMonAn");
        }
    }
}
