using Newtonsoft.Json;

namespace Botreon.Models.Patreon
{
    public class Relationships
    {
        [JsonProperty("currently_entitled_tiers")]
        public CurrentlyEntitledTiers CurrentlyEntitledTiers { get; set; }
        public User User { get; set; }
    }
}