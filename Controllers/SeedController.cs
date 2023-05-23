using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameLibraryAPI.Models;
using GameLibraryAPI;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly GameDBContext _context;


        public SeedController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, GameDBContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("Users")]
        public async Task<IActionResult> ImportUsers()
        {
            const string roleUser = "RegisteredUser";
            const string roleAdmin = "Administrator";

            if (await _roleManager.FindByNameAsync(roleUser) is null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleUser));
            }
            if (await _roleManager.FindByNameAsync(roleAdmin) is null)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleAdmin));
            }

            List<IdentityUser> addedUserList = new();
            (string name, string email) = ("admin", "admin@email.com");

            if (await _userManager.FindByNameAsync(name) is null)
            {
                IdentityUser userAdmin = new()
                {
                    UserName = name,
                    Email = email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var createAdminResult = await _userManager.CreateAsync(userAdmin, _configuration["DefaultPasswords:Administrator"]
                    ?? throw new InvalidOperationException());

                if (!createAdminResult.Succeeded)
                {
                    return BadRequest(createAdminResult.Errors);
                }

                var addToRolesAdminResult = await _userManager.AddToRolesAsync(userAdmin, new[] { roleUser, roleAdmin });

                if (!addToRolesAdminResult.Succeeded)
                {
                    return BadRequest(addToRolesAdminResult.Errors);
                }

                // ... (rest of your code)
                userAdmin.EmailConfirmed = true;
                userAdmin.LockoutEnabled = false;
                addedUserList.Add(userAdmin);
            }

            (string name, string email) registered = ("user", "user@email.com");

            if (await _userManager.FindByNameAsync(registered.name) is null)
            {
                IdentityUser user = new()
                {
                    UserName = registered.name,
                    Email = registered.email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var createUserResult = await _userManager.CreateAsync(user, _configuration["DefaultPasswords:RegisteredUser"]
                    ?? throw new InvalidOperationException());

                if (!createUserResult.Succeeded)
                {
                    return BadRequest(createUserResult.Errors);
                }

                var addToRoleUserResult = await _userManager.AddToRoleAsync(user, roleUser);

                if (!addToRoleUserResult.Succeeded)
                {
                    return BadRequest(addToRoleUserResult.Errors);
                }

                // ... (rest of your code)
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                addedUserList.Add(user);
            }

            if (addedUserList.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                addedUserList.Count,
                Users = addedUserList
            });

        }
    }
}