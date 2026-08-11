using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.Abstraction;
using myshop.DAL;
using myshop.DAL.Data;
using myshop.DataAccess;
using myshop.Entities.Models;
using myshop.Web.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    )) ;
builder.Services.AddAutoMapper(cfg => { }, typeof(myshop.BLL.Mappers.ProductProfile).Assembly);

builder.Services.AddIdentity<ApplicationUser,IdentityRole>(
    options=>
    {
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(4);
        options.SignIn.RequireConfirmedEmail = true;
    }).AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<IAuthorizationHandler, ActiveAccountAuthorizationHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.ActiveAccount, policy =>
        policy.Requirements.Add(new ActiveAccountRequirement()));
});

builder.Services.AddScoped<IAuthService, myshop.BLL.Services.AuthService>();
builder.Services.AddScoped<IEmailService, myshop.BLL.Services.EmailService>();
builder.Services.AddScoped<IUserService, myshop.BLL.Services.UserService>();
builder.Services.AddScoped<IFileService, myshop.BLL.Services.FileService>();
builder.Services.AddScoped<IUnitOfWork, myshop.DAL.UnitOfWork>();
builder.Services.AddScoped<IProductService, myshop.BLL.Services.ProductService>();
builder.Services.AddScoped<ICategoryService, myshop.BLL.Services.CategoryService>();

builder.Services.AddHttpContextAccessor();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();
    var context = scope.ServiceProvider
       .GetRequiredService<ApplicationDbContext>();

    await DbInitializer.SeedAsync(context,roleManager);
}




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


app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.Run();

