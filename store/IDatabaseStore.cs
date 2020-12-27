using Shikimori.Agent.Models;
using Shikimori.Data.Models;
using System.Threading.Tasks;

namespace Shikimori.Store
{
    public interface IDatabaseStore
    {
        Task<Video> SaveOrUpdateVideoAsync(VideoInfo videoPageInfo);
        Task<Setting> SaveOrUpdateSettingAsync(string key, string value);
        Task<Setting> GetSettingAsync(string key);
    }
}
