using EtcdManager.API.Models;
using EtcdManager.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EtcdManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(
        IUserService userService
    )
    {
        this._userService = userService;
    }

    [HttpPost("ChangeMyPassword")]
    public async Task<IActionResult> ChangeMyPassword([FromBody] ChangePasswordModel changePasswordModel)
    {
        return Ok(await this._userService.ChangeMyPassword(changePasswordModel));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await this._userService.GetUsers());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        return Ok(await this._userService.GetUserById(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User userModel)
    {
        return Ok(await this._userService.CreateUser(userModel));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] User userModel)
    {
        return Ok(await this._userService.UpdateUser(userModel));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        return Ok(await this._userService.DeleteUser(new User { Id = id }));
    }

    [HttpGet("GetUserInfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        var user = await this._userService.GetUserInfo();
        return Ok(user);
    }
}