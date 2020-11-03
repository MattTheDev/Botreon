using static Botreon.Models.BotSettings;

namespace Botreon.Models.Patreon
{
    public class Patron
    {
        public string PatreonId { get; set; }
        public string DiscordId { get; set; }
        public TierRole TierRole { get; set; }
    }
}