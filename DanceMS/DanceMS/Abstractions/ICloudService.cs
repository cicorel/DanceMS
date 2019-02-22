using DanceMS.Models;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace DanceMS.Abstractions
{
    public interface ICloudService
    {
        ICloudTable<T> GetTable<T>() where T : TableData;
        Task LoginAsync(MobileServiceAuthenticationProvider provider);
        Task<AppServiceIdentity> GetIdentityAsync();
    }
}
