﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Botreon.Handlers;
using Botreon.Models;
using Botreon.Models.Patreon;
using Discord.WebSocket;
using MediatR;
using Microsoft.Extensions.Options;
using static Botreon.Models.BotSettings;

namespace Botreon.Services
{
    public class PatreonService : IDisposable
    {
        private readonly BotSettings _botSettings;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly LoggingService _loggingService;
        private readonly IMediator _mediator;

        public PatreonService(
            IOptions<BotSettings> botSettings,
            DiscordSocketClient discordSocketClient,
            LoggingService loggingService,
            IMediator mediator)
        {
            _botSettings = botSettings.Value;
            _discordSocketClient = discordSocketClient;
            _loggingService = loggingService;
            _mediator = mediator;
        }

        private Timer _roleReconciliationTimer;

        public void Init()
        {
            _roleReconciliationTimer = new Timer(async (e) =>
            {
                await _loggingService.LogAsync("Getting Active Patrons.");
                var patrons = await GetActivePatrons();

                await _loggingService.LogAsync("Processing Roles - Assigning.");
                await AssignRoles(patrons);

                await _loggingService.LogAsync("Processing Roles - Removing.");
                await RemoveRoles(patrons);

                await _loggingService.LogAsync("Finished Processing Patrons.");
            }, null, 0, _botSettings.Timers.RoleReconcilier);
        }

        private async Task AssignRoles(List<Patron> patrons)
        {
            var guild = _discordSocketClient?.GetGuild(_botSettings.DiscordGuildId);

            if (guild == null)
            {
                return;
            }

            var listOfRoles = _botSettings.TierRoles.Select(x => guild.GetRole(x.RoleId));

            foreach (var patron in patrons.Where(x => !string.IsNullOrWhiteSpace(x.DiscordId)))
            {
                if (!string.IsNullOrEmpty(patron.DiscordId))
                {
                    var role = listOfRoles.FirstOrDefault(x => x.Id == patron.TierRole.RoleId);
                    var discordUser = guild.GetUser(ulong.Parse(patron.DiscordId));

                    try
                    {
                        if (discordUser != null &&
                            !discordUser.Roles.Contains(role))
                        {
                            await discordUser.AddRoleAsync(role);
                        }
                    }
                    catch(Exception ex)
                    {
                        await _loggingService.LogException($"Unable to Add {role.Name} to {discordUser.Nickname ?? discordUser.Username}. Confirm the bot has Manage Role Access " +
                            $"and that the Bot's role is above the roles you are trying to manage. ({ex.Message})");
                    }
                }
            }
        }

        private async Task RemoveRoles(List<Patron> patrons)
        {
            var guild = _discordSocketClient?.GetGuild(_botSettings.DiscordGuildId);

            if (guild == null)
            {
                return;
            }

            var listOfRoles = _botSettings.TierRoles.Select(x => guild.GetRole(x.RoleId));

            var patronRoleUsers = guild.Users.Where(x => x.Roles.Intersect(listOfRoles).Any());

            foreach (var patron in patronRoleUsers)
            {
                try
                {
                    var isPatron = patrons.Where(x => !string.IsNullOrEmpty(x.DiscordId))
                        .FirstOrDefault(x => x.DiscordId.Equals(patron.Id.ToString()));

                    if (isPatron == null)
                    {
                        await patron.RemoveRolesAsync(listOfRoles);

                        continue;
                    }

                    var tierRoles = patron.Roles.Intersect(listOfRoles).ToList();

                    if (tierRoles.Count > 1)
                    {
                        await patron.RemoveRolesAsync(tierRoles.Where(x => x.Id != isPatron.TierRole.RoleId));
                    }
                }
                catch(Exception ex)
                {
                    await _loggingService.LogException($"Unable to remove roles from {patron.Nickname ?? patron.Username}. Confirm the bot has Manage Role Access " +
                        $"and that the Bot's role is above the roles you are trying to manage. ({ex.Message})");
                }
            }
        }

        private async Task<List<Patron>> GetActivePatrons()
        {
            var listOfPatrons = new List<Patron>();

            var response = await _mediator.Send(new PatreonRequest());

            if(response.Data == null || 
                response.Included == null || 
                response.Meta == null)
            {
                return listOfPatrons;
            }

            var paidPatrons = GetPaidPatrons(response);

            if (paidPatrons == null)
            {
                return listOfPatrons;
            }

            foreach (var data in paidPatrons)
            {
                var tierRole = GetTier(data);

                if (tierRole == null)
                {
                    continue;
                }

                listOfPatrons.Add(new Patron
                {
                    PatreonId = data.Relationships.User.Data.Id,
                    TierRole = tierRole
                });
            }

            foreach (var patron in listOfPatrons)
            {
                var patronIncluded = response.Included.FirstOrDefault(x => x.Id.Equals(patron.PatreonId));

                if (patronIncluded?.Attributes.SocialConnections.Discord != null)
                {
                    patron.DiscordId = patronIncluded.Attributes.SocialConnections.Discord.UserId;
                }
            }

            return listOfPatrons;
        }

        private List<CampaignMemberData> GetPaidPatrons(CampaignMemberResponse patronMetadata)
        {
            if (patronMetadata?.Data == null ||
                patronMetadata.Included == null ||
                patronMetadata.Data?.Count == 0 ||
                patronMetadata.Included?.Count == 0)
            {
                return null;
            }

            var parsedList = patronMetadata.Data.Where(x =>
                x.Attributes.PatronStatus != null &&
                x.Attributes.PatronStatus.Equals("active_patron") &&
                x.Attributes.LastChargeStatus != null &&
                x.Attributes.LastChargeStatus.Equals("Paid") &&
                x.Relationships.CurrentlyEntitledTiers?.Data != null &&
                x.Relationships.CurrentlyEntitledTiers.Data.Count > 0);

            return parsedList.ToList();
        }

        private TierRole GetTier(CampaignMemberData data)
        {
            if (data.Relationships.CurrentlyEntitledTiers.Data != null &&
            data.Relationships.CurrentlyEntitledTiers.Data.Count > 0)
            {
                foreach (var tierRole in _botSettings.TierRoles)
                {
                    if (data.Relationships.CurrentlyEntitledTiers.Data.Any(x => int.Parse(x.Id) == tierRole.TierId))
                    {
                        return tierRole;
                    }
                }
            }

            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _roleReconciliationTimer.Dispose();
            }
        }
    }
}
