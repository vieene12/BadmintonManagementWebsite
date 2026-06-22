using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using AquarSmartCourt.Models;
using AquarSmartCourt.Hubs;
using Microsoft.Extensions.DependencyInjection;
using AquarSmartCourt.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();
// Đăng ký dịch vụ Gemini AI vào hệ thống
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<AquarSmartCourt.Services.GeminiService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    if (db.Database.IsSqlServer() || db.Database.ProviderName?.Contains("SqlServer") == true)
    {
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('MatchmakingGroups', RESEED)");
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('MatchmakingParticipants', RESEED)");
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Bookings', RESEED)");
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Invoices', RESEED)");
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('InvoiceDetails', RESEED)");
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Users', RESEED)");
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Courts', RESEED)");
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('ServiceItems', RESEED)");
        db.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('SurveillanceVideos', RESEED)");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<CourtHub>("/courtHub");

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
