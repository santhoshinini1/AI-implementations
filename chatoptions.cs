// Add a new plugin with a local .NET function
// that should be available to the AI model.
var chatOptions = new ChatOptions
{
    Tools = [AIFunctionFactory.Create((string location, string unit) =>
    {
        // Here you would call a weather API
        // to get the weather for the location.
        return "Periods of rain or drizzle, 15 C";
    },
    "get_current_weather",
    "Gets the current weather in a given location")]
};
