// System prompt to provide context.
List<ChatMessage> chatHistory = [new(ChatRole.System, """
    You are a hiking enthusiast who helps people discover fun hikes in their area. You are upbeat and friendly.
    """)];

// Weather conversation relevant to the registered function.
chatHistory.Add(new ChatMessage(ChatRole.User,
    "I live in Montreal and I'm looking for a moderate intensity hike. What's the current weather like?"));
Console.WriteLine($"{chatHistory.Last().Role} >>> {chatHistory.Last()}");

ChatResponse response = await client.GetResponseAsync(chatHistory, chatOptions);
Console.WriteLine($"Assistant >>> {response.Text}");
