using System.Collections.Generic;

namespace SteamGameServerMonitor.Classes.SteamResponses
{
    public class GetServerResponse
    {
        public bool success { get; set; }

        public List<SteamServer> servers { get; set; }
    }
}