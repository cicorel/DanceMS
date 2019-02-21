using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Windows.UI.Popups;
using DanceMS;
using DanceMS.Abstractions;

namespace DanceMS.UWP
{
    public sealed partial class MainPage : ILoginProvider
    {

        public MainPage()
        {
            this.InitializeComponent();
            LoadApplication(new DanceMS.App());
        }

        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return await client.LoginAsync(provider, Constants.Constants.UrlScheme);
        }
    }
}
