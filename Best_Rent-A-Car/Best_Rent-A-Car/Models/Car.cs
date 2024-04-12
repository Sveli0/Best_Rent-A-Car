using System;
using System.ComponentModel.DataAnnotations;
using Best_Rent_A_Car.Models.Attributes;

namespace Best_Rent_A_Car.Models
{
    public class Car
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Brand { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Model { get; set; }
        [Display(Name ="Year:")]
        [DataType(DataType.Date)]
        [YearAttribute(ErrorMessage = "Time cannot be set past the present.")]
        public DateTime Year { get; set; }
        [Required]
        [Display(Name = "Number of seats:")]
        [Range(0, int.MaxValue, ErrorMessage ="Seats cannot be a negtaive number.")]
        public int Seats { get; set; }
        [Display(Name = "Info:")]
        public string Info { get; set; }
        [Required]
        [Display(Name ="Price per day:")]
        [Range(0, double.MaxValue, ErrorMessage ="Price per day cannot be negative.")]
        public double PricePerDay { get; set; }
        public Car()
        {
            
        }
    }
}
