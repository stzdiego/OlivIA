using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Olivia.Data;
using Olivia.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Olivia.Shared.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Olivia.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Olivia.Shared.Entities;
using Olivia.Services;
using Xunit.Sdk;

namespace Olivia.Tests.Olivia.Api.Controllers;

public class DoctorsControllerTest
{
    private ServiceProvider serviceProvider;
    private Mock<IDoctorService> _mockDoctorService;

    public DoctorsControllerTest()
    {
        var serviceCollection = new ServiceCollection();

        var mockLoggerDoctorAsistenceController = new Mock<ILogger<DoctorsController>>();
        _mockDoctorService = new Mock<IDoctorService>();

        serviceCollection.AddTransient(_ => mockLoggerDoctorAsistenceController.Object);
        serviceCollection.AddTransient(_ => _mockDoctorService.Object);

        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [Fact]
    public async Task Get_Should_Return_Ok()
    {
        // Arrange
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, serviceProvider.GetService<IDoctorService>()!);

        // Act
        var result = await DoctorsController.Get();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_Should_Return_InternalServerError()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(x => x.Get()).Throws(new Exception("Error getting doctors"));
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, mockDoctorService.Object);

        // Act
        var result = await DoctorsController.Get();

        // Assert
        Assert.IsType<StatusCodeResult>(result);
    }

    [Fact]
    public async Task Get_With_Guid_Should_Return_Ok()
    {
        // Arrange
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, serviceProvider.GetService<IDoctorService>()!);

        // Act
        var result = await DoctorsController.Get(Guid.NewGuid());

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Get_With_Guid_Should_Return_InternalServerError()
    {
        // Arrange
        var mockDoctorService = new Mock<IDoctorService>();
        mockDoctorService.Setup(x => x.Find(It.IsAny<Guid>())).Throws(new Exception("Error getting doctor"));
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, mockDoctorService.Object);

        // Act
        var result = await DoctorsController.Get(Guid.NewGuid());

        // Assert
        Assert.IsType<StatusCodeResult>(result);
    }

    [Fact]
    public async Task Post_Should_Return_Ok()
    {
        // Arrange
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, serviceProvider.GetService<IDoctorService>()!);
        var doctorDto = new DoctorDto() { Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "email@email.com", Phone = 123456, Speciality = "Speciality", Information = "Information", Start = new TimeSpan(12, 0, 0), End = new TimeSpan(18, 0, 0) };

        // Act
        var result = await DoctorsController.Post(doctorDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    /*
    [Fact]
    public async void Post_Should_Return_InternalServerError()
    {
        // Arrange
        _mockDoctorService.Setup(x => x.Create(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>())).ThrowsAsync(new Exception("Error creating doctor"));
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, serviceProvider.GetService<IDoctorService>()!);
        var doctorDto = new DoctorDto() { Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "email", Phone = 123456, Speciality = "Speciality", Information = "Information", Start = new TimeSpan(12, 0 ,0), End = new TimeSpan(18, 0, 0)};

        // Act
        var result = await DoctorsController.Post(doctorDto);

        // Assert
        Assert.IsType<StatusCodeResult>(result);
    }
    */

    [Fact]
    public async Task Put_Should_Return_Ok()
    {
        // Arrange
        _mockDoctorService.Setup(x => x.Update(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, serviceProvider.GetService<IDoctorService>()!);
        var doctorDto = new DoctorDto() { Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "email", Phone = 123456, Speciality = "Speciality", Information = "Information", Start = new TimeSpan(12, 0, 0), End = new TimeSpan(18, 0, 0) };

        // Act
        var result = await DoctorsController.Put(new Guid(), doctorDto);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    /*
    [Fact]
    public async void Put_Should_Return_InternalServerError()
    {
        // Arrange
        _mockDoctorService.Setup(x => x.Update(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Error updating doctor"));
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, serviceProvider.GetService<IDoctorService>()!);
        var doctorDto = new DoctorDto() { Identification = 123456, Name = "Mike", LastName = "Wazowski", Email = "email", Phone = 123456, Speciality = "Speciality", Information = "Information", Start = new TimeSpan(12, 0 ,0), End = new TimeSpan(18, 0, 0)};

        // Act
        var result = await DoctorsController.Put(new Guid(), doctorDto);

        // Assert
        Assert.IsType<StatusCodeResult>(result);
    }
    */

    [Fact]
    public async Task Delete_Should_Return_Ok()
    {
        // Arrange
        _mockDoctorService.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(Task.FromResult(true));
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, serviceProvider.GetService<IDoctorService>()!);

        // Act
        var result = await DoctorsController.Delete(new Guid());

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Delete_Should_Return_InternalServerError()
    {
        // Arrange
        _mockDoctorService.Setup(x => x.Delete(It.IsAny<Guid>())).ThrowsAsync(new Exception("Error deleting doctor"));
        var DoctorsController = new DoctorsController(serviceProvider.GetService<ILogger<DoctorsController>>()!, serviceProvider.GetService<IDoctorService>()!);

        // Act
        var result = await DoctorsController.Delete(new Guid());

        // Assert
        Assert.IsType<StatusCodeResult>(result);
    }
}
