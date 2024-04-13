using Best_Rent_A_Car.Models.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Best_Rent_A_Car.Models
{
    public class User : IdentityUser
    {
        [UniqueEGN]
        public string EGN { get; set; }
    }
}
