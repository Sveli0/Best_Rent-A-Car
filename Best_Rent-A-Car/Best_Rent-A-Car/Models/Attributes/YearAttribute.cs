using System;
using System.ComponentModel.DataAnnotations;

namespace Best_Rent_A_Car.Models.Attributes
{
    public class YearAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            int date = (int)value;
            return date <= DateTime.Now.Year;
        }
    }
}
