using System;
using System.Threading.Tasks;

namespace Shikimori.App.Services
{
    public interface IAgentService : IDisposable
    {
        Task Start(string startPageUrl);
        void Stop();
        bool Running { get; }
        string CurrentPage { get; }
    }
}
