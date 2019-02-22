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
        public async Task LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var concreteClient = client as MobileServiceClient; //extensionmethod we need is not on interface but on implementation class

                LoginContinuation = concreteClient;
                await client.LoginAsync(provider, Constants.Constants.UrlScheme);
            }
            catch (Exception e)
            {
                e.Data["method"] = "LoginAsync";

            }
            finally
            {
                LoginContinuation = null;
            }

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