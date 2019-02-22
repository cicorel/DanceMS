using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using DanceMS.Abstractions;
using DanceMS.iOS.Services;
using UIKit;
using Newtonsoft.Json.Linq;
using System.Runtime.Remoting.Contexts;
using Xamarin.Auth;
using System;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(iOSLoginProvider))]
namespace DanceMS.iOS.Services
{
    public class iOSLoginProvider : ILoginProvider
    {
        public Context Context { get; private set; }
        public AccountStore AccountStore { get; private set; }

        public void Init(Context context)
        {
            this.Context = context;
            AccountStore = AccountStore.Create();
        }

        /// <summary>
        ///  The first piece is to check to see if there is an existing token in the KeyStore. 
        ///  If there is, we check the expiry time and then set up the Azure Mobile Apps client with the username and token from the KeyStore.
        ///  If there isn't, we do the normal authentication process. 
        ///  If the authentication process is successful, we reach the second piece, which is to store the token within the KeyStore.
        ///  If there is an existing entry, it will be overwritten. 
        ///  Finally, there is a method called IsTokenExpired() whose only job is to check to see if a token is expired or not. 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {

            var accounts = AccountStore.FindAccountsForService("dancems");
            if (accounts != null)
            {
                foreach (var userAccount in accounts)
                {
                    string token;

                    if (userAccount.Properties.TryGetValue("token", out token))
                    {
                        if (!IsTokenExpired(token))
                        {
                            client.CurrentUser = new MobileServiceUser(userAccount.Username);
                            client.CurrentUser.MobileServiceAuthenticationToken = token;
                            return;
                        }
                    }
                }
            }

            await client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController, MobileServiceAuthenticationProvider.Google, Constants.Constants.UrlScheme);


            var account = new Account(client.CurrentUser.UserId);
            account.Properties.Add("token", client.CurrentUser.MobileServiceAuthenticationToken);
            AccountStore.Save(account, "dancems");
        }

        private bool IsTokenExpired(string token)
        {
            //Get just the JWT part of the token
            var jwt = token.Split(new char[] { '.', '/' })[1];

            //Unde the URL encoding
            jwt = jwt.Replace('-', '+').Replace('_', '/');
            switch (jwt.Length % 4)
            {
                case 0: break;
                case 2: jwt += "=="; break;
                case 3: jwt += "="; break;
                default:
                    throw new ArgumentException("The token is not a valid Base64 string.");
            }

            // Convert to a JSON String
            var bytes = Convert.FromBase64String(jwt);
            string jsonString = UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            // Parse as JSON object and get the exp field value,
            // which is the expiration date as a JavaScript primative date.
            JObject jsonObj = JObject.Parse(jsonString);
            var exp = Convert.ToDouble(jsonObj["exp"].ToString());

            // Calculate the expiration by adding the exp value (in seconds) to the
            // base date of 1/1/1970.
            DateTime minTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var expire = minTime.AddSeconds(exp);
            return (expire < DateTime.UtcNow);
        }
    }
}