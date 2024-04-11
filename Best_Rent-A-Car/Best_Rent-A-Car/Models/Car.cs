using System;

namespace Best_Rent_A_Car.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public DateTime Year { get; set; }
        public int Seats { get; set; }
        public string Info { get; set; }
        public double PricePerDay { get; set; }
        public Car()
        {
            
        }
    }
}
