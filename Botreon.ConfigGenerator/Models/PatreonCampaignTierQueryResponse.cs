using System.Collections.Generic;
using Newtonsoft.Json;

namespace Botreon.ConfigGenerator.Models
{
    public class PatreonCampaignTierQueryResponse
    {
        [JsonProperty("included")]
        public List<IncludedTier> Included { get; set; }

        public class Attributes
        {
            [JsonProperty("title")]
            public string Title { get; set; }
        }

        public class IncludedTier
        {
            [JsonProperty("attributes")]
            public Attributes Attributes { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }
        }
    }
}
