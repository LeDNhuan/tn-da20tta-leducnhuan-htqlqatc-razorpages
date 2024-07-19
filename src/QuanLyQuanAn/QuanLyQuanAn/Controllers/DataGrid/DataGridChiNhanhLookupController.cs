using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using QuanLyQuanAn.Models;
using System.Data;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridChiNhanhLookupController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public List<VaiTro> VaiTros { get; set; } = new List<VaiTro>();

        public DataGridChiNhanhLookupController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }
        public string role { get; set;}


        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            role = HttpContext.Session.GetString("Role");
            if (role != "QuanLyTong")
            {
                var branch = HttpContext.Session.GetInt32("BranchId");
                var chinhanh = _context.ChiNhanhs.Where(cn => cn.Id == branch).Select(cn => new
                {
                    cn.Id,
                    cn.TenChiNhanh
                });

                var loadResult = await DataSourceLoader.LoadAsync(chinhanh, loadOptions);
                return Json(loadResult);
            }
            else
            {
                var chinhanh = _context.ChiNhanhs.Select(cn => new
                {
                    cn.Id,
                    cn.TenChiNhanh
                });

                var loadResult = await DataSourceLoader.LoadAsync(chinhanh, loadOptions);
                return Json(loadResult);
            }
            
        }
    }
}
