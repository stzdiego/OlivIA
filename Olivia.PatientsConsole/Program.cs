using System.Net.Http.Json;
using Olivia.Shared.Dtos;

HttpClient client = new HttpClient()
{
    BaseAddress = new Uri("http://localhost:5146/api/PatientsAsistence/"),
    Timeout = TimeSpan.FromMinutes(5)
};

var response = await client.PostAsJsonAsync("Initialize", new { });

if (response.IsSuccessStatusCode)
{
    Console.WriteLine("Olivia initialized");
}
else
{
    Console.WriteLine("Olivia failed to initialize");
    return;
}

var agentMessageDto = await response.Content.ReadFromJsonAsync<AgentMessageDto>();

Console.WriteLine($"Chat created with id: {agentMessageDto!.Id}");
Console.WriteLine(agentMessageDto!.Content);

while (true)
{
    Console.Write("You: ");
    var message = Console.ReadLine();

    var messageResponse = await client.PostAsJsonAsync("NewMessage", new NewMessageDto
    {
        ChatId = agentMessageDto.Id,
        Content = message!
    });

    agentMessageDto = await messageResponse.Content.ReadFromJsonAsync<AgentMessageDto>();

    Console.WriteLine(agentMessageDto!.Content);
}

