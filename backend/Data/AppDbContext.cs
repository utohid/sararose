using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Models;

namespace SaraRose.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<EquipmentCategory> Categories => Set<EquipmentCategory>();
    public DbSet<EquipmentItem> Equipment => Set<EquipmentItem>();
    public DbSet<Enquiry> Enquiries => Set<Enquiry>();
    public DbSet<SliderSlide> SliderSlides => Set<SliderSlide>();
    public DbSet<HeaderLink> HeaderLinks => Set<HeaderLink>();
    public DbSet<UserRegistration> Registrations => Set<UserRegistration>();

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

        modelBuilder.Entity<SliderSlide>(entity =>
        {
            entity.ToTable("slider_slides");
            entity.Property(x => x.Alt).HasMaxLength(200);
            entity.Property(x => x.FileName).HasMaxLength(180);
            entity.Property(x => x.ContentType).HasMaxLength(80);
        });

        modelBuilder.Entity<HeaderLink>(entity =>
        {
            entity.ToTable("header_links");
            entity.Property(x => x.Label).HasMaxLength(80);
            entity.Property(x => x.Path).HasMaxLength(240);
        });

        modelBuilder.Entity<UserRegistration>(entity =>
        {
            entity.ToTable("registrations");
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.FullName).HasMaxLength(120);
            entity.Property(x => x.Email).HasMaxLength(160);
            entity.Property(x => x.Phone).HasMaxLength(40);
            entity.Property(x => x.Company).HasMaxLength(160);
            entity.Property(x => x.City).HasMaxLength(120);
            entity.Property(x => x.Role).HasMaxLength(40);
            entity.Property(x => x.UserType).HasMaxLength(40);
            entity.Property(x => x.PasswordHash).HasMaxLength(64);
        });
    }
}
