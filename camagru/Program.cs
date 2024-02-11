using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;


using camagru.Models;
using camagru.Services;
using camagru.Utilities;

namespace camagru
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            SetEnvironment();
            var builder = Configure(args);
            StartApplication(builder);

        }

        public static void SetEnvironment()
        {
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);
        }

        public static WebApplicationBuilder Configure(string[] args)
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

            builder = ConfigureJwt(builder);

            // for CRUD
            builder.Services.AddDbContext<TodoContext>(opt =>
                opt.UseInMemoryDatabase("TodoList"));


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            return builder;
        }

        public static WebApplicationBuilder ConfigureJwt(WebApplicationBuilder builder)
        {
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
            return builder;
        }

        public static void StartApplication(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            app.UseAuthentication();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    Console.WriteLine($"Serving Files from :{context.File.PhysicalPath}");
                    if (!context.Context.User.Identity.IsAuthenticated &&
                        !Path.HasExtension(context.File.Name)) {
                        context.Context.Response.Redirect("/");
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

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
