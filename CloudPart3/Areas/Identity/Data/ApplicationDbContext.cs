using CloudPart3.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CloudPart3.Models;

namespace CloudPart3.Areas.Identity.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

public DbSet<CloudPart3.Models.Product> Product { get; set; } = default!;

public DbSet<CloudPart3.Models.Order> Order { get; set; } = default!;

public DbSet<CloudPart3.Models.Cart> Cart { get; set; } = default!;

public DbSet<CloudPart3.Models.Admin> Admin { get; set; } = default!;

public DbSet<CloudPart3.Models.Contract> Contract { get; set; } = default!;
}
