using DanceMS.Abstractions;
using DanceMS.Helpers;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DanceMS.ViewModels
{
    public class EntryPageViewModel : BaseViewModel
    {
        public EntryPageViewModel()
        {
            Title = "Task List";
        }

        Command loginCmdFB;
        Command loginCmdGoogle;
        public Command LoginFacebook => loginCmdFB ?? (loginCmdFB = new Command(async () => await ExecuteLoginCommand(MobileServiceAuthenticationProvider.Facebook).ConfigureAwait(false)));
        public Command LoginGoogle => loginCmdGoogle ?? (loginCmdGoogle = new Command(async () => await ExecuteLoginCommand(MobileServiceAuthenticationProvider.Google).ConfigureAwait(false)));

        async Task ExecuteLoginCommand(MobileServiceAuthenticationProvider provider)
        {
            if (IsBusy)
                return;
            IsBusy = true;

            try
            {
                var cloudService = ServiceLocator.Instance.Resolve<ICloudService>();
                await cloudService.LoginAsync(provider);
                Application.Current.MainPage = new NavigationPage(new Pages.TaskList());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Login] Error = {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
