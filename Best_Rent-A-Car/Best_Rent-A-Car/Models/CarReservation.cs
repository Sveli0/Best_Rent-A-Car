using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Localization;

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
        public bool Pending { get; set; } =true;
        public CarReservation()
        {

        }
    }
}
