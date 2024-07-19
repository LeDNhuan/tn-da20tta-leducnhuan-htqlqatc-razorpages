using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Pages.BanAn
{
    public class IndexModel : PageModel
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public List<Ban> Bans { get; set; } = new List<Ban>();
        public List<HoaDon> HoaDonChuaTT { get; set; } = new List<HoaDon>();
        public Ban BanAn { get; set; }
        public IndexModel(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }
        
        public IActionResult OnGet(int chinhanhId)
        {
            var role = HttpContext.Session.GetString("Role");            

            if (role != "NhanVien" && role != "QuanLyTong" && role != "ThuNgan" && role != "QuanLy")
            {
                Response.Redirect("/LoiTruyCap"); // Chuyển hướng đến trang lỗi truy cập
            }
            else
            {
                
                if(role == "QuanLyTong")
                {
                    if (chinhanhId != 0)
                    {
                        Bans = _context.Bans
                            .Where(b => b.IdChiNhanh == chinhanhId)
                            .OrderBy(b => b.Id)
                            .ToList();
                    }                        
                }
                else
                {
                    var branch = HttpContext.Session.GetInt32("BranchId");
                    Bans = _context.Bans
                        .Where(b => b.IdChiNhanh == branch)
                        .OrderBy(b => b.Id).ToList();
                }

                //Xóa session id hóa đơn
                HttpContext.Session.Remove("BillId");

                // Kiểm tra và xóa hóa đơn chưa hoàn thành 
                var HoaDonChuaHoanThanh = _context.HoaDons
                    .Where(hd => hd.TrangThai == "KhoiTao")
                    .ToList();

                foreach (var hoadon in HoaDonChuaHoanThanh)
                {
                    var ChiTietGoiMons = _context.ChiTietGoiMons
                        .Where(ct => ct.IdHoaDon == hoadon.Id)
                        .ToList();

                    if (ChiTietGoiMons.Count == 0)
                    {
                        _context.HoaDons.Remove(hoadon);
                    }
                }
                _context.SaveChanges();
               

                // Lấy danh sách hóa đơn chưa thanh toán
                HoaDonChuaTT = _context.HoaDons
                    .Where(hd => hd.TrangThai != "ThanhToan")
                    .ToList();

                foreach (var ban in Bans)
                {
                    var HoaDonChuaTT = _context.HoaDons
                        .Where(hd => hd.IdBan == ban.Id && hd.TrangThai != "ThanhToan")
                        .FirstOrDefault();

                    if (HoaDonChuaTT != null)
                    {
                        var ChiTietGoiMons = _context.ChiTietGoiMons
                            .Where(ct => ct.IdHoaDon == HoaDonChuaTT.Id)
                            .ToList();

                        if (ChiTietGoiMons.All(ct => ct.TrangThai == "Roi"))
                        {
                            ban.TinhTrang = "ToanBoRoi"; // Cập nhật trạng thái bàn
                        }
                        else
                        {
                            ban.TinhTrang = "DangPhucVu";
                        }
                    }
                }              
            }
            return Page();

        }

        [HttpPost]
        public IActionResult UpdateMucDoCho([FromBody] UpdateMucDoChoRequest request)
        {
            var hoadon = _context.HoaDons.FirstOrDefault(hd => hd.Id == request.HoaDonId);
            if (hoadon != null)
            {
                hoadon.MucDoCho = request.MucDoCho;
                _context.HoaDons.Update(hoadon);
                _context.SaveChanges();
                //return Ok();  // Trả về mã trạng thái HTTP 200 nếu thành công
            }
            return NotFound();  // Trả về mã trạng thái HTTP 404 nếu không tìm thấy hóa đơn
        }

        public class UpdateMucDoChoRequest
        {
            public int HoaDonId { get; set; }
            public int MucDoCho { get; set; }
        }

    }
}
