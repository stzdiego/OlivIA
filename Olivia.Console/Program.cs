using System.Net.Http.Json;
using Olivia.Shared.Dtos;

HttpClient client = new HttpClient()
{
    BaseAddress = new Uri("http://localhost:5146/api/ChatAssistance/"),
    Timeout = TimeSpan.FromMinutes(5)
};

var response = await client.GetAsync("NewChat");

if (response.IsSuccessStatusCode == false)
{
    return;
}

var idChat = await response.Content.ReadFromJsonAsync<IdDto>();

while (true)
{
    Console.Write("You: ");
    var message = Console.ReadLine();

    var messageResponse = await client.PostAsJsonAsync("NewMessage", new NewMessageDto
    {
        ChatId = idChat!.Id,
        Content = message!
    });

    var agentMessageDto = await messageResponse.Content.ReadFromJsonAsync<AgentMessageDto>();

    Console.WriteLine(agentMessageDto!.Content);
}

