using HtmlAgilityPack;
using Shikimori.Agent.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shikimori.Agent
{
    public class Loader : ILoader
    {
        private readonly HtmlWeb _web = new HtmlWeb();
        private readonly IParser _parser = new Parser();

        private async Task<HtmlDocument> LoadUrlAsync(string url)
        {
            Console.WriteLine("Loading url: {0}", url);
            var doc = await _web.LoadFromWebAsync(url);
            await Task.Delay(1000); // Special delay to be slower othewise might work nginx DDOS protection
            return doc;
        }

        private async Task<VideoInfo> LoadAnimeDetails(string url)
        {
            var html = await LoadUrlAsync(url);
            var videoPageInfo = _parser.ParseVideoPage(html.DocumentNode);
            return videoPageInfo;
        }

        public async Task<PageInfo> ScanPageAsync(string pageUrl = "https://shikimori.one/animes")
        {
            var html = await LoadUrlAsync(pageUrl);
            var videosInfos = new List<VideoInfo>();
            var urls = _parser.ParseVideosUrls(html.DocumentNode);

            foreach (var url in urls)
            {
                var videoInfo = await LoadAnimeDetails(url);
                videosInfos.Add(videoInfo);
            }

            var nextPageUrl = _parser.ParseNextPageUrl(html.DocumentNode);

            var pageInfo = new PageInfo
            {
                PageUrl = pageUrl,
                NextPageUrl = nextPageUrl,
                VideosInfos = videosInfos
            };

            return pageInfo;
        }
    }
}
