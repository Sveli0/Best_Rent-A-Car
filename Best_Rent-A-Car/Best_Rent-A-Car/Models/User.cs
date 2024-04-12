using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace Best_Rent_A_Car.Models
{
    public class User : IdentityUser
    {
        public string EGN { get; set; }
    }
}
