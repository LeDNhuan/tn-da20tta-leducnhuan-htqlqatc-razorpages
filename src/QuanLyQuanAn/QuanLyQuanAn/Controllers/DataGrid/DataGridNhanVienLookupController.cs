using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridNhanVienLookupController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public List<VaiTro> VaiTros { get; set; } = new List<VaiTro>();

        public DataGridNhanVienLookupController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var nhanvien = _context.NhanViens.Select(nv => new
            {
                nv.Id,
                nv.HoTen,
                nv.IdChiNhanh,
                TenChiNhanh = nv.IdChiNhanhNavigation.TenChiNhanh,
            });

            var loadResult = await DataSourceLoader.LoadAsync(nhanvien, loadOptions);
            return Json(loadResult);
        }
    }
}
