using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using static QuanLyQuanAn.Pages.TKhoan.DangNhapModel;

namespace QuanLyQuanAn.Pages.MonAn
{
    public class ChiTietGoiMonModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public List<ChiTietGoiMon> ChiTietChuas { get; set; } = new List<ChiTietGoiMon>();
        public List<ChiTietGoiMon> ChiTiets { get; set; } = new List<ChiTietGoiMon>();
        public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public HoaDon HoaDon { get; set; }
        public int HoaDonId { get; set; }
        public bool TatCaChiTiet { get; set; }
        public bool TonTaiChiTietRoi { get; set; }
        public string role { get; set; }

        public ChiTietGoiMonModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public List<ChiTietGoiMonInput> ChiTiets { get; set; }
        }

        public class ChiTietGoiMonInput
        {
            public int Id { get; set; }
            public int SoLuong { get; set; }
        }


        public void OnGet(int hoadonId)
        {
            role = HttpContext.Session.GetString("Role");

            if (role != "NhanVien" && role != "QuanLyTong" && role != "ThuNgan")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                HttpContext.Session.SetInt32("BillId", hoadonId);
                HoaDonId = hoadonId;
                HoaDon = _context.HoaDons.FirstOrDefault(hd => hd.Id == hoadonId);

                if (HoaDon != null)
                {
                    ChiTiets = _context.ChiTietGoiMons
                        .Where(ct => ct.IdHoaDon == hoadonId)
                        .Include(ct => ct.IdMonGoiNavigation)
                            .ThenInclude(ma => ma.IdLoaiMonNavigation)
                        .ToList();

                    TatCaChiTiet = ChiTiets.All(ct => ct.TrangThai == "Roi");

                    TonTaiChiTietRoi = ChiTiets.Any(ct => ct.TrangThai == "Roi");
                }
            }                
        }

        public void OnPostUpdateChiTiet(int chitietId, int soluongmoi)
        {
            var billId = HttpContext.Session.GetInt32("BillId");

            if (ModelState.IsValid)
            {
                foreach (var inputChiTiet in Input.ChiTiets)
                {
                    var chitiet = _context.ChiTietGoiMons.FirstOrDefault(ct => ct.Id == inputChiTiet.Id);
                    if (chitiet != null)
                    {
                        var monan = _context.MonGois.FirstOrDefault(m => m.Id == chitiet.IdMonGoi);
                        if (monan != null)
                        {
                            // Cập nhật số lượng và thành tiền
                            chitiet.SoLuong = inputChiTiet.SoLuong;
                            chitiet.ThanhTien = monan.GiaBan * inputChiTiet.SoLuong;

                            _context.ChiTietGoiMons.Update(chitiet);
                        }
                    }
                }

                _context.SaveChanges();

                // Cập nhật tổng tiền của hóa đơn
                var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.Id == billId);
                if (hoadon != null)
                {
                    hoadon.TongTien = _context.ChiTietGoiMons
                        .Where(ct => ct.IdHoaDon == hoadon.Id)
                        .Sum(ct => ct.ThanhTien);

                    _context.HoaDons.Update(hoadon);
                    _context.SaveChanges();
                }

                // Redirect lại trang với id hóa đơn để cập nhật lại dữ liệu hiển thị
                Response.Redirect($"/ChiTiet?hoadonId={billId}");
            }
        }

        public void OnPostThongBao()
        {
            var billId = HttpContext.Session.GetInt32("BillId");

            if (billId.HasValue)
            {
                var chitiets = _context.ChiTietGoiMons
                    .Where(ct => ct.IdHoaDon == billId.Value)
                    .ToList();

                // Dictionary để lưu trữ thông tin món gọi đã gộp
                var combinedOrders = new Dictionary<int, ChiTietGoiMon>();

                foreach (var chitiet in chitiets)
                {
                    // Chuyển trạng thái thành "Roi"
                    chitiet.TrangThai = "Roi";

                    // Kiểm tra xem IdMonGoi có giá trị không
                    if (chitiet.IdMonGoi.HasValue)
                    {
                        int monGoiId = chitiet.IdMonGoi.Value;

                        // Kiểm tra xem món gọi đã tồn tại trong dictionary hay chưa
                        if (combinedOrders.ContainsKey(monGoiId))
                        {
                            // Nếu đã tồn tại, gộp số lượng và thành tiền
                            combinedOrders[monGoiId].SoLuong += chitiet.SoLuong;
                            combinedOrders[monGoiId].ThanhTien += chitiet.ThanhTien;

                            // Xóa chi tiết gọi món gốc khỏi database
                            _context.ChiTietGoiMons.Remove(chitiet);
                        }
                        else
                        {
                            // Nếu chưa tồn tại, thêm vào dictionary
                            combinedOrders[monGoiId] = chitiet;
                        }
                    }
                }

                // Lưu lại tất cả các chi tiết gọi món đã gộp
                foreach (var order in combinedOrders.Values)
                {
                    _context.ChiTietGoiMons.Update(order);
                }

                _context.SaveChanges();
            }

            Response.Redirect($"/ChiTiet?hoadonId={billId}");
        }

        //public void OnPostHuyHoaDon(int hoadonId)
        //{
        //    var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.Id == hoadonId);

        //    if (hoadon == null)
        //    {
        //        Response.Redirect("/BanAn");
        //    }

        //    var chitiets = _context.ChiTietGoiMons
        //        .Where(ct => ct.IdHoaDon == hoadonId)
        //        .ToList();

        //    TonTaiChiTietRoi = chitiets.Any(ct => ct.TrangThai == "Roi");

        //    if (TonTaiChiTietRoi)
        //    {
        //        Response.Redirect("/BanAn");
        //    }

        //    _context.HoaDons.Remove(hoadon);
        //    _context.ChiTietGoiMons.RemoveRange(chitiets);
        //    _context.SaveChanges();

        //    Response.Redirect("/BanAn");
        //}
        public void OnPostHuyHoaDon(int hoadonId)
        {
            var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.Id == hoadonId);
            Console.WriteLine($"Hoadon: {hoadon}");  // Thêm câu lệnh gỡ lỗi

            if (hoadon == null)
            {
                Console.WriteLine("Hoadon không tồn tại");  // Thêm câu lệnh gỡ lỗi
                Response.Redirect("/BanAn");
                return;  // Thêm câu lệnh return để dừng phương thức
            }

            var chitiets = _context.ChiTietGoiMons
                .Where(ct => ct.IdHoaDon == hoadonId)
                .ToList();

            TonTaiChiTietRoi = chitiets.Any(ct => ct.TrangThai == "Roi");
            Console.WriteLine($"TonTaiChiTietRoi: {TonTaiChiTietRoi}");  // Thêm câu lệnh gỡ lỗi

            if (TonTaiChiTietRoi)
            {
                Console.WriteLine("Tồn tại chi tiết gọi món có trạng thái 'Roi'");  // Thêm câu lệnh gỡ lỗi
                Response.Redirect("/BanAn");
                return;  // Thêm câu lệnh return để dừng phương thức
            }

            _context.HoaDons.Remove(hoadon);
            _context.ChiTietGoiMons.RemoveRange(chitiets);
            _context.SaveChanges();
            Console.WriteLine("Hóa đơn và các chi tiết gọi món đã được xóa");  // Thêm câu lệnh gỡ lỗi

            Response.Redirect("/BanAn");
        }

    }
}
