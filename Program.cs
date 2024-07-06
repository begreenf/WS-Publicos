using MyWebApp.Services;
using MyWebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSingleton(new AuditLogService(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton(new QueryLogService(builder.Configuration.GetConnectionString("DefaultConnection"))); // Registrar el nuevo servicio

// Agregar SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configurar middleware de la aplicación
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

// Mapear SignalR hub
app.MapHub<AuditHub>("/auditHub");

app.Run();
