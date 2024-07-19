using Microsoft.AspNetCore.Mvc;
using QRCoder;
using QuanLyQuanAn.Models;
using System.Drawing;
using System.IO;
using static QRCoder.PayloadGenerator;

namespace QuanLyQuanAn.Controllers
{
    public class TaoQR : Controller
    {
        private readonly QuanLyQuanAnContext _context;

        public TaoQR(QuanLyQuanAnContext context)
        {
            _context = context;
        }

        public IActionResult ScanQrCode(string payload)
        {
            try
            {
                var parts = payload.Split(':');
                if (parts.Length == 2 && long.TryParse(parts[1], out long expiryTicks))
                {
                    var hoadonId = int.Parse(parts[0]);
                    var expiry = new DateTime(expiryTicks);

                    if (expiry > DateTime.UtcNow)
                    {
                        var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.Id == hoadonId);
                        if (hoadon != null)
                        {
                            var khachHang = _context.KhachHangs.FirstOrDefault(kh => kh.Id == hoadon.IdKhachHang);
                            if (khachHang != null)
                            {
                                // Cộng điểm cho khách hàng
                                khachHang.DiemTichLuy += (int)(hoadon.TongTien * 0.10);
                                _context.SaveChanges();
                                return Ok("Điểm đã được cộng thành công!");
                            }
                            return BadRequest("Không tìm thấy khách hàng.");
                        }
                        return BadRequest("Không tìm thấy hóa đơn.");
                    }
                    return BadRequest("Mã QR đã hết hạn.");
                }
                return BadRequest("Mã QR không hợp lệ.");
            }
            catch (Exception ex)
            {
                return BadRequest("Đã xảy ra lỗi khi xử lý mã QR: " + ex.Message);
            }
        }
    }
}
