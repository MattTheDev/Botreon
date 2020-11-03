using System.Threading.Tasks;
using Discord.Commands;

namespace Botreon.Modules
{
    public class UtilityCommands : ModuleBase
    {
        [Command("ping")]
        [Summary("Simple command to confirm if the bot is able to response to user input.")]
        public Task PingAsync()
        {
            return ReplyAsync("Pong!");
        }
    }
}