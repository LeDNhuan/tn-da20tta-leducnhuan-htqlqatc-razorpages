using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridHoaDonController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        //public List<MonGoi> MonAns { get; set; } = new List<MonGoi>();
        //public List<LoaiMon> LoaiMons { get; set; } = new List<LoaiMon>();
        public List<CaLamViec> CaLams { get; set; } = new List<CaLamViec>();
        public List<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        //public HoaDon HoaDon { get; set; }
        //public int HoaDonId { get; set; }

        public DataGridHoaDonController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }
        [HttpGet]
        public IActionResult Get(DataSourceLoadOptions loadOptions)
        {
            var userId = HttpContext.Session.GetString("UserId");
            //var user = _context.NhanViens.FirstOrDefault(nv => nv.Id == userId);
                
            //var calams = _context.CaLamViecs
            //        .Where(clv => clv.NhanViens.Contains(user))
            //        .ToList();
            var hoadons = _context.HoaDons.ToList();

            return Json(DataSourceLoader.Load(hoadons, loadOptions));
        }
    }
}
