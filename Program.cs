using CivicResolve.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// State Management: Distributed memory cache + Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".CivicResolve.Session";
});

// Database connectivity using Entity Framework Core and MySQL (with intelligent SQLite fallback)
var dbProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Auto";
var mySqlConnectionString = builder.Configuration.GetConnectionString("MySqlConnection");
var sqliteConnectionString = builder.Configuration.GetConnectionString("SqliteConnection") ?? "Data Source=civicresolve.db";

bool useMySql = false;

if (dbProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase) || dbProvider.Equals("Auto", StringComparison.OrdinalIgnoreCase))
{
    try
    {
        using var tcpClient = new System.Net.Sockets.TcpClient();
        var result = tcpClient.BeginConnect("localhost", 3306, null, null);
        var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500));
        if (success && tcpClient.Connected)
        {
            tcpClient.EndConnect(result);
            useMySql = true;
        }
    }
    catch
    {
        useMySql = false;
    }
}

if (useMySql && !string.IsNullOrEmpty(mySqlConnectionString))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(mySqlConnectionString, ServerVersion.AutoDetect(mySqlConnectionString)));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(sqliteConnectionString));
}

var app = builder.Build();

// Ensure upload directory exists
var webRoot = app.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
var uploadsPath = Path.Combine(webRoot, "uploads", "complaints");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// Initialize Database & Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Session State
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
