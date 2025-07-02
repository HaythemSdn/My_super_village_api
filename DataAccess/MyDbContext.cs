using Common.Dao;
using Microsoft.EntityFrameworkCore;

public class MyDbContext :DbContext
{
    public DbSet<UserDAO> Users { get; set; }
    public DbSet<UserBuildingDAO> UserBuildings { get; set; }
    public DbSet<UserResourceDAO> UserResources { get; set; }
    public DbSet<UserConstructionDAO> UserConstructions { get; set; }

    public MyDbContext(DbContextOptions<MyDbContext> options) :
        base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // === User ===
        var userBuilder = modelBuilder.Entity<UserDAO>();
        userBuilder.ToTable("Users");
        userBuilder.HasKey(u => u.Id);
        userBuilder.Property(u => u.Id).HasColumnName("id").HasColumnType("uuid");
        userBuilder.Property(u => u.Pseudo).HasColumnName("pseudo").HasMaxLength(1000).IsUnicode(true);

        // === Building ===
        var buildingBuilder = modelBuilder.Entity<UserBuildingDAO>();
        buildingBuilder.ToTable("UserBuildings");
        buildingBuilder.HasKey(b => b.Id);
        buildingBuilder.Property(b => b.Id).HasColumnName("id").HasColumnType("uuid");
        buildingBuilder.Property(b => b.Type).HasColumnName("type").HasConversion<string>();
        buildingBuilder.Property(b => b.Level).HasColumnName("level");
        buildingBuilder.Property(b => b.UserId).HasColumnName("user_id").HasColumnType("uuid");

        buildingBuilder
            .HasOne(b => b.User)
            .WithMany(u => u.Buildings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // === Resource ===
        var resourceBuilder = modelBuilder.Entity<UserResourceDAO>();
        resourceBuilder.ToTable("UserResources");
        resourceBuilder.HasKey(r => r.Id);
        resourceBuilder.Property(r => r.Id).HasColumnName("id").HasColumnType("uuid");
        resourceBuilder.Property(r => r.Type).HasColumnName("type").HasConversion<string>();
        resourceBuilder.Property(r => r.Quantity).HasColumnName("quantity");
        resourceBuilder.Property(r => r.UserId).HasColumnName("user_id").HasColumnType("uuid");

        resourceBuilder
            .HasOne(r => r.User)
            .WithMany(u => u.Resources)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // === Construction ===
        var constructionBuilder = modelBuilder.Entity<UserConstructionDAO>();
        constructionBuilder.ToTable("UserConstructions");
        constructionBuilder.HasKey(c => c.Id);
        constructionBuilder.Property(c => c.Id).HasColumnName("id").HasColumnType("uuid");
        constructionBuilder.Property(c => c.UserId).HasColumnName("user_id").HasColumnType("uuid");
        constructionBuilder.Property(c => c.BuildingId).HasColumnName("building_id").HasColumnType("uuid");
        constructionBuilder.Property(c => c.StartTime).HasColumnName("start_time");
        constructionBuilder.Property(c => c.EndTime).HasColumnName("end_time");
        constructionBuilder.Property(c => c.IsCompleted).HasColumnName("is_completed");

        constructionBuilder
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        constructionBuilder
            .HasOne(c => c.Building)
            .WithMany()
            .HasForeignKey(c => c.BuildingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}