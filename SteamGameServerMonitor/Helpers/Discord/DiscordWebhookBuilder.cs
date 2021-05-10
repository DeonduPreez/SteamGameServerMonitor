using System.Collections.Generic;
using SteamGameServerMonitor.Classes.Config;
using SteamGameServerMonitor.Classes.Discord;

namespace SteamGameServerMonitor.Helpers.Discord
{
    public class DiscordWebhookBuilder
    {
        public static DiscordWebhookMessage BuildWebhookMessage(string ip, List<RequiredServer> servers)
        {
            var message = new DiscordWebhookMessage();

            for (var i = 0; i < servers.Count; i++)
            {
                var server = servers[i];
                var embed = new DiscordWebhookEmbed()
                    {
                        title = $"```{server.ServerName}```",
                        description = $"Connect: steam://connect/{ip}:{server.Port}",
                        color = 4437377 // TODO : Get a red and check if server's running or not
                    }
                    .AddStatus(server)
                    .AddAddress(ip, server.Port)
                    .AddLocation(server)
                    .AddGame(server)
                    .AddCurrentMap(server)
                    .AddPlayers(server)
                    .AddFooter();
                message.embeds.Add(embed);
            }

            return message;
        }

        // {
        //     "title": "```DB Retakes 1```",
        //     "description": "",
        //     "color": 4437377,
        //     "fields": [
        //     {
        //         "name": "Status",
        //         "value": ":green_circle: **Online**",
        //         "inline": true
        //     },
        //     {
        //         "name": "Address:Port",
        //         "value": "197.81.132.94:27016",
        //         "inline": true
        //     },
        //     {
        //         "name": "Location",
        //         "value": ":flag_za: ZA",
        //         "inline": true
        //     },
        //     {
        //         "name": "Game",
        //         "value": "CS:GO",
        //         "inline": true
        //     },
        //     {
        //         "name": "Current Map",
        //         "value": "de_dust2",
        //         "inline": true
        //     },
        //     {
        //         "name": "Players",
        //         "value": "2/11",
        //         "inline": true
        //     }
        //     ],
        //     "footer": {
        //         "text": "Metal Game Server Monitor | Last update: Sun, 2021-05-02 11:51:37PM"
        //     }
        // }
    }
}