using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace lunch_bot;

public static class Utilities
{
    public static List<RestaurantData> GetRestaurants(ILogger log)
    {
        try
        {
            var restaurantsJson = Environment.GetEnvironmentVariable("URLS");
            return JsonConvert.DeserializeObject<List<RestaurantData>>(restaurantsJson) ?? new List<RestaurantData>();
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to load or parse restaurants: {ex.Message}");
            return new List<RestaurantData>();
        }
    }
}