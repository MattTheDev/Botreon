using Newtonsoft.Json;

namespace Botreon.Models.Patreon
{
    public class Attributes
    {
        [JsonProperty("patron_status")]
        public string PatronStatus { get; set; }

        [JsonProperty("last_charge_status")]
        public string LastChargeStatus { get; set; }
    }
}