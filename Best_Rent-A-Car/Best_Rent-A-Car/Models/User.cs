using Best_Rent_A_Car.Models.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Best_Rent_A_Car.Models
{
    public class User : IdentityUser
    {
        /// <summary>
        /// A user class that is used accross the app an inheritor of the AspNetCoreIdentityUser
        /// </summary>
        /// A custom iteration of the IdentityUser, so that includes an EGN(SSN) field and to be able to expand
        /// functionality in other directions if necessary
        [UniqueEGN]
        public string EGN { get; set; }
    }
}
