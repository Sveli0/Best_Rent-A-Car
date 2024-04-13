using Best_Rent_A_Car.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Best_Rent_A_Car.Models.Attributes
{
    public class UniquePhoneNumber : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ApplicationDbContext dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            User user = dbContext.Users.SingleOrDefault(x => x.PhoneNumber == (string)value);

            if (user != null)
            {
                return new ValidationResult("There is already a user with this Phone number.");
            }

            return ValidationResult.Success;
        }
    }
}
