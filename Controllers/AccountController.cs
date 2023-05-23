using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using GameLibraryAPI.Models; // Update this to the appropriate namespace for your GameModels project
using GameLibraryAPI; // Update this to the appropriate namespace for your BlogApi project
using GameLibraryAPI.Dtos; // Update this to the appropriate namespace for your BlogApi project

namespace GameLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtHandler _jwtHandler;

        public AccountController(UserManager<IdentityUser> userManager, JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            IdentityUser user = await _userManager.FindByNameAsync(loginRequest.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password)) {
                return Unauthorized(new LoginResult {
                    Success = false,
                    Message = "Invalid Username or Password."
                });
            }

            JwtSecurityToken secToken = await _jwtHandler.GetTokenAsync(user);
            string jwt = new JwtSecurityTokenHandler().WriteToken(secToken);
            return Ok(new LoginResult {
                Success = true,
                Message = "Login successful",
                Token = jwt
            });
        }
    }
}