using System.Collections.Generic;

namespace Botreon.Models.Patreon
{
    public class PatreonLookupResult
    {
        public string PatreonId { get; set; }
        public string DiscordId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> EntitledTiers { get; set; }
        public string PatronStatus { get; set; }
        public string PaymentStatus { get; set; }
    }
}