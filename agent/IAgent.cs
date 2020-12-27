using Shikimori.Agent.Models;
using System.Threading.Tasks;

namespace Shikimori.Agent
{
    public interface IAgent
    {
        Task<PageInfo> ScanPageAsync(string startUrl = "https://shikimori.one/animes");
    }
}
