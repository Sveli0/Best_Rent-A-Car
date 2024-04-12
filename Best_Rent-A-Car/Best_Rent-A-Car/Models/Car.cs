using System;
using System.ComponentModel.DataAnnotations;

namespace Best_Rent_A_Car.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Seats { get; set; }
        public string Info { get; set; }
        [Display(Name = "Price Per Day")]
        public double PricePerDay { get; set; }
        public Car()
        {
            
        }
        //TEST
    }
}
