using HtmlAgilityPack;
using shikimori.agent.models;
using shikimori.agent.parsers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace shikimori.agent
{
    public class Agent
    {
        private readonly HtmlWeb web;
        private const string animesFirstPageUrl = "https://shikimori.one/animes";

        public Agent(HtmlWeb web)
        {
            this.web = web;
        }

        private async Task<HtmlDocument> LoadAnimesFirstPageAsync() => await LoadUrlAsync(animesFirstPageUrl);

        private async Task<HtmlDocument> LoadUrlAsync(string url)
        {
            Console.WriteLine("Loading url: {0}", url);
            var doc = await web.LoadFromWebAsync(url);
            await Task.Delay(300); // Special delay to be slower othewise might work nginx DDOS protection
            return doc;
        }

        private async Task<VideoPageInfo> GetAnimeDetails(string url)
        {
            var html = await LoadUrlAsync(url);
            var videoPageInfo = VideoDetailsParser.FromHtmlNode(html.DocumentNode);
            return videoPageInfo;
        }

        private async Task StoreResults(List<VideoPageInfo> itemsToSave)
        {

        }

        public async Task ScanAsync()
        {
            var html = await LoadAnimesFirstPageAsync();
            var nextPageUrl = VideoListParser.GetNextPageUrl(html);

            while (nextPageUrl != null)
            {
                var itemsToSave = new List<VideoPageInfo>();
                var urls = VideoListParser.GetVideosUrls(html);
                foreach (var url in urls)
                {
                    var animeDetails = await GetAnimeDetails(url);
                    itemsToSave.Add(animeDetails);
                }

                await StoreResults(itemsToSave);

                html = await LoadUrlAsync(nextPageUrl);
                nextPageUrl = null; // VideoListPageParser.GetNextPageUrl(html);
            }
        }
    }
}
