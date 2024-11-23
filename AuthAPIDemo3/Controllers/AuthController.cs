using AuthAPIDemo3.Models;
using AuthAPIDemo3.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthAPIDemo3.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signinManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AuthService _authService;

    public AuthController (UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signinManager,
        RoleManager<IdentityRole> roleManager,
        AuthService authService)
    {
        _userManager = userManager;
        _signinManager = signinManager;
        _roleManager = roleManager;
        _authService = authService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register (RegisterUserModel user)
    {
        var identity = new IdentityUser
        {
            UserName = user.Username,
            Email = user.Email
        };

        var entity = await _userManager.CreateAsync(identity, user.Password);

        if (!entity.Succeeded)
        {
            return BadRequest(entity.Errors);
        }

        var newClaims = new List<Claim>
        {
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
        };

        await _userManager.AddClaimsAsync(identity, newClaims);
        await AddRoleToUser(identity, user.Role);

        var token = await GenerateJwtToken(identity);

        return Ok(new { Token = token });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login (LoginUserModel credentials)
    {
        var user = await _userManager.FindByEmailAsync(credentials.Email);

        if (user is null)
        {
            return BadRequest("User not found");
        }

        var result = await _signinManager.CheckPasswordSignInAsync(user, credentials.Password, false);

        if (!result.Succeeded)
        {
            return BadRequest("Faild login attempt");
        }

        var token = await GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

    private async Task AddRoleToUser (IdentityUser user, Role role)
    {
        var roleName = role.ToString();
        var identityRole = await _roleManager.FindByNameAsync(roleName);

        if (identityRole is null)
        {
            identityRole = new IdentityRole(roleName);
            await _roleManager.CreateAsync(identityRole);
        }

        await _userManager.AddToRoleAsync(user, roleName);
    }

    private async Task<string> GenerateJwtToken (IdentityUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new (JwtRegisteredClaimNames.Sub, user.Id ?? throw new InvalidOperationException()),
            new (JwtRegisteredClaimNames.Email, user.Email ?? throw new InvalidOperationException())
        });

        foreach (var role in roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        claimsIdentity.AddClaims(claims);

        var token = _authService.CreateToken(claimsIdentity);
        return _authService.WriteToken(token);
    }
}
