using Library.Models.Security;
using Library.SharedKernel.Enums;
using Library.SQL.Context;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.SQL.Seed
{
    public class SecurityDataSeed
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = (RoleManager<IdentityRole<Guid>>)services.GetService(typeof(RoleManager<IdentityRole<Guid>>));
            var userManager = (UserManager<User>)services.GetService(typeof(UserManager<User>));
            var context = (LibraryDBContext)services.GetService(typeof(LibraryDBContext));

            var newRoles = await CreateNewRoles(roleManager);
            await CreateSuperAdmin(context, userManager, roleManager);

            return;
        }

        private static async Task<IEnumerable<string>> CreateNewRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var IdentityRoles = Enum.GetValues(typeof(LibraryRole)).Cast<LibraryRole>().Select(a => a.ToString());
            var ExistedRoles = roleManager.Roles.Select(a => a.Name).ToList();
            var newRoles = IdentityRoles.Except(ExistedRoles);

            foreach (var @new in newRoles)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>() { Name = @new });
            }

            return newRoles;
        }

        private static async Task CreateSuperAdmin(LibraryDBContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            var superAdmin = await userManager.FindByNameAsync("caro");

            if (superAdmin is null)
            {
                superAdmin = new User
                {
                    FullName = "Super Admin",
                    UserName = "caro",
                    UserType = UserType.SuperAdmin,
                    PhoneNumber = "0930057225",
                    Email = "carolinedahi1212@gmail.com",
                    DateCreated = DateTime.UtcNow,
                    GenerationStamp = ""
                };

                var createResult = await userManager.CreateAsync(superAdmin, "1234");

                if (createResult.Succeeded && createResult.Succeeded)
                {
                    var identityRoles = roleManager.Roles.Select(a => a.Name).ToList();
                    var roleResult = await userManager.AddToRolesAsync(superAdmin, identityRoles);
                    if (roleResult.Succeeded)
                        return;
                    throw new Exception(string.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
                throw new Exception(string.Join("\n", createResult.Errors.Select(error => error.Description)));

            }
        }
    }
}
