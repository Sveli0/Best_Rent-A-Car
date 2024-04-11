using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Best_Rent_A_Car.Models;

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
    }
}
