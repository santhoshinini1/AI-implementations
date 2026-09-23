using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

string SummarizeArticle(string articleText)
{
    if (string.IsNullOrWhiteSpace(articleText))
    {
        return "No article text was supplied.";
    }

    // This is a sample local summarization helper.
    var sentences = articleText
        .Split(['.', '!', '?'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .ToArray();

    var firstThree = sentences.Take(3).ToArray();
    return firstThree.Length == 0
        ? "Article is too short to summarize."
        : string.Join("; ", firstThree) + ".";
}

IConfigurationRoot config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string? model = config["ModelName"];
string? key = config["OpenAIKey"];

bool hasRealApiKey = !string.IsNullOrWhiteSpace(key)
    && !key.Contains("<", StringComparison.Ordinal)
    && !key.Contains("your-openai-api-key", StringComparison.OrdinalIgnoreCase);

Func<string, string, string> getWeather = (location, unit) =>
    $"The weather in {location} is 15 {unit} with periods of rain and drizzle.";

Func<string, string> summarizeArticle = articleText => SummarizeArticle(articleText);

var weatherTool = AIFunctionFactory.Create(
    getWeather,
    "get_current_weather",
    "Gets the current weather in a given location.");

var summaryTool = AIFunctionFactory.Create(
    summarizeArticle,
    "summarize_article",
    "Summarizes a supplied article into a short, readable summary.");

var article = "Artificial intelligence is changing the way software is built. It helps developers automate repetitive tasks and quickly analyze large amounts of information. Teams are now using AI to speed up coding, improve customer support, and make better decisions in business. The technology continues to evolve and is becoming a standard part of modern application development.";

if (!hasRealApiKey)
{
    Console.WriteLine("No valid OpenAI API key was found. Running in local demo mode without the API key.");
    Console.WriteLine();
    Console.WriteLine("Weather tool result:");
    Console.WriteLine(getWeather("Montreal", "C"));
    Console.WriteLine();
    Console.WriteLine("Article summary result:");
    Console.WriteLine(summarizeArticle(article));
    return;
}

IChatClient client =
    new ChatClientBuilder(new OpenAIClient(key).GetChatClient(model ?? "gpt-5").AsIChatClient())
    .UseFunctionInvocation()
    .Build();

var chatOptions = new ChatOptions
{
    Tools = [weatherTool, summaryTool]
};

List<ChatMessage> chatHistory =
[
    new(ChatRole.System, """
        You are a helpful AI assistant that can use local .NET tools when needed.
        Use the weather tool for weather questions and the article summarizer for article text.
        """)
];

chatHistory.Add(new ChatMessage(ChatRole.User,
    $"Please summarize this article for me: {article}"));

Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");

ChatResponse response = await client.GetResponseAsync(chatHistory, chatOptions);
Console.WriteLine($"Assistant >>> {response.Text}");
