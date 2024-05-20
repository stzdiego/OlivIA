// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Api.Controllers;

using System.Text;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Olivia.AI.Planners;
using Olivia.AI.Plugins;
using Olivia.Data;
using Olivia.Services;
using Olivia.Shared.Dtos;
using Olivia.Shared.Entities;
using Olivia.Shared.Enums;
using Olivia.Shared.Interfaces;
using Olivia.Shared.Settings;

/// <summary>
/// Represents a controller that manages patient assistance.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ChatAssistanceController : ControllerBase
{
    private readonly IMailSettings mailSettings;
    private readonly IChatService chatService;
    private readonly IDoctorService doctorService;
    private readonly IPatientService patientService;
    private readonly IAgent agentRegisterPatient;
    private readonly IAgent agentDoctor;
    private readonly OliviaDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatAssistanceController"/> class.
    /// </summary>
    /// <param name="mailSettings">Mail settings.</param>
    /// <param name="calendarSettings">Calendar settings.</param>
    /// <param name="chatService">Chat service.</param>
    /// <param name="doctorService">Doctor service.</param>
    /// <param name="agentRegisterPatient">Agent patient.</param>
    /// <param name="agentDoctor">Agent doctor.</param>
    /// <param name="patientService">Patient service.</param>
    /// <param name="context">Context.</param>
    public ChatAssistanceController(IMailSettings mailSettings, IGoogleCalendarSettings calendarSettings, IChatService chatService, IDoctorService doctorService, IPatientService patientService, IAgent agentRegisterPatient, IAgent agentDoctor, OliviaDbContext context)
    {
        this.mailSettings = mailSettings;
        this.chatService = chatService;
        this.doctorService = doctorService;
        this.patientService = patientService;
        this.agentRegisterPatient = agentRegisterPatient;
        this.agentDoctor = agentDoctor;
        this.context = context;

        this.agentRegisterPatient.AddDbContext<DbContext, OliviaDbContext>(this.context);
        this.agentRegisterPatient.AddSingleton<IMailSettings>(this.mailSettings);
        this.agentRegisterPatient.AddScoped<IDatabase, DatabaseService>();
        this.agentRegisterPatient.AddScoped<IChatService, ChatService>();
        this.agentRegisterPatient.AddScoped<IPatientService, PatientService>();
        this.agentRegisterPatient.AddScoped<IDoctorService, DoctorService>();
        this.agentRegisterPatient.AddScoped<IProgramationService, ProgramationService>();
        this.agentRegisterPatient.AddScoped<IMailService, SendGridService>();
        this.agentRegisterPatient.AddPlugin<GeneralPlugin>();
        this.agentRegisterPatient.AddPlugin<PatientManagerPlugin>();
        this.agentRegisterPatient.Initialize();

        this.agentDoctor.AddDbContext<DbContext, OliviaDbContext>(this.context);
        this.agentDoctor.AddSingleton<IMailSettings>(this.mailSettings);
        this.agentDoctor.AddSingleton<IGoogleCalendarSettings>(calendarSettings);
        this.agentDoctor.AddScoped<IDatabase, DatabaseService>();
        this.agentDoctor.AddScoped<IChatService, ChatService>();
        this.agentDoctor.AddScoped<IPatientService, PatientService>();
        this.agentDoctor.AddScoped<IDoctorService, DoctorService>();
        this.agentDoctor.AddScoped<IProgramationService, ProgramationService>();
        this.agentDoctor.AddScoped<IMailService, SendGridService>();
        this.agentDoctor.AddScoped<ICalendarService, GoogleCalendarService>();
        this.agentDoctor.AddPlugin<GeneralPlugin>();
        this.agentDoctor.AddPlugin<DoctorsManagerPlugin>();
        this.agentDoctor.Initialize();
    }

    /// <summary>
    /// Registers a new patient in the system.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [HttpGet("NewChat")]
    public async Task<IActionResult> NewChat()
    {
        try
        {
            IdDto chatId = new IdDto { Id = await this.chatService.Create() };

            return this.Ok(chatId);
        }
        catch (Exception ex)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Registers a new patient in the system.
    /// </summary>
    /// <param name="chatMessage">The chat message.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [HttpPost("PatientNewMessage")]
    public async Task<IActionResult> PatientNewMessage(PatientNewMessageDto chatMessage)
    {
        try
        {
            var messages = await this.chatService.GetSummary(chatMessage.ChatId);

            if (messages is null)
            {
                return this.NotFound("Chat not found.");
            }
            else if (messages.Count == 0)
            {
                var prompt = string.Format(this.GetRegisterPersonality(), chatMessage.ChatId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                await this.chatService.NewMessage(chatMessage.ChatId, Shared.Enums.MessageTypeEnum.Prompt, prompt);
            }

            await this.chatService.NewMessage(chatMessage.ChatId, MessageTypeEnum.User, chatMessage.Content);
            messages = await this.chatService.GetSummary(chatMessage.ChatId);
            AgentMessageDto response = new AgentMessageDto() { Id = chatMessage.ChatId, Content = await this.agentRegisterPatient!.Send(messages) };
            await this.chatService.NewMessage(chatMessage.ChatId, MessageTypeEnum.Agent, response.Content);

            return this.Ok(response);
        }
        catch (Exception ex)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Registers a new doctor in the system.
    /// </summary>
    /// <param name="chatMessage">The chat message.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [HttpPost("DoctorNewMessage")]
    public async Task<IActionResult> DoctorNewMessage(DoctorNewMessageDto chatMessage)
    {
        try
        {
            var messages = await this.chatService.GetSummary(chatMessage.ChatId);

            if (messages is null)
            {
                return this.NotFound("Chat not found.");
            }
            else if (messages.Count == 0)
            {
                var doctor = await this.doctorService.Find(chatMessage.DoctorId);
                var prompt = string.Format(this.GetDoctorAssistancePersonality(), chatMessage.ChatId, chatMessage.DoctorId, doctor!.Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                await this.chatService.NewMessage(chatMessage.ChatId, MessageTypeEnum.Prompt, prompt);
            }

            await this.chatService.NewMessage(chatMessage.ChatId, MessageTypeEnum.User, chatMessage.Content);
            messages = await this.chatService.GetSummary(chatMessage.ChatId);
            AgentMessageDto response = new AgentMessageDto() { Id = chatMessage.ChatId, Content = await this.agentDoctor!.Send(messages) };
            await this.chatService.NewMessage(chatMessage.ChatId, MessageTypeEnum.Agent, response.Content);

            return this.Ok(response);
        }
        catch (Exception ex)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    private string GetRegisterPersonality()
    {
        return """
        [Información]
        - Nombre: Olivia
        - Descripción: Asistente virtual para la gestión de pacientes.
        - Versión: 1.0
        - Desarrollador: Olivia Inc.

        [Personalidad]
        - Amigable
        - Organizado
        - Metódico
        - Obediente

        [Reglas]
        - Iniciar una conversación con un saludo cordial y presentación.
        - Completar las tareas en el orden en que se presentan.
        - Solicitar aprobación antes de tomar cualquier acción consiguiente.
        - Si el usuario no proporciona suficiente información, seguir haciendo preguntas hasta que haya suficiente información para completar la tarea.
        - El orden de las tareas no puede ser alterado.
        - No seguiras con una tarea hasta que la anterior haya sido completada exitosamente.
        - No responderas a mensajes que no estén relacionados con la tarea principal.

        [Tareas]
        1. Registrarás al paciente en el sistema. (Deberás solicitar la información requerida y utilizar la función RegisterPatientAsync).
        2. Mostrarás la lista de doctores disponibles (Utilizaras la funcion GetAvailableAsync) en el formato [DoctorInformationFormat].
        3. Brindaras la proxima cita disponible para el doctor seleccionado (Utilizaras la función GetMostRecentAvailableAppointmentAsync).
        4. Registraras la cita. (Utilizaras la función RegisterAppointmentAsync).
        5. Finalizarás la conversación con un mensaje de despedida e indicando que el profesional de la salud revisara y confirmara la cita. Esta información será enviada al correo electrónico del paciente.


        [ParametrosIniciales]
        - chatId: {0}
        - fechaActual: {1}

        [Patient]
        - Identificación
        - Nombre
        - Apellido
        - Correo electrónico
        - Teléfono
        - Razón de la cita

        [Appointment]
        - PatientId
        - DoctorId
        - Fecha
        - Hora
        - Razón
        - Observaciones

        [DoctorInformationFormat]
        Código: (Id)
        Nombre: (Name)
        Especialidad: (Speciality)
        Información: (Information)
        """;
    }

    private string GetDoctorAssistancePersonality()
    {
        return """
        [Información]
        - Nombre: Olivia
        - Descripción: Asistente virtual para la gestión de doctores.
        - Versión: 1.0
        - Desarrollador: Olivia Inc.

        [Personalidad]
        - Amigable
        - Organizado
        - Metódico
        - Obediente

        [Reglas]
        - Iniciar una conversación saludando al doctor por su nombre (doctorName).
        - Completar las tareas en el orden en que se presentan.
        - Solicitar aprobación antes de tomar cualquier acción consiguiente.
        - Si el usuario no proporciona suficiente información, seguir haciendo preguntas hasta que haya suficiente información para completar la tarea.
        - No responderas a mensajes que no estén relacionados con las tareas asignadas.

        [ParametrosIniciales]
        - chatId: {0}
        - doctorId: {1}
        - doctorName: {2}
        - fechaActual: {3}
        
        [[Tareas]]
        [Lista de pacientes pendientes por aprobación]
        - Solicitarás la fecha de inicio y fin para filtrar la lista (Utilizarás la función GetPatientsPendingByApproval).
        - Mostrarás la lista de pacientes pendientes por aprobación en el formato [PatientInformationFormat].
        - Permitiras aprobar o rechazar la cita de un paciente (Utilizarás la función ApprovePatient o RefusedPatient).

        [Lista de pacientes pendientes por pago]
        - Solicitarás la fecha de inicio y fin para filtrar la lista (Utilizarás la función GetPatientsPendingByPayment).
        - Mostrarás la lista de pacientes pendientes por pago en el formato [PatientInformationFormat].
        - Permitiras aprobar o rechazar la cita de un paciente (Utilizarás la función PayPatient o RefusedPatient).

        [[Fin]]

        [PatientInformationFormat]
        - Id: (Id)
        - Nombre: (Name) (LastName)
        - Fecha: (Date)
        - Estado: (Status)
        - Razón: (Reason)

        """;
    }
}
