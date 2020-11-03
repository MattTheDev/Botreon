using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Botreon.Models;
using Botreon.Models.Patreon;
using Botreon.Services;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Botreon.Handlers
{
    public class PatreonRequest : IRequest<CampaignMemberResponse> { }
    public class PatreonHandler : IRequestHandler<PatreonRequest, CampaignMemberResponse>
    {
        private readonly BotSettings _botSettings;
        private readonly LoggingService _loggingService;

        public PatreonHandler(
            IOptions<BotSettings> botSettings,
            LoggingService loggingService)
        {
            _botSettings = botSettings.Value;
            _loggingService = loggingService;
        }

        public async Task<CampaignMemberResponse> Handle(PatreonRequest request, CancellationToken cancellationToken)
        {
            var campaignMemberResponse = new CampaignMemberResponse
            {
                Data = new List<CampaignMemberData>(),
                Included = new List<CampaignMemberIncluded>()
            };

            var nextPage = "";
            var loopDeDoop = true;

            while (loopDeDoop)
            {
                var url =
                    $"https://www.patreon.com/api/oauth2/v2/campaigns/{_botSettings.PatreonCampaignId}/members?" +
                    $"include=currently_entitled_tiers,user&" +
                    $"fields%5Bmember%5D=patron_status,last_charge_status&" +
                    $"fields%5Buser%5D=social_connections,email,first_name,last_name";

                if (!string.IsNullOrEmpty(nextPage))
                {
                    url += $"&page%5Bcursor%5D={nextPage}";
                }

                try
                {
                    var webRequest =
                        (HttpWebRequest)WebRequest.Create(url);

                    webRequest.Headers["authorization"] = $"Bearer {_botSettings.Tokens.Patreon}";
                    var responseText = "";
                    var response = await webRequest.GetResponseAsync().ConfigureAwait(false);
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        responseText = await sr.ReadToEndAsync().ConfigureAwait(false);
                    }

                    var responseData = JsonConvert.DeserializeObject<CampaignMemberResponse>(responseText);
                    campaignMemberResponse.Data.AddRange(responseData.Data);
                    campaignMemberResponse.Included.AddRange(responseData.Included.Where(x => x.Type.Equals("user")));

                    if (responseData.Meta.Pagination.Cursors == null)
                    {
                        loopDeDoop = false;
                    }
                    else
                    {
                        nextPage = responseData.Meta.Pagination.Cursors.Next;
                    }
                }
                catch (Exception ex)
                {
                    await _loggingService.LogException($"Unable to get patron list from the Patreon API. See the issue here: {ex.Message}");
                }
            }

            return campaignMemberResponse;
        }
    }
}