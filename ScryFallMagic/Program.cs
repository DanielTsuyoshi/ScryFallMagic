using ScryFallMagic.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adicione servi�os ao cont�iner.
builder.Services.AddControllersWithViews();

// Adicione o DbContext com a sua conex�o Oracle
builder.Services.AddDbContext<OracleDbContext>(options => {
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Adicione o servi�o IMemoryCache
builder.Services.AddMemoryCache(); // Adicione esta linha

var app = builder.Build();

// Configure o pipeline de solicita��o HTTP.
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
