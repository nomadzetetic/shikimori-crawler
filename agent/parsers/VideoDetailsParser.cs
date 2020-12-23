using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Shikimori.Agent.Models;
using System.Collections.Generic;
using System.Linq;

namespace Shikimori.Agent.Parsers
{
    public static class VideoDetailsParser
    {
        public static VideoPageInfo FromHtmlNode(HtmlNode node)
        {
            var url = GetUrl(node);
            var title = GetTitle(node);
            var type = GetType(node);
            var genres = GetGenres(node);
            var duration = GetAnimeDuration(node);
            var description = GetDescription(node);

            var animeDetails = new VideoPageInfo
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

        private static List<MultilangValue> GetGenres(HtmlNode node)
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

        private static string GetType(HtmlNode node) =>
            node.QuerySelector("meta[property=\"og:type\"]").GetAttributeValue("content", string.Empty);

        private static int GetAnimeDuration(HtmlNode node)
        {
            var textValue = node.QuerySelector("meta[property=\"video:duration\"]").GetAttributeValue("content", string.Empty);
            if (int.TryParse(textValue, out int duration))
            {
                return duration;
            }

            return 0;
        }

        private static string GetUrl(HtmlNode node) =>
            node.QuerySelector("meta[property=\"og:url\"]").GetAttributeValue("content", string.Empty);

        private static string GetDescription(HtmlNode node) =>
            node.QuerySelector(".russian [itemprop=\"description\"]").InnerText.Trim().Replace("\n", " ").Replace("  ", " ");
    }
}
