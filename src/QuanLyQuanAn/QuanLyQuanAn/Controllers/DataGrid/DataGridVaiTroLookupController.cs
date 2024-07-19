using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridVaiTroLookupController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public List<VaiTro> VaiTros { get; set; } = new List<VaiTro>();

        public DataGridVaiTroLookupController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var vaitro = _context.VaiTros.Select(vt => new
            {
                vt.Id,
                vt.TenVaiTro,
            });

            var loadResult = await DataSourceLoader.LoadAsync(vaitro, loadOptions);
            return Json(loadResult);
        }
    }
}
