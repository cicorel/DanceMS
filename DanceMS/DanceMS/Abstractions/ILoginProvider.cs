using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace DanceMS.Abstractions
{
    public interface ILoginProvider
    {
        Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
    }
}
