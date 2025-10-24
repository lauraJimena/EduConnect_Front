using EduConnect_Front.Services;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// ?? Registro de servicios personalizados
builder.Services.AddScoped<API_Service>();
builder.Services.AddScoped<TutorService>();
builder.Services.AddScoped<ChatService>();


// Cache para Session (requerido)
builder.Services.AddDistributedMemoryCache();


// 3) Configurar Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // tiempo de inactividad
    options.Cookie.HttpOnly = true;                 // por seguridad
    options.Cookie.IsEssential = true;              // requerido si usas GDPR/cookie consent
});

// ✅ Configuración Rotativa (también antes del Build)
Rotativa.AspNetCore.RotativaConfiguration.Setup(builder.Environment.ContentRootPath, "Rotativa");

// ✅ Ahora sí se construye la app
var app = builder.Build();

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

//Habilitar Session 
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Inicio}/{id?}");

app.Run();
