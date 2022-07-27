using EtcdManager.API.Models;
using EtcdManager.API.Repos;
using EtcdManager.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EtcdManager.API.Controllers;

[ApiController]
[Route("[controller]")]

public class AuthenController : ControllerBase
{
    private readonly IConnectionService _connectionService;
    private readonly IAuthenRepos _authenRepos;
    private readonly IUserRepos _userRepos;

    // constructor
    public AuthenController(
        IAuthenRepos authenRepos,
        IUserRepos userRepos
    )
    {
        this._authenRepos = authenRepos;
        this._userRepos = userRepos;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
    {
        if (loginModel == null)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var loginResult = await this._authenRepos.Login(loginModel.UserName, loginModel.Password);
        return Ok(loginResult);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return Ok(await _authenRepos.Logout());
    }
}