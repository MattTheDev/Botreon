using System.Collections.Generic;
using Newtonsoft.Json;

namespace Botreon.ConfigGenerator.Models
{
    public class PatreonCampaignQueryResponse
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }

        [JsonProperty("meta")]
        public MetaData Meta { get; set; }

        public class Attributes
        {
        }

        public class Datum
        {
            [JsonProperty("attributes")]
            public Attributes Attributes { get; set; }

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; }
        }

        public class Pagination
        {
            [JsonProperty("total")]
            public int Total { get; set; }
        }

        public class MetaData
        {
            [JsonProperty("pagination")]
            public Pagination Pagination { get; set; }
        }
    }
}
