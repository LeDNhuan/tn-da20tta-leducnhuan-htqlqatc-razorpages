using Microsoft.EntityFrameworkCore;
using QuanLyQuanAn.Models;
using QuanLyQuanAn.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services
    .AddRazorPages()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

//Ket noi co so du lieu
builder.Services.AddDbContext<QuanLyQuanAnContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    options.UseSqlServer(connectionString);
});


builder.Services.AddScoped<ChiNhanhService>();

builder.Services.AddRazorPages();

// ------------Session--------------
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout
    options.Cookie.HttpOnly = true; // Chỉ sử dụng cookie qua HTTP
    options.Cookie.IsEssential = true; // Đánh dấu cookie là cần thiết
});

builder.Services.AddDistributedMemoryCache(); // Sử dụng cache trong bộ nhớ
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Sử dụng session
app.UseSession(); // Đảm bảo gọi UseSession trước UseEndpoints hoặc UseMvc

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
