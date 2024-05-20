// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
#pragma warning disable SA1116 // SplitParametersMustStartOnLineAfterDeclaration
#pragma warning disable SA1201 // ElementsMustAppearInTheCorrectOrder

namespace Olivia.AI.Plugins;

using System.ComponentModel;
using System.Text;
using JetBrains.Annotations;
using Microsoft.SemanticKernel;
using Olivia.Shared.Dtos;
using Olivia.Shared.Entities;
using Olivia.Shared.Enums;
using Olivia.Shared.Interfaces;

/// <summary>
/// Represents a plugin that manages the patient.
/// </summary>
public class PatientManagerPlugin : IPlugin
{
    /// <summary>
    /// Gets or sets the patient.
    /// </summary>
    public Patient? Patient { get; set; }

    /// <summary>
    /// Gets or sets the doctors.
    /// </summary>
    public IEnumerable<Doctor>? Doctors { get; set; }

    private readonly IChatService chatService;
    private readonly IPatientService patientService;
    private readonly IDoctorService doctorService;
    private readonly IProgramationService programationService;
    private readonly IMailService mailService;
    private IList<DoctorInfoDto>? doctors;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientManagerPlugin"/> class.
    /// </summary>
    /// <param name="chatService">Chat service.</param>
    /// <param name="patientService">Patient service.</param>
    /// <param name="doctorService">Doctor service.</param>
    /// <param name="programationService">Programation service.</param>
    /// <param name="mailService">Mail service.</param>
    public PatientManagerPlugin(IChatService chatService, IPatientService patientService, IDoctorService doctorService, IProgramationService programationService, IMailService mailService)
    {
        this.chatService = chatService;
        this.patientService = patientService;
        this.doctorService = doctorService;
        this.programationService = programationService;
        this.mailService = mailService;
    }

    /// <summary>
    /// Registers a patient in the system.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="chatId">Chat identifier.</param>
    /// <param name="identification">Identification.</param>
    /// <param name="name">Name.</param>
    /// <param name="lastname">Lastname.</param>
    /// <param name="email">Email.</param>
    /// <param name="phone">Phone.</param>
    /// <param name="reason">Reason.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [KernelFunction("RegisterPatient")]
    [Description("Register a patient in the system.")]
    public async Task RegisterPatientAsync(
        Kernel kernel,
        [Description("Chat identifier")] string chatId,
        [Description("Identificación")] long identification,
        [Description("Nombre")] string name,
        [Description("Apellido")] string lastname,
        [Description("Correo electronico")] string email,
        [Description("Telefono")] long phone,
        [Description("Razón")] string reason)
    {
        var patient = new Patient
        {
            Identification = identification,
            Name = name,
            LastName = lastname,
            Email = email,
            Phone = phone,
            Reason = reason,
        };

        await this.patientService.Create(patient);
        await this.chatService.AsociateSender(Guid.Parse(chatId), patient.Id);

        var chatGuid = Guid.Parse(chatId);
        var prompt = $"""
        [Parametro]
        - patientId: {patient.Id}
        """;
        await this.chatService.NewMessage(chatGuid, MessageTypeEnum.Prompt, prompt);
    }

    /// <summary>
    /// Gets the doctors information.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="chatId">Chat identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [KernelFunction("GetDoctorsInfo")]
    [Description("Get the doctors information.")]
    public async Task<IEnumerable<DoctorInfoDto>> GetDoctorsInfoAsync(
        Kernel kernel,
        [Description("Chat identifier")] string chatId)
    {
        try
        {
            int maxCharacters = 120;
            var chatGuid = Guid.Parse(chatId);
            StringBuilder sbDoctors = new ();

            if (this.doctors is null)
            {
                this.doctors = new List<DoctorInfoDto>();
                var doctorsInfo = await this.doctorService.GetAvailable();
                sbDoctors.AppendLine("[DoctorsAvailable]");

                foreach (var doctor in doctorsInfo!)
                {
                    string information = string.Empty;

                    if (doctor.Information.Length > maxCharacters)
                    {
                        var resposne = await kernel.InvokePromptAsync(
                        $"""
                    Genera un resumen donde no incluyas el nombre de maximo {maxCharacters} caracteres del doctor {doctor.Name}:
                    {doctor.Information}
                    """, new () { { "doctor.Name", doctor.Name }, { "doctor.Information", doctor.Information } });

                        information = resposne.ToString();
                    }
                    else
                    {
                        information = doctor.Information;
                    }

                    this.doctors.Add(
                        new DoctorInfoDto
                        {
                            Id = doctor.Id.ToString(),
                            Name = doctor.Name + " " + doctor.LastName,
                            Speciality = doctor.Speciality,
                            Information = information,
                        });

                    sbDoctors.AppendLine($"Id: {doctor.Id} - Name: {doctor.Name} + {doctor.LastName}");
                }
            }

            await this.chatService.NewMessage(chatGuid, MessageTypeEnum.Prompt, sbDoctors.ToString());

            return this.doctors;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Array.Empty<DoctorInfoDto>();
        }
    }

