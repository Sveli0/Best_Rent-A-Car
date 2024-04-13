using Best_Rent_A_Car.Models.Attributes;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Best_Rent_A_Car.Models
{
    public class User : IdentityUser
    {
        /// <summary>
        /// A user class that is used accross the app an inheritor of the AspNetCoreIdentityUser
        /// </summary>
        /// A custom iteration of the IdentityUser, so that includes an EGN(SSN) field and to be able to expand
        /// functionality in other directions if necessary
        [MinLength(10, ErrorMessage = "EGN must be 10 symbols long.")]
        [MaxLength(10, ErrorMessage = "EGN must be 10 symbols long.")]
        [Display(Name = "EGN")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "EGN must contain only numbers.")]
        public string EGN { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
    }
}
