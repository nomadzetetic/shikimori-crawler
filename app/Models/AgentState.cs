namespace Shikimori.App.Models
{
    public record AgentState
    {
        public bool Working { get; init; }
        public string CurrentPage { get; init; }
    }
}
