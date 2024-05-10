using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace lunch_bot;

public static class SlackClient
{
    private static readonly HttpClient HttpClient = new ();

    public static async Task SendMessageToSlack(List<string> messages)
    {
        var webhookUrl = Environment.GetEnvironmentVariable("SLACK_WEBHOOK_URL");
        StringBuilder currentMessage = new StringBuilder();

        foreach (var message in messages)
        {
            // Check if adding the next message exceeds the Slack character limit
            if (currentMessage.Length + message.Length + 2 > 3000) // +2 for new line characters
            {
                // Send the current batch of messages
                await PostMessageToSlack(webhookUrl, currentMessage.ToString());
                currentMessage.Clear(); // Clear the current message
            }

            // Add the message to the current batch
            if (currentMessage.Length > 0)
                currentMessage.Append("\n\n"); // Add a separator between messages
            currentMessage.Append(message);
        }

        // Send any remaining messages
        if (currentMessage.Length > 0)
        {
            await PostMessageToSlack(webhookUrl, currentMessage.ToString());
        }
    }

    static async Task PostMessageToSlack(string webhookUrl, string message)
    {
        var payload = JsonSerializer.Serialize(new
        {
            blocks = new[]
            {
                new
                {
                    type = "section",
                    text = new
                    {
                        type = "mrkdwn",
                        text = message
                    }
                }
            }
        });
        var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync(webhookUrl, content);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(
                $"Failed to send message to Slack. Status code: {response.StatusCode}. Response: {responseContent}");
        }
    }
}