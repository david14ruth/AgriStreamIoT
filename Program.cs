
using AgriStreamIoT.Data;
using AgriStreamIoT.Services;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AzureStorageService>();

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MVC services
builder.Services.AddControllersWithViews();

// Session configuration
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton(x =>
    new BlobServiceClient(
        builder.Configuration["AzureStorage"]));

builder.Services.AddScoped<BlobStorageService>();
builder.Services.AddSingleton(sp =>
    new TableStorageService(
        builder.Configuration["AzureStorage"]!));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var storageService = scope.ServiceProvider
        .GetRequiredService<AzureStorageService>();

    await storageService.InitializeStorageAsync();
}

// Enable Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// Register API controllers
app.MapControllers();

// Register MVC controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
