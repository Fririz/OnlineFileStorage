using FileApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileApiService.Infrastructure.Persistence;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options) : base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //AccessRightsRepository
        modelBuilder.Entity<AccessRights>()
            .HasIndex(ar => new { ar.UserId, ar.ItemId })
            .IsUnique();
        //Item
        modelBuilder.Entity<Item>().HasQueryFilter(i => !i.IsDeleted);
        
        modelBuilder.Entity<Item>()
            .HasIndex(i => new { i.OwnerId, i.ParentId, i.Name })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
        
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Parent) 
            .WithMany(i => i.Children) 
            .HasForeignKey(i => i.ParentId) 
            .OnDelete(DeleteBehavior.Restrict); 
        
        modelBuilder.Entity<Item>(entity =>
        {
            entity.Property(u => u.Type)
                .HasConversion<string>();
            entity.Property(u => u.Status)
                .HasConversion<string>();
        });


    }
    public DbSet<Item> Items { get; set; }
    public DbSet<AccessRights> AccessRights { get; set; }
}