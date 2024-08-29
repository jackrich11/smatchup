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
        _client = client.CreateClient("discord");
        _userDao = factory.GetUserDao();
        _sessionDao = factory.GetSessionDao();
        _urlSettings = urlSettings;
    }

    [HttpGet("user")]
    public async Task<ActionResult<User>> GetCurrentUser() {
        try {
            var sessionId = Request.Cookies["session-id"];
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

    // [HttpGet("auth/callback")]
    // public async Task<ActionResult<User>> SetCurrentUser(string code) {

    //     TokenResponse? tokenResp;
    //     string retVal;
    //     try {
    //         tokenResp = await SendTokenReq(new FormUrlEncodedContent(new[] {
    //             new KeyValuePair<string, string>("grant_type", "authorization_code"),
    //             new KeyValuePair<string, string>("code", code),
    //             new KeyValuePair<string, string>("redirect_uri", _urlSettings.Value.ServerBaseUrl + "/api/auth/callback"),
    //         }));
    //         retVal = "AFTER CALL";
    //     } catch(HttpRequestException e) {
    //         _logger.LogError(e.Message);
    //         retVal = "in catch block";
    //         return BadRequest(e.Message + " 1 " + e.HttpRequestError);
    //     }
    //     System.Diagnostics.Trace.WriteLine("1");
        


    //     DiscordRestClient discordClient = new();
    //     try
    //     {
    //         await discordClient.LoginAsync(TokenType.Bearer, tokenResp!.access_token);
    //     } catch (Exception ex) {
    //         return BadRequest(ex.Message + " login catch block");
    //     }


    //     _logger.LogInformation("Refreshing user access token.");
    //     try {
    //         tokenResp = await SendTokenReq(new FormUrlEncodedContent(new[] {
    //             new KeyValuePair<string, string>("grant_type", "refresh_token"),
    //             new KeyValuePair<string, string>("refresh_token", tokenResp!.refresh_token),
    //         }));
    //     } catch(HttpException e) {
    //         _logger.LogError(e.Message);
    //         return BadRequest(e.Message + "2");
    //     }

    //     try {
    //         await discordClient.LoginAsync(TokenType.Bearer, tokenResp!.access_token);
    //     } catch(Exception e) {
    //         _logger.LogError(e.Message);
    //         return BadRequest(e.Message + "3");
    //     }
        



    //     var user = await _userDao.GetUser(discordClient.CurrentUser.Username);

    //     if(user is null) {
    //         user = new User() {
    //             Id = discordClient.CurrentUser.Id,
    //             Username = discordClient.CurrentUser.Username,
    //             AvatarUrl = discordClient.CurrentUser.GetAvatarUrl(ImageFormat.Auto),
    //             IsBanned = false,
    //             Created = DateTime.Now
    //         };

    //         await _userDao.CreateUser(user);
    //     }

    //     //new session for user
    //     var session = new Session
    //     {
    //         SessionId = Guid.NewGuid().ToString(),
    //         UserID = user.Username
    //     };

    //     _logger.LogInformation("GUID: " + session.SessionId);

    //     //create session cookie
    //     var sessionCookieOpts = new CookieOptions
    //     {
    //         // Domain = "localhost",
    //         HttpOnly = true,
    //         IsEssential = true,
    //         SameSite = SameSiteMode.Lax
    //     };

    //     Response.Cookies.Append("session-id", session.SessionId, sessionCookieOpts);

    //     try {
    //         await _sessionDao.DeleteAllUserSessions(user.Username);
    //         await _sessionDao.SetSession(session);
    //     } catch(Exception e) {
    //         _logger.LogDebug($"Failed to write session. Message: {e.Message}");
    //         return StatusCode((int)HttpStatusCode.InternalServerError);
    //     }

    //     return Redirect(_urlSettings.Value.ClientBaseUrl + "/matches");
    // }

    [HttpGet("auth/callback")]
    public async Task<ActionResult<User>> SetCurrentUser(string code) {

        TokenResponse? tokenResp;
        try {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "token")
            {
                Content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", _urlSettings.Value.ServerBaseUrl + "/api/auth/callback"),
                })
            };

            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var res = await _client.SendAsync(req);

            res.EnsureSuccessStatusCode();

            tokenResp = await res.Content.ReadFromJsonAsync<TokenResponse>();

        } catch(HttpRequestException e) {
            _logger.LogError(e.Message);
            return BadRequest(e.Message + " 1 " + e.HttpRequestError);
        }
        System.Diagnostics.Trace.WriteLine("1");

        return Redirect(_urlSettings.Value.ServerBaseUrl + "/api/login/" + tokenResp!.access_token);
        
        // DiscordRestClient discordClient = new();
        // try
        // {

        //     await discordClient.LoginAsync(TokenType.Bearer, tokenResp!.access_token);
        // } catch (Exception ex) {
        //     return BadRequest(ex.Message + " login catch block " + tokenResp is not null ? tokenResp!.access_token : "NULL");
        // }

        // _logger.LogInformation("Refreshing user access token.");
        // try {
        //     HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "token")
        //     {
        //         Content = new FormUrlEncodedContent(new[] {
        //             new KeyValuePair<string, string>("grant_type", "refresh_token"),
        //             new KeyValuePair<string, string>("refresh_token", tokenResp!.refresh_token),
        //         })
        //     };

        //     req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        //     var res = await _client.SendAsync(req);

        //     res.EnsureSuccessStatusCode();

        //     tokenResp = await res.Content.ReadFromJsonAsync<TokenResponse>();
        // } catch(HttpException e) {
        //     _logger.LogError(e.Message);
        //     return BadRequest(e.Message + "2");
        // }
        
        // User? user;
        // try {
        //     user = await _userDao.GetUser(discordClient.CurrentUser.Username);

        //     if(user is null) {
        //         user = new User() {
        //             Id = discordClient.CurrentUser.Id,
        //             Username = discordClient.CurrentUser.Username,
        //             AvatarUrl = discordClient.CurrentUser.GetAvatarUrl(ImageFormat.Auto),
        //             IsBanned = false,
        //             Created = DateTime.Now
        //         };

        //         await _userDao.CreateUser(user);
        //     }
        // } catch(Exception e) {
        //     _logger.LogError(e.Message);
        //     return StatusCode((int)HttpStatusCode.InternalServerError);
        // }


        // //new session for user
        // var session = new Session
        // {
        //     SessionId = Guid.NewGuid().ToString(),
        //     UserID = user.Username
        // };

        // _logger.LogInformation("GUID: " + session.SessionId);

        // //create session cookie
        // var sessionCookieOpts = new CookieOptions
        // {
        //     // Domain = "localhost",
        //     HttpOnly = true,
        //     IsEssential = true,
        //     SameSite = SameSiteMode.Lax
        // };

        // Response.Cookies.Append("session-id", session.SessionId, sessionCookieOpts);

        // try {
        //     await _sessionDao.DeleteAllUserSessions(user.Username);
        //     await _sessionDao.SetSession(session);
        // } catch(Exception e) {
        //     _logger.LogDebug($"Failed to write session. Message: {e.Message}");
        //     return StatusCode((int)HttpStatusCode.InternalServerError);
        // }

        // return Redirect(_urlSettings.Value.ClientBaseUrl + "/matches");
    }

    [HttpGet("login/{token}")]
    public async Task<IActionResult> GetUserInfo(string token) {
        DiscordRestClient discordClient = new();
        try
        {
            await discordClient.LoginAsync(TokenType.Bearer, token);
        } catch (Exception ex) {
            return BadRequest(ex.Message + " login catch block " + token);
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
            return StatusCode((int)HttpStatusCode.InternalServerError);
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

        Response.Cookies.Append("session-id", session.SessionId, sessionCookieOpts);

        try {
            await _sessionDao.DeleteAllUserSessions(user.Username);
            await _sessionDao.SetSession(session);
        } catch(Exception e) {
            _logger.LogDebug($"Failed to write session. Message: {e.Message}");
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        return Redirect(_urlSettings.Value.ClientBaseUrl + "/matches");
    }
}