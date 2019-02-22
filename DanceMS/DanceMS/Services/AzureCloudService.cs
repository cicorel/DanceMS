using DanceMS.Abstractions;
using DanceMS.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DanceMS.Services
{
    public class AzureCloudService : ICloudService
    {
        MobileServiceClient client;

        public AzureCloudService()
        {
            client = new MobileServiceClient(Constants.Constants.ApplicationURL);
        }

        public ICloudTable<T> GetTable<T>() where T : TableData => new AzureCloudTable<T>(client);

        public async Task LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            var loginProvider = DependencyService.Get<ILoginProvider>();
            await loginProvider.LoginAsync(client, provider);
        }

        public async Task LogoutAsync()
        {
            if(client.CurrentUser == null || client.CurrentUser.MobileServiceAuthenticationToken == null)
            {
                return;
            }

            //Log out of the identity provider

            //Invalidate the token on the mobile backend
            var authUri = new Uri($"{client.MobileAppUri}/.auth/logout");
            using ( var httpClient = new HttpClient() ) {
                httpClient.DefaultRequestHeaders.Add("X-ZUMO-AUTH", client.CurrentUser.MobileServiceAuthenticationToken);
                await httpClient.GetAsync(authUri);
            }

            //Remove the token from the cache
            DependencyService.Get<ILoginProvider>().RemoveTokenFromSecureStore();

            //Remove the token from the MobileServiceClient
            await client.LogoutAsync();
        }

        List<AppServiceIdentity> identities = null;

        public async Task<AppServiceIdentity> GetIdentityAsync()
        {
            if (client.CurrentUser == null || client.CurrentUser?.MobileServiceAuthenticationToken == null)
            {
                throw new InvalidOperationException("Not Authenticated");
            }

            if (identities == null)
            {
                identities = await client.InvokeApiAsync<List<AppServiceIdentity>>("/.auth/me");
            }

            if (identities.Count > 0)
                return identities[0];
            return null;
        }


    }
}
