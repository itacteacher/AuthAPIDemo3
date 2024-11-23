using AuthAPIDemo3.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthAPIDemo3.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext (DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Ticket> Tickets { get; set; }
}
