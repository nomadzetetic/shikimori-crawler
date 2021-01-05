using System.Collections.Generic;

namespace Shikimori.Agent.Models
{
    public class VideoInfo
    {
        public MultilangValue Title { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }
        public string ImageUrl { get; set; }

        /// <summary>
        /// Duration in seconds
        /// </summary>
        public int Duration { get; set; }

        public string Description { get; set; }

        public Dictionary<string, string> Genres { get; set; }
    }
}
