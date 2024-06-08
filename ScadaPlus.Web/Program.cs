using Microsoft.EntityFrameworkCore;
using ScadaPlus.Data.Contexts;
using ScadaPlus.Web.Data;
using MudBlazor.Services;
using ScadaPlus.Data.Repositories;
using ScadaPlus.Web.States;
using ScadaPlus.Web.BackgroundServices;
using Microsoft.AspNetCore.SignalR;
using ScadaPlus.Web.Hubs;
using ScadaPlus.Data.Identity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddDbContext<ScadaPlusDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<ScadaPlusDbContextFactory>();

builder.Services.AddIdentity<ScadaPlusIdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ScadaPlusDbContext>();

builder.Services.AddScoped<MachineRepository>();
builder.Services.AddScoped<JobRepository>();

builder.Services.AddSingleton<ApplicationState>();

builder.Services.AddHostedService<ApplicationBackgroundService>();

builder.Services.AddSignalR();

builder.Services.AddControllersWithViews();

builder.Services.AddFastReport();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseFastReport();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapHub<Hub1>("/hub-1");
app.MapHub<Hub2>("/hub-2");
app.MapHub<Hub3>("/hub-3");
app.MapFallbackToPage("/_Host");

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