    /// <summary>
    /// Gets the most recent available appointment.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="doctorId">Doctor identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [KernelFunction("GetMostRecentAvailableAppointment")]
    [Description("Get the most recent available appointment.")]
    public async Task<DateTime> GetMostRecentAvailableAppointmentAsync(
        Kernel kernel,
        [Description("Doctor identifier")] string doctorId)
    {
        try
        {
            var id = Guid.Parse(doctorId);
            return await this.doctorService.GetMostRecentAvailableAppointmentAsync(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return DateTime.Now;
        }
    }

    /// <summary>
    /// Gets the available appointment by date.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="doctorId">Doctor identifier.</param>
    /// <param name="date">Date.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [KernelFunction("GetAvailableAppointmentByDate")]
    [Description("Get the available appointment by date.")]
    public async Task<IEnumerable<DateTime>> GetAvailableAppointmentByDateAsync(
        Kernel kernel,
        [Description("Doctor identifier")] string doctorId,
        [Description("Date")] string date)
    {
        try
        {
            var id = Guid.Parse(doctorId);
            var dateTime = DateTime.Parse(date);
            return await this.doctorService.GetAvailableAppointmentsByDate(id, dateTime);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return Array.Empty<DateTime>();
        }
    }

    /// <summary>
    /// Registers an appointment.
    /// </summary>
    /// <param name="kernel">Kernel.</param>
    /// <param name="chatId">Chat identifier.</param>
    /// <param name="doctorId">Doctor identifier.</param>
    /// <param name="patientId">Patient identifier.</param>
    /// <param name="dateTime">Date time.</param>
    /// <param name="reason">Reason.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [KernelFunction("RegisterAppointmentAsync")]
    [Description("Register an appointment.")]
    public async Task<bool> RegisterAppointmentAsync(
        Kernel kernel,
        [Description("Chat identifier")] string chatId,
        [Description("Doctor identifier")] string doctorId,
        [Description("Patient identifier")] string patientId,
        [Description("Date time")] string dateTime,
        [Description("Reason")] string reason)
    {
        try
        {
            var patient = await this.patientService.Find(Guid.Parse(patientId));
            var doctor = await this.doctorService.Find(Guid.Parse(doctorId));

            await this.programationService.CreateAppointment(Guid.Parse(doctorId), Guid.Parse(patientId), DateTime.Parse(dateTime), reason);

            ////Send email to patient
            await this.mailService.SendEmailTemplateAsync("d-25e49bc28a6f401fb79871e6f9e6ed96",
            new List<string> { patient!.Email },
            new Dictionary<string, string>
            {
                { "Patient_Name", patient!.Name },
                { "Doctor_Full_Name", doctor!.Name + " " + doctor!.LastName },
                { "Fecha", dateTime },
            });

            ////Send email to doctor
            await this.mailService.SendEmailTemplateAsync("d-718d9d0dcced4e1aa1355e2eab99ae6c",
            new List<string> { doctor!.Email },
            new Dictionary<string, string>
            {
                { "Doctor_Name", doctor!.Name },
                { "Patient_Name", patient!.Name + " " + patient!.LastName },
                { "Fecha", dateTime },
                { "Patient_Mail", patient!.Email },
                { "Patient_Phone", patient!.Phone.ToString() },
                { "Motivo", reason },
            });

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}

#pragma warning restore SA1116 // SplitParametersMustStartOnLineAfterDeclaration
#pragma warning restore SA1201 // ElementsMustAppearInTheCorrectOrder