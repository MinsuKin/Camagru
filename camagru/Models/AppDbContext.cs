using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace camagru.Models;

class AppDbContext(DbContextOptions<AppDbContext> options) : 
    IdentityDbContext<MyUser>(options)
{
}