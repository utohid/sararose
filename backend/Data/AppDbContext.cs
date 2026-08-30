using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Models;

namespace SaraRose.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EquipmentCategory> Categories => Set<EquipmentCategory>();
    public DbSet<EquipmentItem> Equipment => Set<EquipmentItem>();
    public DbSet<Enquiry> Enquiries => Set<Enquiry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EquipmentCategory>(entity =>
        {
            entity.ToTable("categories");
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.Property(x => x.Slug).HasMaxLength(80);
            entity.Property(x => x.Code).HasMaxLength(16);
            entity.Property(x => x.Name).HasMaxLength(160);
            entity.Property(x => x.ShortName).HasMaxLength(80);
            entity.Property(x => x.Summary).HasMaxLength(800);
        });

        modelBuilder.Entity<EquipmentItem>(entity =>
        {
            entity.ToTable("equipment");
            entity.HasIndex(x => x.Slug).IsUnique();
            entity.Property(x => x.Slug).HasMaxLength(80);
            entity.Property(x => x.Name).HasMaxLength(160);
            entity.Property(x => x.MachineType).HasMaxLength(80);
            entity.Property(x => x.Summary).HasMaxLength(400);
            entity.Property(x => x.Description).HasColumnType("text");
            entity.Property(x => x.TypicalUse).HasMaxLength(400);
            entity.Property(x => x.AvailabilityNote).HasMaxLength(400);
            entity.HasOne(x => x.Category)
                .WithMany(x => x.Equipment)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Enquiry>(entity =>
        {
            entity.ToTable("enquiries");
            entity.Property(x => x.FullName).HasMaxLength(120);
            entity.Property(x => x.Company).HasMaxLength(160);
            entity.Property(x => x.Phone).HasMaxLength(40);
            entity.Property(x => x.Email).HasMaxLength(160);
            entity.Property(x => x.MachineType).HasMaxLength(80);
            entity.Property(x => x.SiteLocation).HasMaxLength(160);
            entity.Property(x => x.Requirement).HasColumnType("text");
            entity.Property(x => x.Status).HasMaxLength(32);
            entity.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
