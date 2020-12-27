using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Shikimori.Agent.Models;
using System.Collections.Generic;
using System.Linq;

namespace Shikimori.Agent
{
    public class Parser : IParser
    {
        private MultilangValue GetTitle(HtmlNode node)
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

        private List<MultilangValue> GetGenres(HtmlNode node)
        {
            var genres = node.QuerySelectorAll(".c-about .c-info-left [itemprop=\"genre\"]").Select(node =>
            {
                var eng = node.QuerySelector(".genre-en").InnerText.Trim();
                var ru = node.QuerySelector(".genre-ru").InnerText.Trim();

                return new MultilangValue
                {
                    Eng = eng,
                    Ru = ru
                };

            }).ToList();

            return genres;
        }

        private string GetType(HtmlNode node) =>
            node.QuerySelector("meta[property=\"og:type\"]").GetAttributeValue("content", string.Empty);

        private int GetAnimeDuration(HtmlNode node)
        {
            var textValue = node.QuerySelector("meta[property=\"video:duration\"]").GetAttributeValue("content", string.Empty);
            if (int.TryParse(textValue, out int duration))
            {
                return duration;
            }

            return 0;
        }

        private string GetUrl(HtmlNode node) =>
            node.QuerySelector("meta[property=\"og:url\"]").GetAttributeValue("content", string.Empty);

        private string GetDescription(HtmlNode node) =>
            node.QuerySelector(".russian [itemprop=\"description\"]").InnerText.Trim().Replace("\n", " ").Replace("  ", " ");

        public VideoInfo ParseVideoPage(HtmlNode node)
        {
            var url = GetUrl(node);
            var title = GetTitle(node);
            var type = GetType(node);
            var genres = GetGenres(node);
            var duration = GetAnimeDuration(node);
            var description = GetDescription(node);

            var animeDetails = new VideoInfo
            {
                Url = url,
                Title = title,
                Type = type,
                Genres = genres,
                Duration = duration,
                Description = description
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
            var links = node.QuerySelectorAll("[data-dynamic=\"cutted_covers\"] a.cover");
            var urls = links.Select(link => link.GetAttributeValue("href", null)).Where(url => !string.IsNullOrWhiteSpace(url)).ToList();
            return urls;
        }
    }
}
