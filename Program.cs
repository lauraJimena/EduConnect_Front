using EduConnect_Front.Services;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Expira tras 20 min de inactividad
    options.Cookie.HttpOnly = true; // Protege contra scripts
    options.Cookie.IsEssential = true; // Necesario para funcionamiento básico
});



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
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ❗ obliga a HTTPS
    options.Cookie.IsEssential = true;              // requerido si usas GDPR/cookie consent
});

// Configuración Rotativa (también antes del Build)
Rotativa.AspNetCore.RotativaConfiguration.Setup(builder.Environment.ContentRootPath, "Rotativa");

var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}
if (!app.Environment.IsDevelopment())
{
    // Manejo centralizado de errores
    app.UseExceptionHandler("/Error/500");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}
else
{
    // En desarrollo puedes mantener las páginas de error detalladas
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.Use(async (context, next) =>
{
    await next();

    // Si no se encontró la ruta (404) y aún no se ha iniciado una respuesta
    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Request.Path = "/Error/404"; // redirige al controlador de error
        await next();
    }
});


app.UseRouting();

//Habilitar Session 
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Inicio}/{id?}");

app.Run();
