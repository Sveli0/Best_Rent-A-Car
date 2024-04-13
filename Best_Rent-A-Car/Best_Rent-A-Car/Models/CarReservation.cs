using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Localization;
using Best_Rent_A_Car.Models.Attributes;

namespace Best_Rent_A_Car.Models
{
    public class CarReservation
    {
        /// <summary>
        /// The model for the core functionality of the program, has a composite PK using the CarID and the 
        /// UserID
        /// </summary>
        /// A model for the requests and reservations, it has a composite primary key using the CarID and VisibleUserID, 
        /// because of the way that the identity user is implemented the name of the userId had to be different in
        /// order to be accessible in the code, the field for Pending was added in order to accommodate accepting pending
        /// requests. Has Validation for many of its fields

        [Display(Name = "Car")]
        public int CarID { get; set; }
        public Car Car { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DateAttribute(ErrorMessage ="The date cannot be previous to now")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DateAttribute(ErrorMessage ="The date cannot be previous to now")]
        [EndDate(ErrorMessage="The date cannot be previous to the start date.")]
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
