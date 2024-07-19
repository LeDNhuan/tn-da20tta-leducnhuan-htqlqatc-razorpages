using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QuanLyQuanAn.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyQuanAn.Controllers.DataGrid
{
    public class DataGridBanController : Controller
    {
        private readonly QuanLyQuanAnContext _context;
        private readonly IWebHostEnvironment _env;

        public DataGridBanController(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _context = context;
            _env = environment;
        }

        public Ban b { get; set; }
        public string role { get; set; }

        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            role = HttpContext.Session.GetString("Role");
            if (role != "QuanLyTong")
            {
                var branch = HttpContext.Session.GetInt32("BranchId");
                var ban = _context.Bans.Where(b => b.IdChiNhanh == branch);

                var loadResult = await DataSourceLoader.LoadAsync(ban, loadOptions);
                return Json(loadResult);
            }
            else
            {
                var ban = _context.Bans;

                var loadResult = await DataSourceLoader.LoadAsync(ban, loadOptions);
                return Json(loadResult);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Post(string values)
        {

            var newBan = new Ban();
            JsonConvert.PopulateObject(values, newBan);

            if (!TryValidateModel(newBan))
                return BadRequest(ModelState);

            newBan.CreatedAt = DateTime.Now;
            newBan.TinhTrang = "Trong";

            _context.Bans.Add(newBan);
            await _context.SaveChangesAsync();
            return Ok(newBan);
        }

        [HttpPut]
        public async Task<IActionResult> Put(int key, string values)
        {
            var ban = await _context.Bans.FirstOrDefaultAsync(b => b.Id == key);
            if (ban == null)
                return StatusCode(409, "Bàn không tồn tại.");

            //var updatedNhanVien = new NhanVien();
            JsonConvert.PopulateObject(values, ban);
            
            if (!TryValidateModel(ban))
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok(ban);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int key)
        {
            var ban = await _context.Bans.FirstOrDefaultAsync(b => b.Id == key);
            if (ban == null)
                return StatusCode(409, "Bàn không tồn tại.");

            _context.Bans.Remove(ban);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
