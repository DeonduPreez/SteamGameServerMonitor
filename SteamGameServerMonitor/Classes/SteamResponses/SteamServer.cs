namespace SteamGameServerMonitor.Classes.SteamResponses
{
    public class SteamServer
    {
        public const string OutOfDateString = "bad_version";

        public string reject { get; set; }

        public string addr { get; set; }

        public int gmsindex { get; set; }

        public int appid { get; set; }

        public string gamedir { get; set; }

        public int region { get; set; }

        public bool secure { get; set; }

        public bool lan { get; set; }

        public int gameport { get; set; }

        public int specport { get; set; }
    }
}