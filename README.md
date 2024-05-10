# Lunch Bot Azure Function

## Overview
Lunch Bot is an Azure Function designed to fetch daily lunch menus from a specified list of restaurants and send this information to a Slack channel at a designated time every weekday. This function aims to automate the process of gathering and distributing daily lunch options to help team members make informed lunch choices quickly and efficiently.

## Features
- Fetches daily menus from multiple restaurant websites.
- Parses HTML content to extract relevant menu information.
- Sends a formatted message to a Slack channel at 8:00 AM CEST every weekday.
- Handles changes in daylight saving time to maintain correct scheduling.
- Configurable through environment variables for easy adjustments.

## Technology Stack
- **Azure Functions**: Serverless computing service to run the function.
- **C#**: Programming language used for implementing the function.
- **HtmlAgilityPack**: Library for HTML parsing.
- **Newtonsoft.Json**: Library for JSON serialization/deserialization.
- **System.Text.Json**: Alternative JSON handling.
- **Microsoft.Extensions.Logging**: For logging support.

## Prerequisites
Before you deploy and run this function, you will need:
- An Azure account with an active subscription.
- Azure Function App setup.
- Slack workspace with a configured webhook URL.

## Setup and Local Development
1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/lunch-bot-function.git
   cd lunch-bot-function
   ```

2. **Configure local settings:**
   Edit the `local.settings.json` file to include your local development settings, such as:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet",
       "SLACK_WEBHOOK_URL": "your_slack_webhook_url_here",
       "URLS": "your_encoded_restaurant_urls_json_here"
     }
   }
   ```

3. **Run the function locally:**
   Ensure you have the Azure Functions Core Tools installed. Add Azurit for local storage simulator. Run the following command in the terminal:
   ```bash
   func start
   ```

## Usage
Once deployed, the function will automatically trigger every weekday at 8:00 AM CEST. It will fetch menus from the configured URLs, parse them, and post the results to the specified Slack channel.

## License
Distributed under the MIT License. See `LICENSE` for more information.
