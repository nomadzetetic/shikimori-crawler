using System.Collections.Generic;

namespace Shikimori.Data.Models
{
    public class Video
    {
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string TitleRus { get; set; }
        public string TitleEng { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public IDictionary<string, string> Genres { get; set; }
    }
}
