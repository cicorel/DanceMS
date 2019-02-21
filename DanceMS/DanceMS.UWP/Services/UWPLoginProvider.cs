using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using DanceMS.Abstractions;
using DanceMS.UWP.Services;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(UWPLoginProvider))]
namespace DanceMS.UWP.Services
{
    public class UWPLoginProvider : ILoginProvider
    {
        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var concreteClient = client as MobileServiceClient; //extensionmethod we need is not on interface but on implementation class

                LoginContinuation = concreteClient;
                var user = await client.LoginAsync(provider, Constants.Constants.UrlScheme);
                return user;
            }
            catch (Exception e)
            {
                e.Data["method"] = "LoginAsync";

            }
            finally
            {
                LoginContinuation = null;
            }
            return null;

        }

        private MobileServiceClient LoginContinuation = null;

        public void ContinueLogin(Uri uri)
        {
            if (LoginContinuation != null)
            {
                LoginContinuation.ResumeWithURL(uri);
                LoginContinuation = null;
            }
        }
    }
}