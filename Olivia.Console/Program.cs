using System.Net.Http.Json;
using Olivia.Shared.Dtos;

HttpClient client = new HttpClient()
{
    BaseAddress = new Uri("http://localhost:5146/api/Asistence/")
};

var response = await client.PostAsJsonAsync("Initialize", new {});

if(response.IsSuccessStatusCode)
{
    Console.WriteLine("Olivia initialized");
}
else
{
    Console.WriteLine("Olivia failed to initialize");
    return;
}

var chatId = await response.Content.ReadFromJsonAsync<Guid>();

Console.WriteLine($"Chat created with id: {chatId}");
while(true)
{
    Console.Write("You: ");
    var message = Console.ReadLine();

    var messageResponse = await client.PostAsJsonAsync("NewMessage", new NewMessageDto
    {
        ChatId = chatId,
        Content = message!
    });

    var agentMessage = await messageResponse.Content.ReadFromJsonAsync<AgentMessageDto>();

    Console.WriteLine($"Agent: {agentMessage!.Content}");
}

