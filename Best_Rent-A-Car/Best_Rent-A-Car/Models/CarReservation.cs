using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Best_Rent_A_Car.Models
{
    public class CarReservation
    {
        public int ID { get; set; }
        public int CarID { get; set; }
        public Car Car { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public CarReservation()
        {

        }
    }
}
