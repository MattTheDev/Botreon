using System;
using System.Threading.Tasks;
using Botreon.Models;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Botreon.Services
{
    public class CommandHandlerService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commandService;
        private readonly BotSettings _botSettings;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlerService(
            DiscordSocketClient discord,
            CommandService commandService,
            IOptions<BotSettings> botSettings,
            IServiceProvider serviceProvider
        )
        {
            _discord = discord;
            _commandService = commandService;
            _botSettings = botSettings.Value;
            _serviceProvider = serviceProvider;

            _discord.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg) ||
                msg.Author.IsBot ||
                msg.Author.IsWebhook)
            {
                return;
            }

            var context = new SocketCommandContext(_discord, msg);
            var argPosition = 0;
            if (msg.HasStringPrefix(_botSettings.Prefix, ref argPosition) ||
                msg.HasMentionPrefix(_discord.CurrentUser, ref argPosition))
            {
                var result = await _commandService.ExecuteAsync(context, argPosition, _serviceProvider);

                if (!result.IsSuccess)
                {
                    await context.Channel.SendMessageAsync($"Sorry, {context.User.Username} something " +
                                                           $"went wrong -> {result}");
                }
            }
        }
    }
}