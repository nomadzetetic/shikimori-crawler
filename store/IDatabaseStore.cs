using Shikimori.Agent.Models;
using Shikimori.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shikimori.Store
{
    public interface IDatabaseStore
    {
        Task SaveOrUpdateVideosAsync(List<VideoInfo> videosInfo);
        Task<string> GetNextPageUrlAsync();
        Task<Setting> SaveOrUpdateNextPageUrlAsync(string value);
    }
}
