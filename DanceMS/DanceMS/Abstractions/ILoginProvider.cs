using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace DanceMS.Abstractions
{
    public interface ILoginProvider
    {
        Task LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
    }
}
