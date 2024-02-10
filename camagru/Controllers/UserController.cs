using camagru.Services;
using camagru.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace camagru.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController: Controller
{
    private readonly UserService _userService;
    
    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public ActionResult<List<User>> GetUsers() =>
        _userService.GetUsers();
    
    [HttpGet("{id}")]
    public ActionResult<User> GetUser(string id)
    {
        var user = _userService.GetUser(id);
        
        if (user is null)
            return NotFound();
        
        return Json(user);
    }
    
    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        _userService.CreateUser(user);
        return Json(user);
    }
    
    [AllowAnonymous]
    [Route("authenticate")]
    [HttpPost]
    public ActionResult<string> Login([FromBody] User user)
    {
        var token = _userService.Authenticate(user.Id, user.Password);
        
        if (token is null)
            return Unauthorized();

        return Ok(new { token, user });
    }
}