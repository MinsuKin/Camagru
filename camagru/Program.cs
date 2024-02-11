using System.Text;
using Microsoft.EntityFrameworkCore;
using camagru.Models;
using camagru.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using camagru.Utility;
using camagru.Application.Stores;
using camagru.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace camagru {
    public static class Program
    {
        public static void Main(string[] args)
        {
            SetEnvironment();
            var builder = ConfigureAndBuild(args);
            StartApplication(builder);
        }

        public static WebApplicationBuilder ConfigureAndBuild(string[] args)
        {
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
            // For Identity in mongo
            builder.Services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtKey").ToString())),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

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

            return builder;
        }

        public static void SetEnvironment()
        {
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);
        }

        public static void StartApplication(WebApplicationBuilder builder)
        {
            var app = builder.Build();
            app.MapIdentityApi<MyUser>();

            app.UseAuthentication();
            app.UseDefaultFiles();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    Console.WriteLine($"Serving Files from :{context.File.PhysicalPath}");
                    // Use later from protecting content
                    // if (!context.Context.User.Identity.IsAuthenticated &&
                    //     !Path.HasExtension(context.File.Name)) {
                    //     // Redirect to index.html if not authenticated, but not for static files
                    //     context.Context.Response.Redirect("/index.html");
                    // }
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

            app.UseAuthorization();

            app.MapControllers();
            app.UseWebSockets();
            app.Run();
        }
    }
}