using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Trifoia.Module.InkPlayer.Repository;
using Oqtane.Controllers;
using System.Net;
using System.Threading.Tasks;
using System;

namespace Trifoia.Module.InkPlayer.Controllers;

[Route(ControllerRoutes.ApiRoute)]
public class InkPlayerController : ModuleControllerBase
{
    private readonly InkPlayerRepository _InkPlayerRepository;

    public InkPlayerController(InkPlayerRepository InkPlayerRepository, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
    {
        _InkPlayerRepository = InkPlayerRepository;
    }

    // GET: api/<controller>?moduleid=x
    [HttpGet]
    [Authorize(Roles = RoleNames.Registered)]
    public async Task<ActionResult<IEnumerable<Models.InkPlayer>>> Get(){
        try{
            var data = _InkPlayerRepository.GetInkPlayers();
            return Ok(data);
        }
        catch(Exception ex){
            var errorMessage = $"Repository Error Get Attempt InkPlayer";
            _logger.Log(LogLevel.Error, this, LogFunction.Read, errorMessage);
            return StatusCode(500);
        }
    }

    // GET api/<controller>/5
    [HttpGet("{id}")]
    [Authorize(Roles = RoleNames.Registered)]
    public async Task<ActionResult<Models.InkPlayer>> Get(int id)
    {
        try {
            var data = _InkPlayerRepository.GetInkPlayer(id);
            return Ok(data);
        }
        catch (Exception ex)       { 
            _logger.Log(LogLevel.Error, this, LogFunction.Read, "Failed InkPlayer Get Attempt {id}", id);
            return StatusCode(500);
        }
    }

    // POST api/<controller>
    [HttpPost]
    [Authorize(Roles = RoleNames.Registered)]
    public async Task<ActionResult<Models.InkPlayer>> Post([FromBody] Models.InkPlayer item)
    {
        if (ModelState.IsValid)
        {
            try{
                item = _InkPlayerRepository.AddInkPlayer(item);
                _logger.Log(LogLevel.Information, this, LogFunction.Create, "InkPlayer Added {InkPlayer}", item);
            }
            catch (Exception ex) {
                _logger.Log(LogLevel.Error, this, LogFunction.Read, "Failed InkPlayer Add Attempt {item} Message {Message} ", item, ex.Message);
                return StatusCode(500);
            }
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Create, "Invaid InkPlayer Post Attempt {item}", item);
            return BadRequest();
        }
        return Ok(item);
    }

    // PUT api/<controller>/5
    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Registered)]
    public async Task<ActionResult<Models.InkPlayer>> Put(int id, [FromBody] Models.InkPlayer item)
    {
        if (ModelState.IsValid && _InkPlayerRepository.GetInkPlayer(item.InkPlayerId, false) != null)
        {
            item = _InkPlayerRepository.UpdateInkPlayer(item);
            _logger.Log(LogLevel.Information, this, LogFunction.Update, "InkPlayer Updated {item}", item);
            return Ok(item);
        }
        else
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Update, "Unauthorized InkPlayer Put Attempt {item}", item);
            return BadRequest();
        }
    }

    // DELETE api/<controller>/5
    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Registered)]
    public async Task<ActionResult> Delete(int id)
    {
        var data = _InkPlayerRepository.GetInkPlayer(id);
        if (data is null)
        {
            _logger.Log(LogLevel.Error, this, LogFunction.Delete, "Failed InkPlayer Delete Attempt {InkPlayerId}", id);
            return NotFound();
        }

        _InkPlayerRepository.DeleteInkPlayer(id);
        _logger.Log(LogLevel.Information, this, LogFunction.Delete, "InkPlayer Deleted {InkPlayerId}", id);
        return Ok();
    
    }
}