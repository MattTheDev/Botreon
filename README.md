![.NET Core](https://github.com/MattTheDev/Botreon/workflows/.NET%20Core/badge.svg)

# Botreon
.NET Core bot for syncing Patreons / Tiers and Rewards within Discord

## Information to Gather for Setup
### Discord Bot Token - STEP ONE

1. Visit the [Discord Developers Portal](http://discord.com/developers/applications)
2. Log in with your Discord Username and Password
3. Create a new Application, Name it your desired bot name.
4. Once created, click Bot in the left navigation menu.
5. Click add bot, and confirm with 'Yes, do it!'
6. Once the bot has been created, click 'Click to Reveal Token'
7. Save this token. >> DO NOT SHARE IT <<

### Patreon OAuth Token - STEP TWO

1. Create a new Client on the [Patreon Developer Portal](https://www.patreon.com/portal/registration/register-clients)
2. Click Create Click
3. Enter the required information, for example:
   App Name: My Bot
   Description: Discord Bot to Sync Discord/Tier Roles
   App Category: Community
   Redirect Url: http://localhost (this wont be used)
   Client API Version: 2 <- Very important
4. Save the client, and you will be presented with a Client ID, Secret, Creator's Access Token .. 
5. Save the Creator's Access Token >> DO NOT SHARE THIS WITH ANYONE <<

### Patreon Reward Tier Ids - STEP THREE

1. Visit your Patreon page (ie: https://patreon.com/CouchBot)
2. Right click your reward tier "Join" button and click Copy URL
3. Paste the URL into your favorite text editor.
4. You will have something like this: https://www.patreon.com/join/CouchBot/checkout?rid=4538910
5. Save the NUMBER after rid= .. and repeat for all of your tiers.

### Discord Role Ids - STEP FOUR

1. Create your Discord Roles in Discord.
2. Once created, go to your server and choose a channel that you can clean up once your done gathering this info.
3. Tag your first role, @TestRole, and put a \ in front of the @. Send to chat.
4. You will see something like this: <@&773021743924510720> .. Save the number, and repeat for every tier.

## Putting it all together

```
{
  "Prefix": "&", <-- Put anything you want here
  "PatreonCampaignId": 0, <-- 
  "DiscordGuildId": 0, <-- Right click your Server in Discord, click Copy Id. Put it here.
  "Tokens": {
    "Discord": "REPLACE_WITH_YOUR_TOKEN", <- Your Discord Bot Token you created in STEP ONE above.
    "Patreon": "REPLACE_WITH_YOUR_TOKEN" <- Your Patreon Creator's Access Token you created in STEP TWO above.
  },
  "Timers": {
    "RoleReconcilier": 60000 <- Time between checks for new Patrons / Roles in milliseconds - 60 seconds * 1000. Leave this.
  },
  "TierRoles": [
    {
      "TierName": "REPLACE_WITH_YOUR_TIER_NAME_1", <- Name of the Tier / Role 
      "TierId": 0, <- Tier Id for the Tier from STEP THREE
      "RoleId": 0 <- Role Id for the Tier from STEP FOUR
    },
    {
      "TierName": "REPLACE_WITH_YOUR_TIER_NAME_2", <- Name of the Tier / Role 
      "TierId": 0, <- Tier Id for the Tier from STEP THREE
      "RoleId": 0 <- Role Id for the Tier from STEP FOUR
    },
    {
      // REPEAT AS NEEDED //
    }
  ]
}
```

## Have Any Questions?

  Open an issue, or join us on our [Discord](https://discord.gg/4PpDrCX) for this and more projects.

## Support

Not required. Always appreciated <3

<a href="https://discord.gg/4PpDrCX">
  <img src="https://discordapp.com/api/guilds/767835739177091083/widget.png?style=shield" alt="Discord Server">
</a>

<br />

<a href="https://paypal.me/dawgeth">
  <img height="32" src="https://github.com/everdrone/coolbadge/raw/master/badges/Paypal/Beer/Dark/Short.png" />
</a>

<br />

[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/E1E2JLQE)
