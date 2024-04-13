using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Best_Rent_A_Car.Models;
using System.Reflection.Emit;

namespace Best_Rent_A_Car.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Best_Rent_A_Car.Models.Car> Cars { get; set; }
        public DbSet<Best_Rent_A_Car.Models.CarReservation> CarReservations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<CarReservation>()
                .HasKey(x => new { x.CarID, x.VisibleUserID });
            builder
                .Entity<User>()
                .HasIndex(x => x.EGN)
                .IsUnique();
            builder
                .Entity<User>()
                .HasIndex(x => x.UserName)
                .IsUnique();
            builder
                .Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();
            base.OnModelCreating(builder);
        }
    }
}
