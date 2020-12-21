using Microsoft.EntityFrameworkCore;

namespace shikimori.agent
{
    public class AgentDbContext : DbContext
    {
        public AgentDbContext(DbContextOptions<AgentDbContext> options) : base(options)
        {
        }
    }
}
