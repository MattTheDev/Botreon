
using System.Text.Json.Serialization;

namespace Botreon.Models
{
    public class BotSettings
    {
        public string Prefix { get; set; }
        public int PatreonCampaignId { get; set; }

        [JsonPropertyName("Tokens")]
        public TokenSettings Tokens  { get; set; }

        public class TokenSettings
    {
            public string Discord { get; set; }
            public string Patreon { get; set; }
        }
    }
}