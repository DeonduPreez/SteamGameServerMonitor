using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Metal.Emailing.Models;
using Metal.Emailing.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SteamGameServerMonitor.Classes.Config;
using SteamGameServerMonitor.Classes.SteamResponses;
using SteamQueryNet;

namespace SteamGameServerMonitor
{
    public class Worker : BackgroundService
    {
        private static readonly HttpClient Client = new();

        private readonly ILogger<Worker> _logger;
        private readonly IReadOnlyList<RequiredServer> _requiredServers;
        private readonly string _ip;
        private readonly MailSettings _mailSettings;
        private readonly string _fromEmail;
        private readonly string _displayName;
        private readonly IReadOnlyList<string> _recipients;
        private readonly TimeSpan _delayTime;
        private readonly TimeSpan _afterAlertDelayTime;
        private bool _alertSent = true;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _delayTime = configuration.GetValue<TimeSpan>("Setup:DelayTime");
            _afterAlertDelayTime = configuration.GetValue<TimeSpan>("Setup:AfterAlertDelayTime");
            _ip = configuration.GetValue<string>("Server:IP");
            _requiredServers = configuration.GetSection("Server:Servers").Get<List<RequiredServer>>().AsReadOnly();
            _mailSettings = configuration.GetSection("MailDetails:EmailSettings").Get<MailSettings>();
            _fromEmail = configuration.GetValue<string>("MailDetails:FromEmail");
            _displayName = configuration.GetValue<string>("MailDetails:DisplayName");
            _recipients = configuration.GetSection("MailDetails:Recipients").Get<List<string>>().AsReadOnly();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _alertSent = false;
                    _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);

                    var serverPort = 27015;
                    var serverQuery = new ServerQuery(_ip, (ushort) serverPort);
                    var serverInfo = await serverQuery.GetServerInfoAsync();


                    var url = $"http://api.steampowered.com/ISteamApps/GetServersAtAddress/v0001?addr={_ip}&format=json";
                    var serverResponse = await GetServerResponse(url);
                    var requiredServers = _requiredServers.ToList();
                    var outOfDateServers = new List<RequiredServer>();
                    foreach (var steamServer in serverResponse.servers)
                    {
                        var foundServer = requiredServers.FirstOrDefault(s => s.Port == steamServer.gameport);
                        if (foundServer == null)
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(steamServer.reject))
                        {
                            outOfDateServers.Add(foundServer);
                        }

                        requiredServers.Remove(foundServer);
                    }

                    if (requiredServers.Any())
                    {
                        ServersDown(requiredServers);
                        _alertSent = true;
                    }

                    if (outOfDateServers.Any())
                    {
                        ServersNeedUpdates(outOfDateServers);
                        _alertSent = true;
                    }

                    var delayTime = _alertSent ? _afterAlertDelayTime : _delayTime;
                    await Task.Delay(Convert.ToInt32(delayTime.TotalMilliseconds), stoppingToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                AlertFatalError(e);
            }
        }

        #region Helper Methods

        private static string GetRequiredServerList(IEnumerable<string> servers)
        {
            var sb = new StringBuilder();
            var serverArr = servers as string[] ?? servers.ToArray();
            for (var i = 0; i < serverArr.Length; i++)
            {
                var server = serverArr.ElementAt(i);
                sb.Append($"<li>{server}</li>\r\n");
            }

            return sb.ToString();
        }

        private void SendEmail(string subject, string body)
        {
            var smtpProvider = new SmtpProvider(_mailSettings);
            var mailRequest = new EmailRequest()
            {
                FromEmail = _fromEmail,
                DisplayName = _displayName,
                Subject = subject,
                Body = body,
                IsHtml = true,
                Recipients = _recipients.ToList()
            };

            var response = smtpProvider.SendEmail(mailRequest);
        }

        private void ServersDown(IEnumerable<RequiredServer> serverPortsNotRunning)
        {
            const string subject = "DB - Some servers are down";
            var body = $@"
<div>
    <div>
        The following servers are currently not running:
        <ul>
            {GetRequiredServerList(serverPortsNotRunning.Select(sp => sp.ServerName))}
        </ul>
    </div>
</div>";
            SendEmail(subject, body);
        }

        private void ServersNeedUpdates(IEnumerable<RequiredServer> serverPortsNotRunning)
        {
            const string subject = "DB - Some servers need updates";
            var body = $@"
<div>
    <div>
        The following servers require updates:
        <ul>
            {GetRequiredServerList(serverPortsNotRunning.Select(sp => sp.ServerName))}
        </ul>
    </div>
</div>";
            SendEmail(subject, body);
        }

        private void AlertFatalError(Exception exception)
        {
            const string subject = "DB - Server Monitor Fatal Error";
            var body = $@"
<div>
    <div>
        The following fatal error occurred: {exception}
    </div>
</div>";
            SendEmail(subject, body);
        }

        #endregion

        #region Static Methods

        private static async Task<GetServerResponse> GetServerResponse(string url)
        {
            var response = await Client.GetAsync(url);
            return response.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<SteamApiResponse<GetServerResponse>>(await response.Content.ReadAsStringAsync())
                    ?.response
                : null;
        }

        #endregion
    }
}