using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TodoApp.Core.Identity;
using TodoApp.Infrastructure.Persistence;

namespace TodoApp.Infrastructure.Persistence
{
    public  class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp, ILogger<DataSeeder> logger)
        {
            var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var userMgr = sp.GetRequiredService<UserManager<ApplicationUser>>();
            var ctx = sp.GetRequiredService<AppDbContext>();

            try
            {
                if (!await roleMgr.RoleExistsAsync("Owner"))
                {
                    var result = await roleMgr.CreateAsync(new IdentityRole<Guid>("Owner"));
                    if (!result.Succeeded)
                    {
                        logger.LogError($"Failed to create role 'Owner': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                    else
                    {
                        logger.LogInformation("Role 'Owner' created successfully.");
                    }
                }

                if (!await roleMgr.RoleExistsAsync("Guest"))
                {
                    var result = await roleMgr.CreateAsync(new IdentityRole<Guid>("Guest"));
                    if (!result.Succeeded)
                    {
                        logger.LogError($"Failed to create role 'Guest': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                    else
                    {
                        logger.LogInformation("Role 'Guest' created successfully.");
                    }
                }

                var ownerEmail = "owner@example.com";
                var ownerUser = await userMgr.FindByEmailAsync(ownerEmail);

                if (ownerUser == null)
                {
                    var owner = new ApplicationUser
                    {
                        UserName = "owner",
                        Email = ownerEmail,
                        EmailConfirmed = true 
                    };

                    var result = await userMgr.CreateAsync(owner, "P@ssw0rd");

                    if (result.Succeeded)
                    {
                        await userMgr.AddToRoleAsync(owner, "Owner");
                        logger.LogInformation("User 'owner' created and added to 'Owner' role successfully.");
                    }
                    else
                    {
                        logger.LogError($"Error creating user 'owner': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogInformation("User 'owner' already exists.");
                }

                if (!ctx.Categories.Any())
                {
                    ctx.Categories.Add(new TodoApp.Domain.Entities.Category { Name = "General" });
                    await ctx.SaveChangesAsync();
                    logger.LogInformation("Category 'General' added to the database.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"An error occurred while seeding the data: {ex.Message}");
            }
        }

    }
}
