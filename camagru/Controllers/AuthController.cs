using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using camagru.Models;

namespace camagru.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<MyUser> _signInManager;

    public AuthController(SignInManager<MyUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out successfully." });
    }
}

