using TT.Domain;
using TT.Domain.Concrete;
using TT.Server.Conventions;
using TT.Server.Features.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("localsettings.json", optional: true, reloadOnChange: true);

var configuration = builder.Configuration;

DomainRegistry.SetConectionStringOrName(configuration.GetConnectionString("StatsWebConnection"));
StatsConnectionStringProvider.ConnectionStringOrName = configuration.GetConnectionString("StatsWebConnection");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddFeatureConventions();
builder.Services.AddIdentity(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();