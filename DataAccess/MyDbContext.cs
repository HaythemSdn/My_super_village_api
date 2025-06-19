using Common.Dao;
using Microsoft.EntityFrameworkCore;

public class MyDbContext :DbContext
{
    public DbSet<UserDAO> Users { get; set; }
    public DbSet<UserBuildingDAO> UserBuildings { get; set; }
    public DbSet<UserResourceDAO> UserResources { get; set; }

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
    }
    
}