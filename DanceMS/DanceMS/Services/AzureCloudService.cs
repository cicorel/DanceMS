using DanceMS.Abstractions;
using DanceMS.Models;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
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

        public async Task<MobileServiceUser> LoginAsync(MobileServiceAuthenticationProvider provider)
        {
            var loginProvider = DependencyService.Get<ILoginProvider>();
            return await loginProvider.LoginAsync(client, provider);
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
