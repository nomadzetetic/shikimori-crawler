using System.Collections.Generic;

namespace shikimori.agent.models
{
    public class VideoPageInfo
    {
        public MultilangValue Title { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// Duration in seconds
        /// </summary>
        public int Duration { get; set; }

        public string Description { get; set; }

        public List<MultilangValue> Genres { get; set; }
    }
}
