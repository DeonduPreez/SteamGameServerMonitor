using System;
using System.Collections.Generic;
using SteamGameServerMonitor.Classes.Config;

namespace SteamGameServerMonitor.Classes.Discord
{
    public class DiscordWebhookEmbed
    {
        public DiscordWebhookEmbed()
        {
            fields = new List<DiscordWebhookEmbedField>();
        }

        public string title { get; set; }

        public string description { get; set; }

        public long color { get; set; }

        public List<DiscordWebhookEmbedField> fields { get; set; }

        public DiscordWebhookEmbedFooter footer { get; set; }

        public DiscordWebhookEmbed AddStatus(RequiredServer requiredServer)
        {
            return this;
        }

        public DiscordWebhookEmbed AddAddress(string ip, int serverPort)
        {
            throw new System.NotImplementedException();
        }

        public DiscordWebhookEmbed AddLocation(RequiredServer server)
        {
            throw new System.NotImplementedException();
        }

        public DiscordWebhookEmbed AddGame(RequiredServer server)
        {
            throw new System.NotImplementedException();
        }

        public DiscordWebhookEmbed AddCurrentMap(RequiredServer server)
        {
            throw new System.NotImplementedException();
        }

        public DiscordWebhookEmbed AddPlayers(RequiredServer server)
        {
            throw new System.NotImplementedException();
        }

        public DiscordWebhookEmbed AddFooter()
        {
            footer = new DiscordWebhookEmbedFooter()
            {
                text =
                    $"Metal Game Server Monitor | Last update: {DateTime.Now.ToLongDateString()} Sun, 2021-05-02 11:51:37PM" // TODO : Check if correct
            };
            return this;
        }
    }
}