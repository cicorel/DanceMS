using DanceMS.Abstractions;
using DanceMS.Helpers;
using DanceMS.Pages;
using DanceMS.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DanceMS
{
    public partial class App : Application
    {

        public static ICloudService CloudService { get; set; }

        public App()
        {
            InitializeComponent();
            CloudService = new AzureCloudService();
            ServiceLocator.Instance.Add<ICloudService, AzureCloudService>();
            MainPage = new NavigationPage(new Pages.EntryPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
