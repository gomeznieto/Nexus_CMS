using Backend_portafolio.Models;
using Backend_portafolio.Sevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Politica de seguridad
var politicaUsuarioAutenticado = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
	options.Filters.Add(new AuthorizeFilter(politicaUsuarioAutenticado));
});
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
builder.Services.AddTransient<IRepositoryRole, RepositoryRole>();
builder.Services.AddTransient<IRepositoryUsers, RepositoryUsers>();
builder.Services.AddTransient<IRepositoryBio, RepositoryBio>();
builder.Services.AddTransient<IUserStore<User>, UsersStore>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<SignInManager<User>>();

// AUTENTICACION
builder.Services.AddIdentityCore<User>(opciones =>
{
	opciones.Password.RequireUppercase = false;
	opciones.Password.RequireLowercase = false;
	opciones.Password.RequireDigit = false;
	opciones.Password.RequireNonAlphanumeric = false;
	opciones.SignIn.RequireConfirmedAccount = true;
});


// COOKIES
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
	options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
	options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, options =>
{
	options.LoginPath = "/users/login";
});

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

app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
