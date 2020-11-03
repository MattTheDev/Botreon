using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AspNet.Security.OAuth.Patreon;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Botreon.ConfigGenerator.Models;
using System.Text.Json;
using static Botreon.ConfigGenerator.Models.PatreonCampaignTierQueryResponse;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http;

namespace Botreon.ConfigGenerator
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownProxies.Add(IPAddress.Parse("10.0.0.100"));
                options.KnownProxies.Add(IPAddress.Parse("127.0.10.1"));
                options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = PatreonAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = PatreonAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Cookie.IsEssential = true;
            })
            .AddPatreon(options =>
            {
                options.ClientId = "CLIENT_ID_HERE";
                options.ClientSecret = "CLIENT_SECRET_HERE";

                options.SaveTokens = true;
                options.AuthorizationEndpoint = "https://patreon.com/oauth2/authorize";
                options.TokenEndpoint = "https://patreon.com/api/oauth2/token";

                options.Scope.Add("identity");
                options.Scope.Add("campaigns");

                options.ClaimActions.MapAll();

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        try
                        {
                            var campaignId = await GetCampaignId(context.AccessToken);
                            var tierList = await GetTierList(context.AccessToken);

                            var model = new PatreonAuthUserModel
                            {
                                CampaignId = campaignId,
                                Tiers = tierList
                            };

                            using var jsonDoc = JsonDocument.Parse(JsonConvert.SerializeObject(model));
                            context.RunClaimActions(jsonDoc.RootElement);
                        }
                        catch (Exception e)
                        {
                            // Error. We should add logging here.
                        }
                    }
                };
            });
        }

        private async Task<int> GetCampaignId(string accessToken)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://www.patreon.com/api/oauth2/v2/campaigns");
            webRequest.ContentType = "application/json; charset=utf-8";
            webRequest.Headers.Add("Accepts", "application/json");
            webRequest.Headers.Add("Authorization", $"Bearer {accessToken}");

            string str;
            using (var streamReader = new StreamReader((await webRequest.GetResponseAsync()).GetResponseStream()))
            {
                str = streamReader.ReadToEnd();
            }

            var user = JsonConvert.DeserializeObject<PatreonCampaignQueryResponse>(str);

            return int.Parse(user.Data.FirstOrDefault().Id);
        }

        private async Task<List<IncludedTier>> GetTierList(string accessToken)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://www.patreon.com/api/oauth2/v2/campaigns/3885253?include=tiers&fields%5Btier%5D=title");
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Accepts", "application/json");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");

            string str;
            using (var streamReader = new StreamReader((await request.GetResponseAsync()).GetResponseStream()))
            {
                str = streamReader.ReadToEnd();
            }

            var response = JsonConvert.DeserializeObject<PatreonCampaignTierQueryResponse>(str);

            return response.Included;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
