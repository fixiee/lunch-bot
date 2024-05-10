using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace lunch_bot;

public static class HtmlContentParser
{
    private static readonly HttpClient HttpClient = new();

    public static async Task<List<string>> FetchAndProcessRestaurants(IEnumerable<RestaurantData> restaurants, ILogger log)
    {
        var tasks = restaurants.Select(async data =>
        {
            HttpResponseMessage response = await HttpClient.GetAsync(data.Url);
            if (response.IsSuccessStatusCode)
            {
                string htmlContent = await response.Content.ReadAsStringAsync();
                return ParseMenu(htmlContent, data.Emoji);
            }
            else
            {
                log.LogWarning($"Failed to fetch data from {data.Url}");
                return null;
            }
        });

        var results = await Task.WhenAll(tasks);
        return results.Where(message => !string.IsNullOrEmpty(message)).ToList();
    }

    static string ParseMenu(string htmlContent, string emoji)
    {
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);
        var restName = htmlDocument.DocumentNode.SelectSingleNode($"//*[@id='menicka']/div/div[2]/div[1]/div[1]/h1");
        var restaurantName = restName?.InnerText.Trim() ?? "Unknown restaurant name";
        var today = DateTime.Now.ToString("d.M.yyyy");
        var todayMenuNode =
            htmlDocument.DocumentNode.SelectSingleNode($"//div[contains(text(), '{today}')]/following-sibling::ul");

        if (todayMenuNode != null)
        {
            var menuItems = todayMenuNode.SelectNodes(".//li")
                .Select(li => new
                {
                    Dish = li.SelectSingleNode(".//div[contains(@class, 'polozka')]")?.InnerText.Trim(),
                    Price = li.SelectSingleNode(".//div[contains(@class, 'cena')]")?.InnerText.Trim(),
                    Class = li.Attributes["class"]?.Value
                })
                .Where(item => item.Class != null && !item.Class.Contains("polevka"))
                .Where(item => !string.IsNullOrWhiteSpace(item.Dish) && !string.IsNullOrWhiteSpace(item.Price))
                .ToList();

            if (menuItems.Count > 0)
            {
                string message = $"{emoji} *{restaurantName}*\n" +
                                 string.Join("\n", menuItems.Select(m => $"> {m.Dish}, *{m.Price}*"));
                return message;
            }
        }
        
        var noMenuMessage = todayMenuNode?.SelectSingleNode(".//li[contains(@class, 'polevka')]")?.InnerText.Trim();
        return noMenuMessage != null
            ? $"{emoji} *{restaurantName}*\n> {noMenuMessage}"
            : $"{emoji} *{restaurantName}*\n> Today's menu is not available.";
    }
}