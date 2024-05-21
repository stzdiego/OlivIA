using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Olivia.Shared.Dtos;

HttpResponseMessage messageResponse;

Console.WriteLine("Olivia Console");
Console.WriteLine("==============");
Console.Write("Si quiere iniciar el chat como doctor escriba 'D':");
var isDoctor = Console.ReadLine()!.ToUpper().Equals("D");

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

    if (isDoctor)
    {
        messageResponse = await client.PostAsJsonAsync("DoctorNewMessage", new DoctorNewMessageDto
        {
            ChatId = idChat!.Id,
            Content = message!,
            DoctorId = Guid.Parse("97268A44-FCC9-4A9D-8139-87AEA8E2CB95")
        });
    }
    else
    {
        messageResponse = await client.PostAsJsonAsync("PatientNewMessage", new PatientNewMessageDto
        {
            ChatId = idChat!.Id,
            Content = message!
        });
    }

    var agentMessageDto = await messageResponse.Content.ReadFromJsonAsync<AgentMessageDto>();

    Console.WriteLine(agentMessageDto!.Content);
}

