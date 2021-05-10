using System.Collections.Generic;

namespace SteamGameServerMonitor.Classes.Discord
{
    public class DiscordWebhookMessage
    {
        public DiscordWebhookMessage()
        {
            content = string.Empty;
            embeds = new List<DiscordWebhookEmbed>();
        }

        public string content { get; set; }

        public List<DiscordWebhookEmbed> embeds { get; set; }
    }
}