using HtmlAgilityPack;
using Shikimori.Agent.Models;
using System.Collections.Generic;

namespace Shikimori.Agent
{
    public interface IParser
    {
        VideoInfo ParseVideoPage(HtmlNode node);
        string ParseNextPageUrl(HtmlNode node);
        List<string> ParseVideosUrls(HtmlNode node);
    }
}
