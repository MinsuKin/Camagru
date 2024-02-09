using Microsoft.EntityFrameworkCore;
using camagru.Models;
using camagru.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// For MongoDB
builder.Services.Configure<BookStoreDatabaseSettings>(
    builder.Configuration.GetSection("BookStoreDatabase"));

builder.Services.AddSingleton<BooksService>();

// For saving user info in mongoDB
builder.Services.AddSingleton<UserService>();

builder.Services.AddControllers()
                .AddJsonOptions(
                    options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

// for CRUD
builder.Services.AddDbContext<TodoContext>(opt =>
    opt.UseInMemoryDatabase("TodoList"));


// for Identity
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.AddAuthorizationBuilder();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseInMemoryDatabase("AppDb"));

builder.Services.AddIdentityCore<MyUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapIdentityApi<MyUser>();

app.UseDefaultFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
// app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        Console.WriteLine($"Serving Files from :{context.File.PhysicalPath}");
        if (!context.Context.User.Identity.IsAuthenticated &&
            !Path.HasExtension(context.File.Name)) {
            // Redirect to index.html if not authenticated, but not for static files
            context.Context.Response.Redirect("/index.html");
        }
    }
});
app.UseFileServer(new FileServerOptions
{
    FileProvider = new CompositeFileProvider(
        new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "wwwrootauth"))
    ),
    EnableDefaultFiles = true
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapRazorPages();
// });

app.Run();