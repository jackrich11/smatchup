using System.Net;
using backend.Daos.Matchups;
using backend.Daos.Sessions;
using backend.Factories;
using backend.Hubs;
using backend.Models;
using backend.Models.Requests;
using backend.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace backend.Controllers;

[ApiController]
[Route("api")]
public class MatchupController : ControllerBase {

    private readonly ILogger _logger;
    private readonly IMatchupDao _matchupDao;
    private readonly ISessionDao _sessionDao;
    private readonly IHubContext<MatchupHub> _matchupHubContext;

    public MatchupController(ILogger<MatchupController> logger, IFactory factory, IHubContext<MatchupHub> matchupHubContext) {
        _logger = logger;
        _matchupDao = factory.GetMatchupDao();
        _sessionDao = factory.GetSessionDao();
        _matchupHubContext = matchupHubContext;
    }

    [HttpGet("matchups/{matchupId}")]
    public async Task<ActionResult<Matchup>> GetMatchup(string matchupId) {
        Matchup matchup;

        _logger.LogInformation($"In matchup controller for {matchupId}");

        var sessionId = Request.Cookies[Strings.SESSION_ID];
        if(sessionId is not null && _sessionDao.GetSession(sessionId) is not null) {
            try {
                matchup = await _matchupDao.GetMatchup(matchupId);
            } catch(Exception e) {
                _logger.LogError($"GET-/api/matchups/single: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        } else {
            return Unauthorized(new { message = "Please log in with your Discord."});
        }

        return Ok(matchup);
    }

    [HttpGet("matchups")]
    public async Task<ActionResult<List<Matchup>>> GetAllMatchups() {
        List<Matchup> matchups;

        var sessionId = Request.Cookies[Strings.SESSION_ID];
        if(sessionId is not null && _sessionDao.GetSession(sessionId) is not null) {
            try {
                matchups = await _matchupDao.GetAllMatchups();
            } catch(Exception e) {
                _logger.LogError($"GET-/api/matchups: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        } else {
            return Unauthorized(new { message = "Please log in with your Discord."});
        }
        
        return Ok(matchups);
    }

    [HttpPost("matchups")]
    public async Task<ActionResult> AddMatchup(CreateMatchupRequest req) {                
        string? sessionId = Request.Cookies[Strings.SESSION_ID];
        Session? session;
        if(sessionId is not null && (session = await _sessionDao.GetSession(sessionId)) is not null) {
            if((await _matchupDao.GetUserMatchup(session.UserID)) != null) {
                return BadRequest(new { message = "You already have a matchup created."});
            }

            try {
                var matchup = new Matchup() {
                    MatchupId =  Guid.NewGuid().ToString(),
                    Creator = session.UserID,
                    LookingFor = req.LookingFor,
                    CanPlay = req.CanPlay,
                    Created = DateTime.Now
                };
                await _matchupDao.SetMatchup(matchup);

                try {
                    await _matchupHubContext.Clients.Group(Strings.MATCHUPS_GROUP).SendAsync(Strings.MATCHUPS_UPDATED);
                } catch (Exception e) {
                    _logger.LogError("ERROR alerting of matchup being added: " + e.Message);
                }

                return Created(nameof(matchup), matchup);

            } catch(Exception e) {
                _logger.LogError($"POST-/api/matchups: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        } else {
            return Unauthorized(new { message = "Please log in with your Discord."});
        }

    }

    [HttpPut("matchups/{matchupId}/{visitor}")]
    public async Task<ActionResult> AddVisitorToMatchup(string matchupId, string visitor) {
        _logger.LogInformation("IN ADDVISITOR ENDPOINT for: " + visitor);

        var sessionId = Request.Cookies[Strings.SESSION_ID];
        if(sessionId is not null && _sessionDao.GetSession(sessionId) is not null) {
            try {
                await _matchupDao.AddVisitorToMatchup(visitor, matchupId);

                try {
                    await _matchupHubContext.Clients.Group(Strings.MATCHUPS_GROUP).SendAsync(Strings.MATCHUPS_UPDATED);
                } catch (Exception e) {
                    _logger.LogError("ERROR alerting of matchup being updated: " + e.Message);
                }

            } catch(Exception e) {
                _logger.LogError($"PUT-/api/matchups: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        } else {
            return Unauthorized(new { message = "Please log in with your Discord."});
        }
        
        return Ok();
    }

    [HttpPut("matchups/{matchupId}")]
    public async Task<ActionResult> RemoveVisitorFromMatchup(string matchupId) {
        _logger.LogInformation("IN REMOVEVISITOR ENDPOINT for: " + matchupId);

        var sessionId = Request.Cookies[Strings.SESSION_ID];
        if(sessionId is not null && _sessionDao.GetSession(sessionId) is not null) {
            try {
                await _matchupDao.RemoveVisitorFromMatchup(matchupId);

                try {
                    await _matchupHubContext.Clients.All.SendAsync(Strings.MATCHUPS_UPDATED);
                } catch (Exception e) {
                    _logger.LogError("ERROR alerting of matchup being updated: " + e.Message);
                }

            } catch(Exception e) {
                _logger.LogError($"PUT-/api/matchups: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        } else {
            return Unauthorized(new { message = "Please log in with your Discord."});
        }
        
        return Ok();
    }

    [HttpDelete("matchups/{matchupId}")]
    public async Task<ActionResult> DeleteMatchup(string matchupId) {
        _logger.LogInformation("IN DELETE MATCHUP ENDPOINT");
        var sessionId = Request.Cookies[Strings.SESSION_ID];
        if(sessionId is not null && _sessionDao.GetSession(sessionId) is not null) {
            _logger.LogInformation($"Deleting matchup : {matchupId}");
            try {
                await _matchupDao.DeleteMatchup(matchupId);

                try {
                    await _matchupHubContext.Clients.Group(Strings.MATCHUPS_GROUP).SendAsync(Strings.MATCHUPS_UPDATED);
                    _logger.LogInformation("MATCHUP WAS DELETED UPDATE");
                } catch (Exception e) {
                    _logger.LogError("ERROR alerting of matchup being deleted: " + e.Message);
                }

            } catch(Exception e) {
                _logger.LogError($"DELETE-api/matchups/{matchupId}: {e.Message}" );
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        } else {
            return Unauthorized(new { message = "Please log in with your Discord."});
        }
        
        return Ok();
    }
}