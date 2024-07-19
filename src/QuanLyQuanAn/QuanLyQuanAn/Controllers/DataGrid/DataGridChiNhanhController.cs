using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridChiNhanhController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DataGridChiNhanhController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        // Ban b { get; set; }
        public string role { get; set; }
        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var chinhanh = _context.ChiNhanhs;

            var loadResult = await DataSourceLoader.LoadAsync(chinhanh, loadOptions);
            return Json(loadResult);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {

            var newChinhanh = new ChiNhanh();
            JsonConvert.PopulateObject(values, newChinhanh);

            if (!TryValidateModel(newChinhanh))
                return BadRequest(ModelState);
            

            _context.ChiNhanhs.Add(newChinhanh);
            await _context.SaveChangesAsync();
            return Ok(newChinhanh);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values)
        {
            var chinhanh = await _context.ChiNhanhs.FirstOrDefaultAsync(cn => cn.Id == key);
            if (chinhanh == null)
                return StatusCode(409, "Chi nhánh không tồn tại.");

            //var updatedNhanVien = new NhanVien();
            JsonConvert.PopulateObject(values, chinhanh);

            if (!TryValidateModel(chinhanh))
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(chinhanh);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int key)
        {
            var chinhanh = await _context.ChiNhanhs.FirstOrDefaultAsync(cn => cn.Id == key);
            if (chinhanh == null)
                return StatusCode(409, "Chi nhánh không tồn tại.");

            _context.ChiNhanhs.Remove(chinhanh);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
