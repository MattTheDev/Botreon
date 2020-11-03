using System.Collections.Generic;

namespace Botreon.Models.Patreon
{
    public class CampaignMemberResponse
    {
        public List<CampaignMemberData> Data { get; set; }
        public List<CampaignMemberIncluded> Included { get; set; }
        public Meta Meta { get; set; }
    }
}