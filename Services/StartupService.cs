using System;
using System.Reflection;
using System.Threading.Tasks;
using Botreon.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Botreon.Services
{
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commandService;
        private readonly BotSettings _botSettings;

        public StartupService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commandService,
            IOptions<BotSettings> botSettings)
        {
            _provider = provider;
            _discord = discord;
            _commandService = commandService;
            _botSettings = botSettings.Value;
        }

        public async Task StartAsync()
        {
            var discordToken = _botSettings.Tokens.Discord;

            if (string.IsNullOrEmpty(discordToken))
            {
                throw new NullReferenceException("There was no Discord token provided in the BotSettings.json.");
            }

            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();

            await _commandService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
    }
}