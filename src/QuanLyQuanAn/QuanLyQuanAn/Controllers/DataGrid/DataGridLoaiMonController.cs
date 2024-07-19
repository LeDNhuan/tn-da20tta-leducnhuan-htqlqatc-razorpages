using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridLoaiMonController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DataGridLoaiMonController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var loaimon = _context.LoaiMons;

            var loadResult = await DataSourceLoader.LoadAsync(loaimon, loadOptions);
            return Json(loadResult);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            var newLoaiMon = new LoaiMon();
            JsonConvert.PopulateObject(values, newLoaiMon);

            if (!TryValidateModel(newLoaiMon))
                return BadRequest(ModelState);

            _context.LoaiMons.Add(newLoaiMon);
            await _context.SaveChangesAsync();
            return Ok(newLoaiMon);

        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values)
        {
            var loaimon = await _context.LoaiMons.FirstOrDefaultAsync(lm => lm.Id == key);
            if (loaimon == null)
                return StatusCode(409, "Loại món không tồn tại.");

            JsonConvert.PopulateObject(values, loaimon);

            if (!TryValidateModel(loaimon))
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(loaimon);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int key)
        {
            var loaimon = await _context.LoaiMons.FirstOrDefaultAsync(lm => lm.Id == key);
            if (loaimon == null)
                return StatusCode(409, "Loại món không tồn tại.");

            _context.LoaiMons.Remove(loaimon);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
