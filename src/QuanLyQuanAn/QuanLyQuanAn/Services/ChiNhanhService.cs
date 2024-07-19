using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;

namespace QuanLyQuanAn.Services
{
    public class ChiNhanhService
    {
        private readonly IWebHostEnvironment _env;
        private readonly QuanLyQuanAnContext _context;

        public ChiNhanhService(IWebHostEnvironment environment, QuanLyQuanAnContext context)
        {
            _env = environment;
            _context = context;
        }

        public async Task<List<ChiNhanh>> GetChiNhanhsAsync()
        {
            return await _context.ChiNhanhs.ToListAsync();
        }
    }
}
