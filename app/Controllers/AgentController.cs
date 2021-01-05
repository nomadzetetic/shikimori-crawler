using Microsoft.AspNetCore.Mvc;
using Shikimori.App.Models;
using Shikimori.App.Services;

namespace Shikimori.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly IAgentService _agent;

        public AgentController(IAgentService agent)
        {
            _agent = agent;
        }

        [HttpGet("status")]
        public JsonResult Status() => new JsonResult(new AgentState { Working = _agent.Running, CurrentPage = _agent.CurrentPage });
    }
}
