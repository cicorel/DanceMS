using System.Threading.Tasks;
using Android.Content;
using Microsoft.WindowsAzure.MobileServices;
using DanceMS.Abstractions;
using DanceMS.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(DroidLoginProvider))]
namespace DanceMS.Droid.Services
{
    public class DroidLoginProvider : ILoginProvider
    {
        Context context;

        public void Init(Context context)
        {
            this.context = context;
        }

        public async Task LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            await client.LoginAsync(context, provider, Constants.Constants.UrlScheme);
        }
    }
}