using Core.Data;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly CloudNativeDbContext _dbContext;

    public UserController(CloudNativeDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    [HttpGet(Name = "GetUser")]
    public IEnumerable<User> Get()
    {
        return _dbContext.Users.ToList();
    }
}
