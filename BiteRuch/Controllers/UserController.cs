using BiteRush.DatabaseContext;
using BiteRush.DTO;
using BiteRush.Models;
using BiteRush.Service;
using BiteRush.ServicesContract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiteRush.Controllers;


[ApiController]
public class UserController(
    DBContext context,
    IJWTService JwtService) : ControllerBase
{
    [HttpPost]
    [Route("api/register")]
    public async Task<IActionResult> Register(UserDTO userDto)
    {
        if (ModelState.IsValid)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == userDto.EmailAddress);
            if (user is not null)
                return BadRequest("this email already exist");
            var passwordHashed = PasswordHasher.HashPassword(userDto.Password);
            User newuser = new User(Guid.NewGuid(), userDto.UserName, userDto.EmailAddress, passwordHashed, userDto.Gender, userDto.Level);
            context.Users.Add(newuser);
            await context.SaveChangesAsync();
            return Ok(newuser.Id);
        }
        else
        {
            String errors = string.Join("; ", ModelState.Values
                                   .SelectMany(x => x.Errors)
                                   .Select(x => x.ErrorMessage));

            return BadRequest(errors);
        }
    }

    [HttpPost]
    [Route("api/login")]

    public async Task<IActionResult> Login(LoginDTO login)
    {
        if (login.Email is null || login.Password is null)
            return BadRequest("email and password cannot be blank");
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
        if (user is null)
            return NotFound("User Not Found");
        var IsValid = PasswordHasher.VerifyPassword(login.Password, user.Password);
        if (!IsValid)
            return BadRequest("invalid password");
        var token = JwtService.GenrateJWT(user);
        return Ok(token);
    }
}
