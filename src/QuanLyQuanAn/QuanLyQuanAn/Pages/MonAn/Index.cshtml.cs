using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Extensions;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Pages.MonAn
{
    public class IndexModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public List<MonGoi> MonAns { get; set; } = new List<MonGoi>();
        public List<LoaiMon> LoaiMons { get; set; } = new List<LoaiMon>();
        public List<ChiTietGoiMon> ChiTiets { get; set; } = new List<ChiTietGoiMon>();
        public int BanId { get; set; }
        public HoaDon HoaDonHienTai { get; set; }

        public IndexModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public void OnGet(int banId)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "NhanVien" && role != "QuanLyTong" && role != "ThuNgan")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            { 
                BanId = banId;
                //HttpContext.Session.SetInt32("SelectedBanId", banId);
                var nvien = HttpContext.Session.GetString("UserId");

                // Kiểm tra xem có hóa đơn chưa thanh toán cho bàn này không
                HoaDonHienTai = _context.HoaDons.FirstOrDefault(hd => hd.IdBan == banId && hd.TrangThai != "ThanhToan");

                if (HoaDonHienTai == null)
                {
                    // Nếu chưa có hóa đơn, tạo mới
                    HoaDonHienTai = new HoaDon
                    {
                        IdBan = banId,
                        IdNhanVien = nvien,
                        TongTien = 0,
                        MucDoCho = 0,
                        GioVao = DateTime.Now,
                        TrangThai = "KhoiTao",
                        CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    };

                    _context.HoaDons.Add(HoaDonHienTai);
                    _context.SaveChanges();
                }

                // Lưu ID hóa đơn vào Session
                //var hoadonIds = HttpContext.Session.GetObjectFromJson<List<int>>("HoaDonIds") ?? new List<int>();
                HttpContext.Session.SetInt32("BillId", HoaDonHienTai.Id);

                //if (!hoadonId.Contains(HoaDonHienTai.Id))
                //{
                //    hoadonId.Add(HoaDonHienTai.Id);
                //    HttpContext.Session.SetObjectAsJson("HoaDonIds", hoaDonIds);
                //}

                LoaiMons = _context.LoaiMons.OrderBy(lm => lm.Id).ToList();
                MonAns = _context.MonGois.Include(m => m.IdLoaiMonNavigation).OrderBy(ma => ma.Id).ToList();

                //foreach (var loaimon in LoaiMons)
                //{
                //    //MonAns = _context.MonGois.Where(m => m.IdLoaiMon == loaimon.Id).OrderBy(ma => ma.Id).ToList();
                //    //MonAns = MonAns.Where(ma => ma.IdLoaiMon == loaimon.Id).ToList();
                //}

                // Tải chi tiết gọi món cho hóa đơn hiện tại
                //ChiTietGoiMons = _context.ChiTietGoiMons.Where(ct => ct.IdHoaDon == HoaDonHienTai.Id).Include(ct => ct.IdMonGoiNavigation).ToList();
            }
        }

        [BindProperty]
        public InputMonGoiModel Input { get; set; }

        public class InputMonGoiModel
        {
            public int? monanId { get; set; }
            public int? soluong { get; set; }
            public string ghichu { get; set; }
        }

        public void OnPost(int banId)
        {
            BanId = banId;
            //var hoadonIds = HttpContext.Session.GetObjectFromJson<List<int>>("HoaDonIds");
            //var hoadonId = hoadonIds?.FirstOrDefault(id => _context.HoaDons.Any(hd => hd.Id == id && hd.IdBan == banId));

            // Lấy ID hóa đơn từ session
            var hdhientaiId = HttpContext.Session.GetInt32("BillId");
            
            if (hdhientaiId.HasValue)
            {
                // Lấy hóa đơn từ cơ sở dữ liệu với ID này
                var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.Id == hdhientaiId.Value && hd.IdBan == banId);

                if (hoadon != null && Input.soluong.HasValue && Input.soluong.Value > 0 && Input.monanId.HasValue)
                {
                    var monan = _context.MonGois.FirstOrDefault(m => m.Id == Input.monanId.Value);
                    if (monan != null)
                    {
                        // Kiểm tra xem chi tiết gọi món này đã tồn tại trong hóa đơn hay chưa
                        var existingOrder = _context.ChiTietGoiMons.FirstOrDefault(ct => ct.IdHoaDon == hoadon.Id && ct.IdMonGoi == Input.monanId.Value && ct.TrangThai == "Chua");

                        if (existingOrder != null)
                        {
                            // Nếu đã tồn tại, cập nhật số lượng và thành tiền
                            existingOrder.SoLuong += Input.soluong.Value;
                            existingOrder.ThanhTien += monan.GiaBan * Input.soluong.Value;
                            _context.ChiTietGoiMons.Update(existingOrder);
                        }
                        else
                        {
                            // Nếu chưa tồn tại, thêm chi tiết gọi món mới
                            var newOrder = new ChiTietGoiMon
                            {
                                IdMonGoi = Input.monanId.Value,
                                SoLuong = Input.soluong.Value,
                                IdHoaDon = hoadon.Id,
                                TrangThai = "Chua",
                                GhiChu = Input.ghichu,
                                ThanhTien = monan.GiaBan * Input.soluong.Value,
                            };

                            _context.ChiTietGoiMons.Add(newOrder);
                        }

                        _context.SaveChanges();

                        // Cập nhật tổng tiền của hóa đơn
                        hoadon.TongTien += monan.GiaBan * Input.soluong.Value;
                        _context.HoaDons.Update(hoadon);
                        _context.SaveChanges();
                    }
                }
            }            

            Response.Redirect($"/GoiMon?banId={BanId}");
        }


    }
}
