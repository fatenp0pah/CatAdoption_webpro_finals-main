using Microsoft.EntityFrameworkCore;
using CatAdoption.Models;

namespace CatAdoption.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSets for the models
        public DbSet<User> Users { get; set; }
        public DbSet<Cat> Cats { get; set; }
        public DbSet<Adoption> Adoptions { get; set; }
        public DbSet<Adopter> Adopter { get; set; }  // Add this DbSet if Adopter model exists

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure Username is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Ensure Email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Enforce length constraints and ensure required fields for User
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .HasMaxLength(255)  // Password field for plain text password (consider hashing for security)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasMaxLength(50);

            // Set default value for Role if not provided
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue("Customer");

            // Automatically set CreatedAt on creation for User
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configure Adoption relationships
            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.Cat)
                .WithMany(c => c.Adoptions)
                .HasForeignKey(a => a.CatId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of Cat if there's an adoption

            modelBuilder.Entity<Adoption>()
                .HasOne(a => a.User)
                .WithMany(u => u.Adoptions)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Delete adoptions if the user is deleted

            // Automatically set CreatedAt on creation for Adoption
            modelBuilder.Entity<Adoption>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configure Cat properties
            modelBuilder.Entity<Cat>()
                .Property(c => c.AvailableForAdoption)
                .HasDefaultValue(true); // Default to available for adoption

            // Convert bool to int (0/1) for AvailableForAdoption
            modelBuilder.Entity<Cat>()
                .Property(c => c.AvailableForAdoption)
                .HasConversion<int>();

            // Set max length for Cat's name to 255 characters and make it required
            modelBuilder.Entity<Cat>()
                .Property(c => c.Name)
                .HasMaxLength(255)
                .IsRequired();

            // Configure the Cat's Breed with a max length of 100 and make it required
            modelBuilder.Entity<Cat>()
                .Property(c => c.Breed)
                .HasMaxLength(100)
                .IsRequired();

            // Add more optional constraints for Cat or User if needed
            // For example, if there's a "description" field for Cats, you could do something like:
            // modelBuilder.Entity<Cat>().Property(c => c.Description).HasMaxLength(500);
        }
    }
}
