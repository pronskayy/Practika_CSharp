using WatchList.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Регистрация сервиса в DI (один экземпляр на всё приложение)
builder.Services.AddSingleton<IWatchlistService, WatchlistService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Watchlist}/{action=Index}/{id?}");

app.Run();