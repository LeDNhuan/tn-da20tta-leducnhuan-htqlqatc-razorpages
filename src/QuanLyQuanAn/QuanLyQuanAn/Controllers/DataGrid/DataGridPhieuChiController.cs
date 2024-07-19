using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridPhieuChiController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DataGridPhieuChiController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public PhieuChi pc { get; set; }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var phieuchi = _context.PhieuChis;

            var loadResult = await DataSourceLoader.LoadAsync(phieuchi, loadOptions);
            return Json(loadResult);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {

            var newPhieuChi = new PhieuChi();
            JsonConvert.PopulateObject(values, newPhieuChi);

            if (!TryValidateModel(newPhieuChi))
                return BadRequest(ModelState);

            newPhieuChi.CreatedAt = DateTime.Now;

            _context.PhieuChis.Add(newPhieuChi);
            await _context.SaveChangesAsync();
            return Ok(newPhieuChi);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values)
        {
            var phieuchi = await _context.PhieuChis.FirstOrDefaultAsync(pc => pc.Id == key);
            if (phieuchi == null)
                return StatusCode(409, "Phiếu chi không tồn tại.");

            //var updatedNhanVien = new NhanVien();
            JsonConvert.PopulateObject(values, phieuchi);

            if (!TryValidateModel(phieuchi))
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(phieuchi);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int key)
        {
            var phieuchi = await _context.PhieuChis.FirstOrDefaultAsync(pc => pc.Id == key);
            if (phieuchi == null)
                return StatusCode(409, "Phiếu chi không tồn tại.");

            _context.PhieuChis.Remove(phieuchi);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
