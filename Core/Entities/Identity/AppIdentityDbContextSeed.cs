using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities.Identity
{
 public   class AppIdentityDbContextSeed
    {

        public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    DisplayName = "Bob",
                    Email = "Bob@test.com",
                    UserName = "Bob@test.com",
                    Address = new Address()
                    {
                        FirstName = "Bob",
                        LastName = "Fang",
                        Street = "10 The Street",
                        City = "New York",
                        State = "NY",
                        Zipcode = "90210"
                    }
                };

                await userManager.CreateAsync(user, "Pa$$0rd");
            }
        }
    }
}
