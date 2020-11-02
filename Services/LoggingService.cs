using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Botreon.Services
{
    public class LoggingService
    {
        public LoggingService(
            DiscordSocketClient discord,
            CommandService commandService)
        {
            discord.Log += OnLogAsync;
            commandService.Log += OnLogAsync;
        }

        private async Task OnLogAsync(LogMessage message)
        {
            var logText = $"[{DateTime.UtcNow:MM/dd/yyyy hh:mm:ss}] {message}";

            await Console.Out.WriteLineAsync(logText);
        }
    }
}