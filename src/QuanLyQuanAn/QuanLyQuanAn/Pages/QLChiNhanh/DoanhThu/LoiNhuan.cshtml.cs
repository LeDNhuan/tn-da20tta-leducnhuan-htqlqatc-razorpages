using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Pages.QLChiNhanh.DoanhThu
{
    public class LoiNhuanModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public LoiNhuanModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public List<PhieuChi> PhieuChis { get; set; } = new List<PhieuChi>();
        public List<ChiTietGoiMon> ChiTiets { get; set; } = new List<ChiTietGoiMon>();
        public string role { get; set; }
        public int branch { get; set; }
        public double DoanhThuNgay { get; set; }
        public double HoaHong { get; set; }
        public double SoChi { get; set; }
        public double LoiNhuan { get; set; }
        public DateTime? NgayChon { get; set; }
        public int ChiNhanhId { get; set; }

        public IActionResult OnGet(int chinhanhId, string ngaychon)
        {
            var userId = HttpContext.Session.GetString("UserId");
            role = HttpContext.Session.GetString("Role");
            var homnay = DateTime.Now;

            if (role != "QuanLyTong" && role != "QuanLy")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                // Thiết lập giá trị mặc định cho ngày nếu không có giá trị được truyền vào
                DateTime date;
                if (string.IsNullOrEmpty(ngaychon) || !DateTime.TryParse(ngaychon, out date))
                {
                    date = DateTime.Now.Date;
                }
                NgayChon = date; // Gán giá trị ngày đã chọn cho thuộc tính
                ChiNhanhId = chinhanhId;

                // Lấy dữ liệu dựa trên vai trò và chi nhánh
                if (role == "QuanLyTong")
                {
                    if (chinhanhId != 0)
                    {
                        DoanhThuNgay = (double)_context.HoaDons
                            .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date && hd.IdBanNavigation.IdChiNhanh == chinhanhId)
                            .Sum(hd => hd.TongTien);

                        HoaHong = (double)_context.HoaDons
                            .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date && hd.IdBanNavigation.IdChiNhanh == chinhanhId)
                            .Sum(hd => hd.TienHoaHong);

                        PhieuChis = _context.PhieuChis
                            .Where(pc => pc.NgayLap.Value.Date == date && pc.IdNhanVienNavigation.IdChiNhanh == chinhanhId)
                            .ToList();

                        SoChi = (double)PhieuChis.Sum(pc => pc.SoChi);

                        LoiNhuan = DoanhThuNgay - (HoaHong + SoChi);
                    }
                    else
                    {
                        DoanhThuNgay = (double)_context.HoaDons
                            .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date)
                            .Sum(hd => hd.TongTien);

                        HoaHong = (double)_context.HoaDons
                            .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date)
                            .Sum(hd => hd.TienHoaHong);

                        PhieuChis = _context.PhieuChis
                            .Where(pc => pc.NgayLap.Value.Date == date)
                            .ToList();

                        SoChi = (double)PhieuChis.Sum(pc => pc.SoChi);

                        LoiNhuan = DoanhThuNgay - (HoaHong + SoChi);
                    }
                }
                else
                {
                    branch = (int)HttpContext.Session.GetInt32("BranchId");

                    DoanhThuNgay = (double)_context.HoaDons
                        .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date && hd.IdBanNavigation.IdChiNhanh == branch)
                        .Sum(hd => hd.TongTien);

                    HoaHong = (double)_context.HoaDons
                        .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date && hd.IdBanNavigation.IdChiNhanh == branch)
                        .Sum(hd => hd.TienHoaHong);

                    PhieuChis = _context.PhieuChis
                        .Where(pc => pc.NgayLap.Value.Date == date && pc.IdNhanVienNavigation.IdChiNhanh == branch)
                        .ToList();

                    SoChi = (double)PhieuChis.Sum(pc => pc.SoChi);

                    LoiNhuan = DoanhThuNgay - (HoaHong + SoChi);
                }

                return Page();

                //if (role == "QuanLyTong")
                //{
                //    if (chinhanhId != 0)
                //    {
                //        if (!string.IsNullOrEmpty(ngaychon) && DateTime.TryParse(ngaychon, out DateTime date))
                //        {
                //            DoanhThuNgay = (double)_context.HoaDons
                //                .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date.Date && hd.IdBanNavigation.IdChiNhanh == chinhanhId)
                //                .Sum(hd => hd.TongTien);

                //            HoaHong = (double)_context.HoaDons
                //                .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date.Date && hd.IdBanNavigation.IdChiNhanh == chinhanhId)
                //                .Sum(hd => hd.TienHoaHong);

                //            PhieuChis = _context.PhieuChis
                //                .Where(pc => pc.NgayLap.Value.Date == date.Date && pc.IdNhanVienNavigation.IdChiNhanh == chinhanhId)
                //                .ToList();

                //            SoChi = (double)PhieuChis.Sum(pc => pc.SoChi);

                //            LoiNhuan = DoanhThuNgay - (HoaHong + SoChi);

                //            // Xử lý doanh thu ngày và trả về view hoặc JSON
                //            return Page(); // hoặc trả về view hoặc JSON
                //        }
                //    }
                //    else
                //    {                        
                //        if (!string.IsNullOrEmpty(ngaychon) && DateTime.TryParse(ngaychon, out DateTime date))
                //        {
                //            DoanhThuNgay = (double)_context.HoaDons
                //                .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date.Date)
                //                .Sum(hd => hd.TongTien);

                //            HoaHong = (double)_context.HoaDons
                //                .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date.Date)
                //                .Sum(hd => hd.TienHoaHong);

                //            PhieuChis = _context.PhieuChis
                //                .Where(pc => pc.NgayLap.Value.Date == date.Date)
                //                .ToList();

                //            SoChi = (double)PhieuChis.Sum(pc => pc.SoChi);

                //            LoiNhuan = DoanhThuNgay - (HoaHong + SoChi);

                //            // Xử lý doanh thu ngày và trả về view hoặc JSON
                //            return Page(); // hoặc trả về view hoặc JSON
                //        }
                //    }


                //}
                //else
                //{
                //    branch = (int)HttpContext.Session.GetInt32("BranchId");

                //    if (!string.IsNullOrEmpty(ngaychon) && DateTime.TryParse(ngaychon, out DateTime date))
                //    {
                //        DoanhThuNgay = (double)_context.HoaDons
                //            .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date.Date && hd.IdBanNavigation.IdChiNhanh == branch)
                //            .Sum(hd => hd.TongTien);

                //        HoaHong = (double)_context.HoaDons
                //                .Where(hd => hd.TrangThai == "ThanhToan" && hd.GioRa.Value.Date == date.Date && hd.IdBanNavigation.IdChiNhanh == branch)
                //                .Sum(hd => hd.TienHoaHong);

                //        PhieuChis = _context.PhieuChis
                //                .Where(pc => pc.NgayLap.Value.Date == date  .Date && pc.IdNhanVienNavigation.IdChiNhanh == branch)
                //                .ToList();

                //        SoChi = (double)PhieuChis.Sum(pc => pc.SoChi);

                //        LoiNhuan = DoanhThuNgay - (HoaHong + SoChi);

                //        // Xử lý doanh thu ngày và trả về view hoặc JSON
                //        return Page(); // hoặc trả về view hoặc JSON
                //    }
                //}


            }
            return BadRequest();
        }
    }
}
