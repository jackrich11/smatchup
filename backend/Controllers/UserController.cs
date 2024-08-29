using System.Data.Common;
using System.Security.Claims;
using System.Text;
using backend.Daos;
using backend.Factories;
using backend.Models;
using backend.Models.Requests;
using backend.Services;
using backend.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace backend.Controllers;

[ApiController]
public class UserController(IFactory factory, ILogger<UserController> logger) : ControllerBase
{
    private readonly ILogger<UserController> _logger = logger;
    private readonly IUserDao _userDao = factory.GetUserDao();

    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(LoginRequest loginRequest) {
        if(loginRequest is null) {
            return BadRequest(new { message = "Invalid client request." });
        }

        var dbUser = await _userDao.GetUser(loginRequest.Username);

        if(dbUser is null) {
            //user doesn't exist, must create new account
            return NotFound(new { message = "That username is not associated with an account." });
        }

        return Ok();
    }

    [HttpGet("users")]
    public async Task<IEnumerable<User>> GetUsers() =>
        await _userDao.GetUsers();

    [HttpGet("user/{id}")]
    public async Task<ActionResult<User>> GetUser(string id) {
        var user = await _userDao.GetUser(id);

        if(user is null) {
            return NotFound();
        }

        return user;
    }

    [HttpPost("user")]
    public async Task<ActionResult<User>> Post(User newUser) {
        await _userDao.CreateUser(newUser);

        return CreatedAtAction(nameof(Post), new { Username = newUser.Username }, newUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id) {
        var user = await _userDao.GetUser(id);

        if(user is null) {
            return NotFound();
        }

        await _userDao.GetUser(id);

        return NoContent();
    }
}

