using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myshop.DAL.Models;
using myshop.DataAccess;
using myshop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myshop.DAL.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            await _SeedRoles(roleManager);
        }

        private static async Task _SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                Roles.Customer,
                Roles.Admin
            };

            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role))
                    continue;

                var result = await roleManager.CreateAsync(new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Failed to create role '{role}': " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
