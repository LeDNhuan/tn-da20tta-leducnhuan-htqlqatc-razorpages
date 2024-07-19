using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Pages.HoaDonAn
{
    public class InHoaDonModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public List<MonGoi> MonAns { get; set; } = new List<MonGoi>();
        public List<LoaiMon> LoaiMons { get; set; } = new List<LoaiMon>();
        public List<ChiTietGoiMon> ChiTiets { get; set; } = new List<ChiTietGoiMon>();
        public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        public HoaDon HoaDon { get; set; }
        public int HoaDonId { get; set; }
        public string TenChiNhanh { get; set; }
        public string nvien { get; set; }

        public InHoaDonModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public void OnGet(int hoadonId)
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "ThuNgan" && role != "QuanLyTong")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                nvien = HttpContext.Session.GetString("UserName");

                HoaDonId = hoadonId;
                HoaDon = _context.HoaDons
                    .Include(hd => hd.IdBanNavigation)
                        .ThenInclude(b => b.IdChiNhanhNavigation)
                    .FirstOrDefault(hd => hd.Id == hoadonId);

                if (HoaDon != null)
                {
                    HoaDon.GioRa = DateTime.Now;
                    _context.HoaDons.Update(HoaDon);
                    _context.SaveChanges();

                    // Tạo payload với thời gian hết hạn
                    var payload = $"{hoadonId}:{DateTime.UtcNow.AddHours(24).Ticks}";

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeImage = qrCode.GetGraphic(20);

                    string base64String = Convert.ToBase64String(qrCodeImage);
                    string qrCodeImageUrl = "data:image/png;base64," + base64String;

                    // Truyền mã QR base64 đến Razor Page
                    TempData["QrCodeImageUrl"] = qrCodeImageUrl;
                }


                ChiTiets = _context.ChiTietGoiMons
                    .Where(ct => ct.IdHoaDon == hoadonId)
                    .Include(ct => ct.IdMonGoiNavigation)
                        .ThenInclude(ma => ma.IdLoaiMonNavigation)
                    .ToList();
            }
            
        }


        //public IActionResult CollectPoints(int orderId, int customerId)
        //{
        //    // Logic để cộng điểm cho khách hàng
        //    var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);
        //    var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);

        //    if (order != null && customer != null)
        //    {
        //        // Cộng 10% từ tổng tiền của hóa đơn vào điểm thưởng của khách hàng
        //        customer.Points += (order.TongTien * 0.10);
        //        _context.SaveChanges();
        //        return Ok("Điểm đã được cộng thành công!");
        //    }
        //    return BadRequest("Không tìm thấy đơn hàng hoặc khách hàng.");
        //}

    }
}
