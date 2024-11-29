using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

//Modelos
builder.Services.AddTransient<IRepositoryCategorias, RepositoryCategorias>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<IRepositoryFormat, RepositoryFormat>();
builder.Services.AddTransient<IRepositoryPosts, RepositoryPosts>();
builder.Services.AddTransient<IRepositoryMedia, RepositoryMedia>();
builder.Services.AddTransient<IRepositoryMediatype, RepositoryMediatype>();
builder.Services.AddTransient<IRepositorySource, RepositorySource>();
builder.Services.AddTransient<IRepositoryLink, RepositoryLink>();
builder.Services.AddTransient<IRepositoryUsers, RepositoryUsers>();
builder.Services.AddTransient<IUserStore<User>, UsersStore>();
builder.Services.AddIdentityCore<User>();
builder.Services.AddHttpContextAccessor();

//Auto Mapper
builder.Services.AddAutoMapper(typeof(Program));

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
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
