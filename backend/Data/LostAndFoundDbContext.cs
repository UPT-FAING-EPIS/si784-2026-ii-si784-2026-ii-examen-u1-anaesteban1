using LostAndFound.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LostAndFound.Api.Data;

public sealed class LostAndFoundDbContext(DbContextOptions<LostAndFoundDbContext> options)
    : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();

    public DbSet<Claim> Claims => Set<Claim>();

    public DbSet<ReturnRecord> ReturnRecords => Set<ReturnRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("items");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.ItemType).HasConversion<string>().HasMaxLength(20);
            entity.Property(item => item.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasMany(item => item.Claims)
                .WithOne(claim => claim.Item)
                .HasForeignKey(claim => claim.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(item => item.ReturnRecord)
                .WithOne(record => record.Item)
                .HasForeignKey<ReturnRecord>(record => record.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Claim>(entity =>
        {
            entity.ToTable("claims");
            entity.HasKey(claim => claim.Id);
            entity.HasIndex(claim => claim.ItemId);
        });

        modelBuilder.Entity<ReturnRecord>(entity =>
        {
            entity.ToTable("return_records");
            entity.HasKey(record => record.Id);
            entity.HasIndex(record => record.ClaimId).IsUnique();
            entity.HasOne(record => record.Claim)
                .WithOne(claim => claim.ReturnRecord)
                .HasForeignKey<ReturnRecord>(record => record.ClaimId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
