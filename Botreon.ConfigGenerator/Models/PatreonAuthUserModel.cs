using System.Collections.Generic;
using static Botreon.ConfigGenerator.Models.PatreonCampaignTierQueryResponse;

namespace Botreon.ConfigGenerator.Models
{
    public class PatreonAuthUserModel
    {
        public int CampaignId { get; set; }
        public List<IncludedTier> Tiers { get; set; }
    }
}
