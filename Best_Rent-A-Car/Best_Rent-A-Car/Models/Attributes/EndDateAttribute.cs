namespace Best_Rent_A_Car.Models.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EndDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var endDate = (DateTime)value;
            var startDateProperty = validationContext.ObjectType.GetProperty("StartDate");
            if (startDateProperty == null)
            {
                return new ValidationResult("Invalid property name");
            }

            var startDate = (DateTime)startDateProperty.GetValue(validationContext.ObjectInstance);

            if (endDate < startDate)
            {
                return new ValidationResult("End date cannot be earlier than start date");
            }

            return ValidationResult.Success;
        }
    }

}
