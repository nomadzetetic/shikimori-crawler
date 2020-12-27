using System.Collections.Generic;

namespace Shikimori.Agent.Models
{
    public class PageInfo
    {
        public string NextPageUrl { get; set; }
        public List<VideoInfo> VideosInfos { get; set; }
    }
}
