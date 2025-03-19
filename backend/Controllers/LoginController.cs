using backend.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Discord.Rest;
using Discord;
using backend.Daos;
using backend.Factories;
using backend.Models;
using backend.Daos.Sessions;
using System.Text.Json;
using System.Net;
using Microsoft.Extensions.Options;
using backend.Setting;
using backend.Types;

namespace backend.Controllers;

[ApiController]
[Route("api")]
public class LoginController : ControllerBase {

    private readonly HttpClient _client;
    private readonly ILogger<LoginController> _logger;
    private readonly IUserDao _userDao;
    private readonly ISessionDao _sessionDao;
    private readonly IOptions<UrlSettings> _urlSettings;

    public LoginController(ILogger<LoginController> logger, IHttpClientFactory client, IFactory factory, IOptions<UrlSettings> urlSettings) {
        _logger = logger;
        _client = client.CreateClient(Strings.DISCORD);
        _userDao = factory.GetUserDao();
        _sessionDao = factory.GetSessionDao();
        _urlSettings = urlSettings;
    }

    [HttpGet("user")]
    public async Task<ActionResult<User>> GetCurrentUser() {
        try {
            var sessionId = Request.Cookies[Strings.SESSION_ID];
            if(sessionId is not null) {
                var session = await _sessionDao.GetSession(sessionId);

                if(session is not null) {
                    var user = await _userDao.GetUser(session.UserID);
                    return Ok(JsonSerializer.Serialize(user));
                }
            }
        } catch(Exception e) {
            _logger.LogError("GetCurrentUser: " + e.Message);
        }

        return NotFound(new { message = "No logged in user was found."});
    }

    [HttpGet("auth/callback")]
    public async Task<ActionResult<User>> SetCurrentUser(string code) {

        TokenResponse? tokenResp;
        try {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, Strings.TOKEN)
            {
                Content = new FormUrlEncodedContent([
                    new KeyValuePair<string, string>(Strings.GRANT_TYPE, Strings.AUTHORIZATION_CODE),
                    new KeyValuePair<string, string>(Strings.CODE_KEY, code),
                    new KeyValuePair<string, string>(Strings.REDIRECT_URI, _urlSettings.Value.ServerBaseUrl + Strings.CALLBACK_ENDPOINT),
                ])
            };

            req.Content.Headers.ContentType = new MediaTypeHeaderValue(Strings.URL_FORM_MEDIA_TYPE);

            var res = await _client.SendAsync(req);

            res.EnsureSuccessStatusCode();

            tokenResp = await res.Content.ReadFromJsonAsync<TokenResponse>();

        } catch(HttpRequestException e) {
            _logger.LogError(e.Message);
            return BadRequest(new { error = e.Message });
        }

        return Redirect(_urlSettings.Value.ServerBaseUrl + Strings.LOGIN_ENDPOINT + tokenResp!.access_token);
    }

    [HttpGet("login/{token}")]
    public async Task<IActionResult> GetUserInfo(string token) {
        DiscordRestClient discordClient = new();
        try
        {
            await discordClient.LoginAsync(TokenType.Bearer, token);
        } catch (Exception e) {
            return BadRequest(new { error = e.Message });
        }

        User? user;
        try {
            user = await _userDao.GetUser(discordClient.CurrentUser.Username);

            if(user is null) {
                user = new User() {
                    Id = discordClient.CurrentUser.Id,
                    Username = discordClient.CurrentUser.Username,
                    AvatarUrl = discordClient.CurrentUser.GetAvatarUrl(ImageFormat.Auto),
                    IsBanned = false,
                    Created = DateTime.Now
                };

                await _userDao.CreateUser(user);
            }
        } catch(Exception e) {
            _logger.LogError(e.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, new { error = e.Message });
        }

        //new session for user
        var session = new Session
        {
            SessionId = Guid.NewGuid().ToString(),
            UserID = user.Username
        };

        //create session cookie
        var sessionCookieOpts = new CookieOptions
        {
            Domain = _urlSettings.Value.Domain,
            SameSite = SameSiteMode.None,
            HttpOnly = true,
            IsEssential = true,
            Secure = true
        };

        Response.Cookies.Append(Strings.SESSION_ID, session.SessionId, sessionCookieOpts);

        try {
            await _sessionDao.DeleteAllUserSessions(user.Username);
            await _sessionDao.SetSession(session);
        } catch(Exception e) {
            _logger.LogDebug($"Failed to write session. Message: {e.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError, new { error = e.Message });
        }

        return Redirect(_urlSettings.Value.ClientBaseUrl + Strings.MATCHES_ENDPOINT);
    }
}