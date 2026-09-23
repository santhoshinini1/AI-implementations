using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string? model = config["ModelName"];
string? key = config["OpenAIKey"];

IChatClient client =
    new ChatClientBuilder(new OpenAIClient(key).GetChatClient(model ?? "gpt-5").AsIChatClient())
    .UseFunctionInvocation()
    .Build();
