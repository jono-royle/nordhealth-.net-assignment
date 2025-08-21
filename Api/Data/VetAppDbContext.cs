using Api.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Api.Data
{
    public class VetAppDbContext : DbContext
    {
        public VetAppDbContext(DbContextOptions<VetAppDbContext> options) : base(options) { }

        public DbSet<Veterinarian> Veterinarians => Set<Veterinarian>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Animal> Animals => Set<Animal>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Veterinarian>().ToTable("Veterinarian");
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Animal>().ToTable("Animal");
            modelBuilder.Entity<Appointment>().ToTable("Appointment");

            modelBuilder.Entity<Veterinarian>().HasKey(x => x.Id);
            modelBuilder.Entity<Customer>().HasKey(x => x.Id);
            modelBuilder.Entity<Animal>().HasKey(x => x.Id);
            modelBuilder.Entity<Appointment>().HasKey(x => x.Id);

            modelBuilder.Entity<Animal>()
                .HasOne(a => a.Owner)
                .WithMany(c => c.Animals)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Animal)
                .WithMany(an => an.Appointments)
                .HasForeignKey(a => a.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Veterinarian)
                .WithMany(v => v.Appointments)
                .HasForeignKey(a => a.VeterinarianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.StartTime, a.VeterinarianId });

            modelBuilder.Entity<Animal>()
                .HasIndex(a => new { a.CustomerId, a.Name });

            modelBuilder.Entity<Customer>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Customer>().Property(x => x.Email).IsRequired();
            modelBuilder.Entity<Animal>().Property(x => x.Name).IsRequired();
            modelBuilder.Entity<Veterinarian>().Property(x => x.Name).IsRequired();
        }
    }
}
