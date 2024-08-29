using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs;

public class MatchupHub : Hub {
    private readonly ILogger<MatchupHub> _logger;

    public MatchupHub(ILogger<MatchupHub> logger) {
        _logger = logger;
    }

    public async Task AddToMatchupGroup(string matchupId, string username) {
        _logger.LogInformation($"Added {username} connection to matchup {matchupId}");
        try {
            await Groups.AddToGroupAsync(Context.ConnectionId, matchupId);
        } catch(Exception e) {
            _logger.LogError("ERROR: " + e.Message);
        }
    }

    public async Task RemoveFromMatchupGroup(string matchupId, string username) {
        _logger.LogInformation($"Removed {username}'s connection from matchup {matchupId}");
        try {
            await Clients.Group(matchupId).SendAsync("ExitMatchup", username);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, matchupId);
        } catch(Exception e) {
            _logger.LogError("ERROR: " + e.Message);
        }
    }

    public async Task SubscribeToMatchups(string username) {
        _logger.LogInformation($"Subscribed {username} connection to matchups.");
        try {
            await Groups.AddToGroupAsync(Context.ConnectionId, "MATCHUPS");
        } catch(Exception e) {
            _logger.LogError("ERROR: " + e.Message);
        }
    }

    public async Task UnsubscribeFromMatchups(string username) {
        _logger.LogInformation($"Unsubscribed {username} connection from matchups.");
        try {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "MATCHUPS");
        } catch(Exception e) {
            _logger.LogError("ERROR: " + e.Message);
        }
    }

    public async Task JoinMatchup(string matchupId, string visitor) {
        _logger.LogInformation($"{visitor} is joining matchup {matchupId}");
        try {
            await Clients.Group(matchupId).SendAsync("MatchupJoined", visitor);
        } catch(Exception e) {
            _logger.LogError("Error in JoinMatchup: " + e.Message);
        }
    }

    public async Task SubscribeToEvents(string user) {
        _logger.LogInformation($"In SendMessage method with {user}");
        try {
            await Clients.All.SendAsync("ReceiveMessage", $"Message from {user}!");
        } catch(Exception e) {
            _logger.LogError("Error in SendMessage: " + e.Message);
        }
    }

}