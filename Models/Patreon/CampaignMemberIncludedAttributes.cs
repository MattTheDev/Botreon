using Newtonsoft.Json;

namespace Botreon.Models.Patreon
{
    public class CampaignMemberIncludedAttributes
    {
        [JsonProperty("social_connections")]
        public SocialConnections SocialConnections { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public string Email { get; set; }
    }
}