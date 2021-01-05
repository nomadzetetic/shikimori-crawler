using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shikimori.App.Services;
using Shikimori.Store;

namespace Shikimori.App.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IDatabaseStore _db;
        private readonly IAgentService _agentService;

        public IndexModel(IAgentService agentService, IDatabaseStore db)
        {
            _db = db;
            _agentService = agentService;
        }

        public bool Running => _agentService.Running;
        public string NextPageUrl { get; private set; }

        public async Task<PageResult> OnGetAsync()
        {
            NextPageUrl = await _db.GetNextPageUrlAsync();
            return Page();
        }

        public async Task<RedirectToPageResult> OnPostStartAsync()
        {
            NextPageUrl = await _db.GetNextPageUrlAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _agentService.Start(NextPageUrl);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            
            return RedirectToPage("Index");
        }

        public async Task<RedirectToPageResult> OnPostStopAsync()
        {
            _agentService.Stop();
            NextPageUrl = await _db.GetNextPageUrlAsync();
            return RedirectToPage("Index");
        }
    }
}
