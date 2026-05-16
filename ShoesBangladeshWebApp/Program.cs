using ShoesBangladesh.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization()
    .ConfigureApplicationPartManager(manager =>
    {
        var apiAssembly = manager.ApplicationParts.FirstOrDefault(p => p.Name == "ShoesBangladeshApi");
        if (apiAssembly != null)
        {
            manager.ApplicationParts.Remove(apiAssembly);
        }
    });

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddHttpClient("ShoesAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5190/");
});

builder.Services.AddScoped<ICategoryApiService, CategoryApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
