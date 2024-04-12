using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Best_Rent_A_Car.Models
{
    public class CarReservation
    {
        [Display(Name = "Car")]
        public int CarID { get; set; }
        public Car Car { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [ForeignKey("User")]
        public string VisibleUserID { get ; set; }
        public User User { get; set; }
        public CarReservation()
        {

        }
    }
}
