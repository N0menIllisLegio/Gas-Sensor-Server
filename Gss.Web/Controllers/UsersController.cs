using System.Threading.Tasks;
using Gss.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gss.Web.Controllers
{
  [Route("api/Users/[action]")]
  [ApiController]
  public class UsersController : Controller
  {
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager<User> userManager)
    {
      _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
      return Ok(await _userManager.Users.ToListAsync());
    }
  }
}