using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Moq;
using Olivia.AI.Plugins;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.Tests.Olivia.AI.Plugins;

public class PatientsManagerPluginTest
{
    private ServiceProvider serviceProvider;
    private Mock<PatientService> mockPatientService;

    public PatientsManagerPluginTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockDatabase = new Mock<IDatabase>();
        var mockLoggerPatientService = new Mock<ILogger<PatientService>>();
        var mockLoggerChatService = new Mock<ILogger<ChatService>>();
        var mockLoggerProgramationService = new Mock<ILogger<ProgramationService>>();
        var mockLoggerDoctorService = new Mock<ILogger<DoctorService>>();
        mockPatientService = new Mock<PatientService>(mockDatabase.Object, mockLoggerPatientService.Object);
        var mockChatService = new Mock<ChatService>(mockDatabase.Object, mockLoggerChatService.Object);
        var mockDoctorService = new Mock<DoctorService>(mockDatabase.Object, mockLoggerDoctorService.Object);
        var mockProgramationService = new Mock<ProgramationService>(mockDatabase.Object, mockLoggerProgramationService.Object);

        mockPatientService.Setup(x => x.Find(It.IsAny<Guid>())).ReturnsAsync(new Patient() { Name = "Mike", LastName = "Wazowski", Email = "MikeW@email.com", Phone = 123456789, Reason = "Reason" });
        mockDoctorService.Setup(x => x.Get()).ReturnsAsync(new List<Doctor>());
        mockDatabase.Setup(x => x.Find<Chat>(It.IsAny<Expression<Func<Chat, bool>>>())).ReturnsAsync(new Chat());
        mockDatabase.Setup(x => x.Find<Doctor>(It.IsAny<Expression<Func<Doctor, bool>>>())).ReturnsAsync(new Doctor() { Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "maik@email.com", Information = "Information", Speciality = "Speciality", Available = true, Phone = 123456789, Start = TimeSpan.FromHours(8), End = TimeSpan.FromHours(16) });

        serviceCollection.AddTransient(_ => mockDatabase.Object);
        serviceCollection.AddTransient(_ => mockLoggerPatientService.Object);
        serviceCollection.AddTransient(_ => mockLoggerChatService.Object);
        serviceCollection.AddTransient(_ => mockLoggerProgramationService.Object);
        serviceCollection.AddTransient(_ => mockLoggerDoctorService.Object);
        serviceCollection.AddTransient(_ => mockPatientService.Object);
        serviceCollection.AddTransient(_ => mockChatService.Object);
        serviceCollection.AddTransient(_ => mockDoctorService.Object);
        serviceCollection.AddTransient(_ => mockProgramationService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

}
