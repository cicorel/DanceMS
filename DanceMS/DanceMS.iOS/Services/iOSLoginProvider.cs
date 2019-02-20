using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using DanceMS.Abstractions;
using DanceMS.iOS.Services;
using UIKit;
using Newtonsoft.Json.Linq;

[assembly: Xamarin.Forms.Dependency(typeof(iOSLoginProvider))]
namespace DanceMS.iOS.Services
{
    public class iOSLoginProvider : ILoginProvider
    {
        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client)
        {
            return await client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, MobileServiceAuthenticationProvider.Google, Constants.Constants.UrlScheme);
        }

        public UIViewController RootView => UIApplication.SharedApplication.KeyWindow.RootViewController;
    }
}