using System.Text.RegularExpressions;

namespace OpenLaunch.Utils;

public static class HtmlUtils
{
    public static string ConvertHtmlToPlainText(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        // Decode HTML entities (e.g., &nbsp;, &lt;)
        html = System.Net.WebUtility.HtmlDecode(html);

        // Replace <br> and <div> with newlines
        html = Regex.Replace(html, @"<br\s*/?>|<div.*?>", "\n", RegexOptions.IgnoreCase);

        // Replace <a href="...">...</a> with "link (URL)"
        html = Regex.Replace(html, @"<a\s+(?:[^>]*?\s+)?href=([""'])(.*?)\1.*?>(.*?)</a>", match =>
        {
            var linkText = match.Groups[3].Value; // The text inside the link
            var linkUrl = match.Groups[2].Value;  // The URL
            return $"{linkText} ({linkUrl})";
        }, RegexOptions.IgnoreCase);

        // Remove all other tags
        html = Regex.Replace(html, @"<[^>]+>", string.Empty);

        // Collapse multiple spaces/newlines into one
        html = Regex.Replace(html, @"\s{2,}", " ");
        html = Regex.Replace(html, @"\n{2,}", "\n");

        return html.Trim();
    }
}