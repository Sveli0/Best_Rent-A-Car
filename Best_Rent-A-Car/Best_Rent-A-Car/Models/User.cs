using Microsoft.AspNetCore.Identity;
using System.Collections;

namespace Best_Rent_A_Car.Models
{
    public class User : IdentityUser
    {
        public bool IsAdmin 
        {
            get { return this.Id == "1"; }
            private set { }
        }
    }
}
