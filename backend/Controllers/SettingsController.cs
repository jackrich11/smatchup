namespace backend.Controllers;

using Microsoft.AspNetCore.Mvc;
using backend.Daos.Sessions;
using backend.Daos.Settings;
using backend.Factories;
using backend.Models;
using System.Net;
using backend.Models.Requests;
using Microsoft.Extensions.Options;
using backend.Settings;
using backend.Types;

[ApiController]
[Route("api")]
public class SettingsController : ControllerBase {
    private readonly ILogger<SettingsController> _logger;
    private readonly ISettingsDao _settingsDao;
    private readonly ISessionDao _sessionDao;
    public SettingsController(ILogger<SettingsController> logger, IFactory factory) {
        _logger = logger;
        _settingsDao = factory.GetSettingsDao();
        _sessionDao = factory.GetSessionDao();
    }

    [HttpGet("settings/{username}")]
    public async Task<ActionResult<Settings>> GetSettings(string username) {
        Settings settings;

        _logger.LogInformation($"In settings controller for {username}");

        var sessionId = Request.Cookies[Strings.SESSION_ID];
        if(sessionId is not null && _sessionDao.GetSession(sessionId) is not null) {
            try {
                settings = await _settingsDao.GetSettings(username);
            } catch(Exception e) {
                _logger.LogError($"GET-/api/settings: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        } else {
            return Unauthorized(new { message = "Please log in with your Discord."});
        }
        return Ok(settings);
    }

    [HttpPost("settings")]
    public async Task<ActionResult<Settings>> SetSettings(SetSettingsRequest settingsReq) {

        _logger.LogInformation($"In set settings controller for {settingsReq.Username}");

        var sessionId = Request.Cookies[Strings.SESSION_ID];
        if(sessionId is not null && _sessionDao.GetSession(sessionId) is not null) {
            Settings newSettings = new() {
                Username = settingsReq.Username,
                Region = settingsReq.Region,
                PlayerSkillLevel = settingsReq.SkillLevel,
                HasDelayMod = settingsReq.HasDelayMod
            };

            try {
                var settings = await _settingsDao.GetSettings(newSettings.Username);
                if(settings is null) {
                    await _settingsDao.CreateSettings(newSettings);
                } else {
                    await _settingsDao.UpdateSettings(newSettings);
                }
                
                return Ok(newSettings);
            } catch(Exception e) {
                _logger.LogError($"POST-/api/settings/: {e.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError, new { error = e.Message });
            }
        } else {
            return Unauthorized(new { message = "Please log in with your Discord."});
        }
    }
}