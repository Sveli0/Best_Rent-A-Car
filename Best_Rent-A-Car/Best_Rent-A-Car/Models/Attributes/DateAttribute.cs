using System;
using System.ComponentModel.DataAnnotations;

namespace Best_Rent_A_Car.Models.Attributes
{
    public class DateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            DateTime date = (DateTime)value;
            return date >= DateTime.Now;
        }
    }
}
