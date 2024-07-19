using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridKhuyenMaiController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DataGridKhuyenMaiController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public KhuyenMai km { get; set; }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var khuyenmai = _context.KhuyenMais;

            var loadResult = await DataSourceLoader.LoadAsync(khuyenmai, loadOptions);
            return Json(loadResult);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {

            var newKhuyenMai = new KhuyenMai();
            JsonConvert.PopulateObject(values, newKhuyenMai);

            if (!TryValidateModel(newKhuyenMai))
                return BadRequest(ModelState);

            newKhuyenMai.CreatedAt = DateTime.Now;

            _context.KhuyenMais.Add(newKhuyenMai);
            await _context.SaveChangesAsync();
            return Ok(newKhuyenMai);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values)
        {
            var khuyenmai = await _context.KhuyenMais.FirstOrDefaultAsync(km => km.Id == key);
            if (khuyenmai == null)
                return StatusCode(409, "CT khuyến mãi không tồn tại.");

            //var updatedNhanVien = new NhanVien();
            JsonConvert.PopulateObject(values, khuyenmai);

            if (!TryValidateModel(khuyenmai))
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(khuyenmai);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int key)
        {
            var khuyenmai = await _context.KhuyenMais.FirstOrDefaultAsync(km => km.Id == key);
            if (khuyenmai == null)
                return StatusCode(409, "CT khuyến mãi không tồn tại.");

            _context.KhuyenMais.Remove(khuyenmai);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
