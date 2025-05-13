using Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastracture
{
    public class Seed
    {
        public static async Task SeedUser(UserManager<User> userManager)
        {
            var user = await userManager.FindByEmailAsync("admin@example.com");

            if (user == null)
            {
                user = new User
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    Name = "Admin User",  
                };

                var result = await userManager.CreateAsync(user, "Password123"); 

                if (result.Succeeded)
                {
                }
            }
        }
    }
}
