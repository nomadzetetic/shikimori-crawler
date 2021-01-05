using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Shikimori.Agent.Models;
using System.Collections.Generic;
using System.Linq;

namespace Shikimori.Agent
{
    public class Parser : IParser
    {
        private static MultilangValue GetTitle(HtmlNode node)
        {
            var titleRaw = node.QuerySelector("h1").InnerText.Trim() ?? string.Empty;
            if (titleRaw.Contains("/"))
            {
                var nameParts = titleRaw.Split("/");
                return new MultilangValue
                {
                    Ru = nameParts[0].Trim(),
                    Eng = nameParts[1].Trim()
                };
            }

            return new MultilangValue
            {
                Ru = titleRaw,
                Eng = null
            };
        }

        private static Dictionary<string, string> GetGenres(HtmlNode node)
        {
            var genres = node.QuerySelectorAll(".c-about .c-info-left [itemprop=\"genre\"]")
                .Aggregate(new Dictionary<string, string>(), (acc, genreNode) =>
                {
                    var key = genreNode.QuerySelector(".genre-en")?.InnerText?.Trim() ?? null;
                    var value = genreNode.QuerySelector(".genre-ru")?.InnerText?.Trim() ?? null;
                    if (key != null && !acc.ContainsKey(key))
                    {
                        acc.Add(key, value);
                    }
                    return acc;
                });

            return genres;
        }

        private static string GetType(HtmlNode node) =>
            node.QuerySelector("meta[property=\"og:type\"]")?.GetAttributeValue("content", string.Empty) ?? string.Empty;

        private static int GetAnimeDuration(HtmlNode node)
        {
            var textValue = node.QuerySelector("meta[property=\"video:duration\"]")?.GetAttributeValue("content", string.Empty) ?? "0";
            if (int.TryParse(textValue, out int duration))
            {
                return duration;
            }

            return 0;
        }

        private static string GetUrl(HtmlNode node) =>
            node.QuerySelector("meta[property=\"og:url\"]")?.GetAttributeValue("content", string.Empty) ?? string.Empty;

        private static string GetDescription(HtmlNode node) =>
            node.QuerySelector(".russian [itemprop=\"description\"]")?.InnerText?.Trim()?.Replace("\n", " ")?.Replace("  ", " ") ?? string.Empty;

        private static string GetImageUrl(HtmlNode node) =>
            node.QuerySelector("meta[property=\"og:image\"]")?.GetAttributeValue("content", string.Empty) ?? string.Empty;

        public VideoInfo ParseVideoPage(HtmlNode node)
        {
            var url = GetUrl(node);
            var title = GetTitle(node);
            var type = GetType(node);
            var genres = GetGenres(node);
            var duration = GetAnimeDuration(node);
            var description = GetDescription(node);
            var imageUrl = GetImageUrl(node);

            var animeDetails = new VideoInfo
            {
                Url = url,
                Title = title,
                Type = type,
                Genres = genres,
                Duration = duration,
                Description = description,
                ImageUrl = imageUrl
            };

            return animeDetails;
        }

        public string ParseNextPageUrl(HtmlNode node)
        {
            var nextLink = node.QuerySelector(".link.link-next");
            if (nextLink != null && !nextLink.HasClass("disabled"))
            {
                return nextLink.GetAttributeValue("href", null);
            }

            return null;
        }

        public List<string> ParseVideosUrls(HtmlNode node)
        {
            var links = node.QuerySelectorAll("[data-dynamic=\"cutted_covers\"] .cover");
            var urls = links.Select(link => link.GetAttributeValue("href", null)).Where(url => !string.IsNullOrWhiteSpace(url)).ToList();

            if (urls.Count == 0)
                urls = links.Select(link => link.GetAttributeValue("data-href", null)).Where(url => !string.IsNullOrWhiteSpace(url)).ToList();

            return urls;
        }
    }
}
