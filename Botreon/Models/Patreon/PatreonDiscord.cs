using Newtonsoft.Json;

namespace Botreon.Models.Patreon
{
    public class PatreonDiscord
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}