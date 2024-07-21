using Microsoft.AspNetCore.Identity;
using FlightBookingApp.Web.Data;
using FlightBookingApp.Web.Models;

public static class SeedData
{
    public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string[] roleNames = { "Admin", "User" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Create the roles and seed them to the database
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create a default admin user
        var adminUser = new IdentityUser
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            EmailConfirmed = true
        };

        var userPassword = "Admin123!";
        var admin = await userManager.FindByEmailAsync(adminUser.Email);

        if (admin == null)
        {
            var createAdminUser = await userManager.CreateAsync(adminUser, userPassword);
            if (createAdminUser.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create a default normal user
        var normalUser = new IdentityUser
        {
            UserName = "user@user.com",
            Email = "user@user.com",
            EmailConfirmed = true
        };

        var normalUserPassword = "User123!";
        var user = await userManager.FindByEmailAsync(normalUser.Email);

        if (user == null)
        {
            var createNormalUser = await userManager.CreateAsync(normalUser, normalUserPassword);
            if (createNormalUser.Succeeded)
            {
                await userManager.AddToRoleAsync(normalUser, "User");
            }
        }

        // Create user andres.aizaga@gmail.com with the role User
        var specificUser = new IdentityUser
        {
            UserName = "andres.aizaga@gmail.com",
            Email = "andres.aizaga@gmail.com",
            EmailConfirmed = true
        };

        var specificUserPassword = "Andres123!";
        var specificUserCheck = await userManager.FindByEmailAsync(specificUser.Email);

        if (specificUserCheck == null)
        {
            var createSpecificUser = await userManager.CreateAsync(specificUser, specificUserPassword);
            if (createSpecificUser.Succeeded)
            {
                await userManager.AddToRoleAsync(specificUser, "User");
            }
        }

        // Seed 10 different flights
        if (!context.Flights.Any())
        {
            context.Flights.AddRange(
                new Flight { Airline = "American Airlines", Destination = "New York", DepartureTime = DateTime.Now.AddDays(10) },
                new Flight { Airline = "Avianca", Destination = "Colombia", DepartureTime = DateTime.Now.AddDays(11) },
                new Flight { Airline = "Delta", Destination = "Panama", DepartureTime = DateTime.Now.AddDays(12) },
                new Flight { Airline = "EasyJet", Destination = "New Jersey", DepartureTime = DateTime.Now.AddDays(13) },
                new Flight { Airline = "JetBlue", Destination = "Miami", DepartureTime = DateTime.Now.AddDays(14) },
                new Flight { Airline = "Qatar Airways", Destination = "Qatar", DepartureTime = DateTime.Now.AddDays(15) },
                new Flight { Airline = "KLM", Destination = "Alemania", DepartureTime = DateTime.Now.AddDays(16) },
                new Flight { Airline = "Aeroméxico", Destination = "Monterrey", DepartureTime = DateTime.Now.AddDays(17) },
                new Flight { Airline = "Copa Airlines", Destination = "Washington", DepartureTime = DateTime.Now.AddDays(18) },
                new Flight { Airline = "Sky Airline", Destination = "California", DepartureTime = DateTime.Now.AddDays(19) }
            );
            await context.SaveChangesAsync();
        }
    }
}
