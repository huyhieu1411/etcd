using EtcdManager.API.Models;
using EtcdManager.API.Repos;
using EtcdManager.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace EtcdManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ConnectionController : ControllerBase
{
    private readonly IConnectionService _connectionService;
    private readonly IConnectionRepos _connectionRepos;

    // constructor
    public ConnectionController(
        IConnectionService connectionService,
        IConnectionRepos connectionRepos
    )
    {
        this._connectionService = connectionService;
        this._connectionRepos = connectionRepos;
    }

    [HttpPost("CheckConnection")]
    public async Task<IActionResult> CheckConnection([FromBody] ConnectionModel connectionModel)
    {
        return Ok(await this._connectionService.TestConnection(connectionModel));
    }

    [HttpPost]
    public async Task<IActionResult> CreateConnection([FromBody] ConnectionModel connectionModel)
    {
        return Ok(await this._connectionRepos.CreateConnection(connectionModel));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateConnection([FromRoute] int id, [FromBody] ConnectionModel connectionModel)
    {
        return Ok(await this._connectionRepos.UpdateConnection(id, connectionModel));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConnection([FromRoute] int id)
    {
        return Ok(await this._connectionRepos.DeleteConnection(id));
    }

    [HttpDelete("DeleteConnectionByName")]
    public async Task<IActionResult> DeleteConnectionByName([FromQuery] string name)
    {
        return Ok(await this._connectionRepos.DeleteConnectionByName(name));
    }

    [HttpGet]
    public async Task<IActionResult> GetConnections()
    {
        return Ok(await this._connectionRepos.GetConnections());
    }

    [HttpGet("GetByName")]
    public async Task<IActionResult> GetConnectionByName([FromQuery] string name)
    {
        return Ok(await this._connectionRepos.GetConnectionByName(name));
    }
}