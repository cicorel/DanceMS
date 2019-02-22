using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using DanceMS.Abstractions;
using DanceMS.UWP.Services;
using System;
using Windows.Security.Credentials;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

[assembly: Xamarin.Forms.Dependency(typeof(UWPLoginProvider))]
namespace DanceMS.UWP.Services
{
    public class UWPLoginProvider : ILoginProvider
    {

        public PasswordVault PasswordVault { get; private set; }
        private MobileServiceClient LoginContinuation = null;
        private PasswordCredential userAccount;

        public UWPLoginProvider()
        {
            PasswordVault = new PasswordVault();
        }

        public async Task LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {

            try
            {
                userAccount = PasswordVault.FindAllByResource("dancems").FirstOrDefault();
            } catch (Exception e)
            {
                userAccount = null;
            }
            

            if (userAccount != null)
            {
                var token = PasswordVault.Retrieve("dancems", userAccount.UserName).Password;
                if (token != null && token.Length > 0 && !IsTokenExpired(token))
                {
                    client.CurrentUser = new MobileServiceUser(userAccount.UserName);
                    client.CurrentUser.MobileServiceAuthenticationToken = token;
                    return;
                }
            }

            try
            {
                var concreteClient = client as MobileServiceClient; //extensionmethod we need is not on interface but on implementation class
                LoginContinuation = concreteClient;
                await client.LoginAsync(provider, Constants.Constants.UrlScheme);
                PasswordVault.Add(new PasswordCredential("dancems", client.CurrentUser.UserId, client.CurrentUser.MobileServiceAuthenticationToken));
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

        public void RemoveTokenFromSecureStore()
        {
            try
            {
                // Check if the token is available within the password vault
                var acct = PasswordVault.FindAllByResource("dancems").FirstOrDefault();
                if (acct != null)
                {
                    PasswordVault.Remove(acct);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving existing token: {ex.Message}");
            }
        }

        public void ContinueLogin(Uri uri)
        {
            if (LoginContinuation != null)
            {
                LoginContinuation.ResumeWithURL(uri);
                LoginContinuation = null;
            }
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