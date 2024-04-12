using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Best_Rent_A_Car.Models
{
    public class CarReservation
    {
        public int ID { get; set; }
        [Display(Name = "Car")]
        public int CarID { get; set; }
        public Car Car { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public User User { get; set; }
        public CarReservation()
        {

        }
    }
}
