using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System.Collections.Generic;
using System.Linq;

namespace Shikimori.Agent.Parsers
{
    public class VideoListParser
    {
        public static string GetNextPageUrl(HtmlDocument html)
        {
            var node = html.DocumentNode.QuerySelector(".link.link-next");
            if (node != null && !node.HasClass("disabled"))
            {
                return node.GetAttributeValue("href", null);
            }

            return null;
        }

        public static List<string> GetVideosUrls(HtmlDocument html)
        {
            var links = html.DocumentNode.QuerySelectorAll("[data-dynamic=\"cutted_covers\"] a.cover");
            var urls = links.Select(link => link.GetAttributeValue("href", null)).Where(url => !string.IsNullOrWhiteSpace(url)).ToList();
            return urls;
        }
    }
}
