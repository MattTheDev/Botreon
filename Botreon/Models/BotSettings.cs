using System.Collections.Generic;
using Newtonsoft.Json;

namespace Botreon.Models
{
    public class BotSettings
    {
        public string Prefix { get; set; }
        public int PatreonCampaignId { get; set; }
        public ulong DiscordGuildId { get; set; }

        [JsonProperty("Tokens")]
        public TokenSettings Tokens  { get; set; }

        [JsonProperty("Timers")]
        public TimerSettings Timers { get; set; }

        [JsonProperty("TierRoles")]
        public List<TierRole> TierRoles { get; set; }

        public class TokenSettings
        {
            public string Discord { get; set; }
            public string Patreon { get; set; }
        }

        public class TimerSettings
        {
            public int RoleReconcilier { get; set; }
        }

        public class TierRole
        {
            [JsonProperty("TierName")]
            public string TierName { get; set; }

            [JsonProperty("TierId")]
            public int TierId { get; set; }

            [JsonProperty("RoleId")]
            public ulong RoleId { get; set; }
        }
    }
}