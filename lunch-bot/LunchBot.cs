using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace lunch_bot;

public static class LunchBot
{
    [FunctionName("lunch_bot")]
    public static async Task RunAsync([TimerTrigger("0 0 7 * * 1-5")] TimerInfo myTimer, ILogger log)
    {
        Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var restaurantData = Utilities.GetRestaurants(log);
        
        var messages = await HtmlContentParser.FetchAndProcessRestaurants(restaurantData, log);

        if (messages.Any())
        {
            await SlackClient.SendMessageToSlack(messages);
        }
    }
}