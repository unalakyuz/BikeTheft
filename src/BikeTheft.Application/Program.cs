using BikeTheft.Application.Filters;
using BikeTheft.Service;
using BikeTheft.Service.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IBikeTheftService, BikeTheftService>();

builder.Services.Configure<BikeIndexApiSettings>(builder.Configuration.GetSection(nameof(BikeIndexApiSettings)));

builder.Services.AddHttpClient("BikeIndex");

builder.Services.AddMvc(options =>
{
    options.Filters.Add<HttpGlobalExceptionFilter>();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
