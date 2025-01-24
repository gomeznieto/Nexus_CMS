using Backend_portafolio.Entities;
using Backend_portafolio.Services;
using Backend_portafolio.Datos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Backend_portafolio.Sevices;
using Backend_portafolio.Models;

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


//****************************************************
//***************** USER SERVICES ********************
//****************************************************
builder.Services.AddTransient<IUserStore<User>, UsersStore>();
builder.Services.AddTransient<SignInManager<User>>();


//****************************************************
//*********************** DATA ***********************
//****************************************************
builder.Services.AddTransient<IRepositoryCategorias, RepositoryCategorias>();
builder.Services.AddTransient<IRepositoryFormat, RepositoryFormat>();
builder.Services.AddTransient<IRepositoryPosts, RepositoryPosts>();
builder.Services.AddTransient<IRepositoryMedia, RepositoryMedia>();
builder.Services.AddTransient<IRepositoryMediatype, RepositoryMediatype>();
builder.Services.AddTransient<IRepositorySource, RepositorySource>();
builder.Services.AddTransient<IRepositoryLink, RepositoryLink>();
builder.Services.AddTransient<IRepositoryRole, RepositoryRole>();
builder.Services.AddTransient<IRepositoryUsers, RepositoryUsers>();
builder.Services.AddTransient<IRepositoryBio, RepositoryBio>();
builder.Services.AddTransient<IRepositorySocialNetwork, RepositorySocialNetwork>();


//****************************************************
//********************* SERVICES *********************
//****************************************************
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddTransient<ICategoriaService, CategoriaService>();
builder.Services.AddTransient<IPostService, PostService>();
builder.Services.AddTransient<IFormatService, FormatService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IMediaTypeService, MediaTypeService>();
builder.Services.AddTransient<ISourceService, SourceService>();
builder.Services.AddTransient<ILinkService, LinkService>();
builder.Services.AddTransient<IMediaService, MediaService>();
builder.Services.AddTransient<IHomeService, HomeService>();
builder.Services.AddTransient<IBioService, BioService>();
builder.Services.AddTransient<INetworkService, NetworkService>();
builder.Services.AddTransient<IApiService, ApiService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddTransient<IRoleService, RoleService>();


//****************************************************
//********************** AUTH ************************
//****************************************************
builder.Services.AddIdentityCore<User>(opciones =>
{
	opciones.Password.RequireUppercase = false;
	opciones.Password.RequireLowercase = false;
	opciones.Password.RequireDigit = false;
	opciones.Password.RequireNonAlphanumeric = false;
	opciones.SignIn.RequireConfirmedAccount = true;
});


//****************************************************
//********************* COOKIES **********************
//****************************************************
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

//****************************************************
//********************** MAPPER **********************
//****************************************************
builder.Services.AddAutoMapper(typeof(Program));


//****************************************************
//******************** ENCRYPTION ********************
//****************************************************

builder.Services.AddSingleton<IEncryptionService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var encryptionKey = configuration["Encryption:Key"];
    var encryptionIV = configuration["Encryption:IV"];

    return new EncryptionService(encryptionKey, encryptionIV);
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

//*** TEST *****///

// Crear el primer usuario administrador
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var userService = services.GetRequiredService<IUsersService>();
//        await CreateAdminUserAsync(userService);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"Error al crear el usuario administrador: {ex.Message}");
//    }
//}

// Configuración del middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


/// ***** FIN TEST ****///

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


//// Método para crear el usuario administrador
//static async Task CreateAdminUserAsync(IUsersService userService)
//{
//    // Verificar si ya existe el usuario administrador
//    User adminUser = await userService.GetUserByUser("admin");

//    if (adminUser == null)
//    {
//        // Crear el usuario administrador
//        var newUser = new RegisterViewModel()
//        {
//            Username = "admin",
//            Email = "admin@example.com",
//            Password = "PasswordSegura123!",
//            role = 1,
//        };

//        var result = await userService.CreateUserAsync(newUser);
//        if (result)
//        {
//            Console.WriteLine("Primer usuario administrador creado con éxito.");
//        }
//        else
//        {
//            Console.WriteLine("Error al crear el usuario administrador.");
//        }
//    }
//    else
//    {
//        Console.WriteLine("El usuario administrador ya existe.");
//    }
//}