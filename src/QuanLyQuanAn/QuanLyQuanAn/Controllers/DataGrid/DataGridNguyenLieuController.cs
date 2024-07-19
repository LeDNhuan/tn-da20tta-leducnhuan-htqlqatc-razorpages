using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridNguyenLieuController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DataGridNguyenLieuController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }


        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var nguyenlieu = _context.NguyenLieus;

            var loadResult = await DataSourceLoader.LoadAsync(nguyenlieu, loadOptions);
            return Json(loadResult);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {
            var newNguyenLieu = new NguyenLieu();
            JsonConvert.PopulateObject(values, newNguyenLieu);

            if (!TryValidateModel(newNguyenLieu))
                return BadRequest(ModelState);

            _context.NguyenLieus.Add(newNguyenLieu);
            await _context.SaveChangesAsync();
            return Ok(newNguyenLieu);

        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values)
        {
            var nguyenlieu = await _context.NguyenLieus.FirstOrDefaultAsync(nl => nl.Id == key);
            if (nguyenlieu == null)
                return StatusCode(409, "Nguyên liệu không tồn tại.");

            JsonConvert.PopulateObject(values, nguyenlieu);

            if (!TryValidateModel(nguyenlieu))
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(nguyenlieu);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int key)
        {
            var nguyenlieu = await _context.NguyenLieus.FirstOrDefaultAsync(nl => nl.Id == key);
            if (nguyenlieu == null)
                return StatusCode(409, "Nguyên liệu không tồn tại.");

            _context.NguyenLieus.Remove(nguyenlieu);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
