using camagru.Services;
using camagru.Models;
using Microsoft.AspNetCore.Mvc;

namespace camagru.Controllers;

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
        {
            return NotFound();
        }
        
        return user;
    }
    
    [HttpPost]
    public ActionResult<User> CreateUser(User user)
    {
        _userService.CreateUser(user);
        return user;
    }
}