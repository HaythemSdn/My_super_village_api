using Common.Dao;
using Microsoft.EntityFrameworkCore;

public class MyDbContext :DbContext
{
    public DbSet<UserDAO> Users { get; set; }

    public MyDbContext(DbContextOptions<MyDbContext> options) :
        base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var  userDaoBuilder = modelBuilder.Entity<UserDAO>();
        userDaoBuilder.Property(x => x.Id)
            .HasColumnName("id")
            .HasColumnType("uuid");

        // Primary Key
        userDaoBuilder.HasKey(x => x.Id);

        userDaoBuilder.Property(x => x.Pseudo)
            .HasColumnName("pseudo")
            .HasMaxLength(1000)
            .IsUnicode(true);
    }
    
}