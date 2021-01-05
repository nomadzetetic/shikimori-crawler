using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shikimori.Agent;
using Shikimori.Store;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shikimori.App.Services
{
    public class AgentService : IAgentService
    {
        private bool _running = false;

        private readonly object _sync = new object();
        private readonly ILogger<AgentService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private CancellationTokenSource _cancellationTokenSource;

        public AgentService(IServiceScopeFactory scopeFactory, ILogger<AgentService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public string CurrentPage
        {
            get;
            private set;
        }

        private async Task ExecuteJob(string startPageUrl, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            CurrentPage = startPageUrl;

            using var scope = _scopeFactory.CreateScope();

            var loader = scope.ServiceProvider.GetService<ILoader>();
            var databaseStore = scope.ServiceProvider.GetService<IDatabaseStore>();

            var pageInfo = string.IsNullOrWhiteSpace(startPageUrl) ?
                await loader.ScanPageAsync() :
                await loader.ScanPageAsync(startPageUrl);

            CurrentPage = pageInfo.PageUrl;

            _logger.LogInformation("Loaded page url: {0}", pageInfo.PageUrl);
            await databaseStore.SaveOrUpdateNextPageUrlAsync(pageInfo.NextPageUrl);
            await databaseStore.SaveOrUpdateVideosAsync(pageInfo.VideosInfos);

            while (pageInfo.NextPageUrl != null)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                pageInfo = await loader.ScanPageAsync(pageInfo.NextPageUrl);
                CurrentPage = pageInfo.PageUrl;
                await databaseStore.SaveOrUpdateNextPageUrlAsync(pageInfo.NextPageUrl);
                await databaseStore.SaveOrUpdateVideosAsync(pageInfo.VideosInfos);
            }
        }

        public bool Running => _running;

        public async Task Start(string startPageUrl)
        {
            lock (_sync)
            {
                if (_running)
                    return;

                _running = true;
                _cancellationTokenSource = new CancellationTokenSource();
            }

            var task = Task.Run(() => ExecuteJob(startPageUrl, _cancellationTokenSource.Token), _cancellationTokenSource.Token);

            try
            {
                await Task.WhenAny(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                lock (_sync)
                {
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                    _running = false;
                }
            }
        }

        public void Stop()
        {
            lock (_sync)
            {
                if (_running)
                {
                    _cancellationTokenSource.Cancel();
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
