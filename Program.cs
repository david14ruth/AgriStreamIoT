
using AgriStreamIoT.Data;
using AgriStreamIoT.Services;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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

builder.Services.AddSingleton(sp =>
    new QueueStorageService(
        builder.Configuration["AzureStorage"]!));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var storageService =
        services.GetRequiredService<AzureStorageService>();

    await storageService.InitializeStorageAsync();

    var tableStorage =
        services.GetRequiredService<TableStorageService>();

    await tableStorage.InitializeAsync();

    var queueStorage =
        services.GetRequiredService<QueueStorageService>();

    await queueStorage.InitializeAsync();
}
/*```

This ensures the tables and queue exist when the application starts.

## Step 6.3 – Test the application

1. Save all files.
2. Press `Ctrl + Shift + B`.
3. If successful, run the application.

<badge color="success">Expected result</badge> The application should start and initialize the Azure Storage resources.

**One important thing before we continue:**Please send me your existing `SensorController.cs` code next.

We'll integrate Azure Table Storage into the existing sensor submission functionality without breaking the SQL Server functionality we've already implemented.
*/
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
