// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.Api.Controllers;

using System.Text;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly IGoogleCalendarSettings calendarSettings;
    private readonly IChatService chatService;
    private readonly IDoctorService doctorService;
    private readonly IPatientService patientService;
    private readonly IAgent agent;
    private readonly OliviaDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatAssistanceController"/> class.
    /// </summary>
    /// <param name="mailSettings">Mail settings.</param>
    /// <param name="calendarSettings">Calendar settings.</param>
    /// <param name="chatService">Chat service.</param>
    /// <param name="doctorService">Doctor service.</param>
    /// <param name="agent">Agent patient.</param>
    /// <param name="patientService">Patient service.</param>
    /// <param name="context">Context.</param>
    public ChatAssistanceController(IMailSettings mailSettings, IGoogleCalendarSettings calendarSettings, IChatService chatService, IDoctorService doctorService, IPatientService patientService, IAgent agent, OliviaDbContext context)
    {
        this.mailSettings = mailSettings;
        this.calendarSettings = calendarSettings;
        this.chatService = chatService;
        this.doctorService = doctorService;
        this.patientService = patientService;
        this.agent = agent;
        this.context = context;
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
            this.InizializateAgentPatient();
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
            AgentMessageDto response = new AgentMessageDto() { Id = chatMessage.ChatId, Content = await this.agent!.Send(messages), Date = DateTime.Now };
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
            this.InizializateAgentDoctor();
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
            AgentMessageDto response = new AgentMessageDto() { Id = chatMessage.ChatId, Content = await this.agent!.Send(messages), Date = DateTime.Now };
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
        3. Brindaras la proxima cita disponible para el doctor seleccionado (Utilizaras la función GetMostRecentAvailableAppointmentAsync), si esta cita no es la deseada, solicitarás la fecha de la cita y valida disponibilidad (Utiliza la funcion GetAvailableAppointmentByDate).
        4. Registraras la cita. (Utilizaras la función RegisterAppointment).
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
        - Iniciar una conversación saludando al doctor/a por su nombre (doctorName).
        - Completar las tareas en el orden en que se presentan.
        - Solicitar aprobación antes de tomar cualquier acción consiguiente.
        - Si el usuario no proporciona suficiente información, seguir haciendo preguntas hasta que haya suficiente información para completar la tarea.
        - No responderas a mensajes que no estén relacionados con las tareas asignadas.
        - No inventaras funcionalidades o simularas ejecuciones de tareas ya que todas tus tareas deben realizarse con las funciones proporcionadas.
        - Si el doctor te pide que listes todo filtra desde la fecha actual hasta un año después.
        - Si las consultas no tienen resultados, informa al doctor/a que no hay pacientes pendientes, por ninguna razón inventes pacientes.

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
        - Permitiras aprobar la cita de un paciente (Utilizarás la función PayPatient).

        [[Fin]]

        [PatientInformationFormat]
        - Id: (Id)
        - Nombre: (Name) (LastName)
        - Fecha: (Date)
        - Estado: (Status)
        - Razón: (Reason)

        """;
    }

    private void InizializateAgentPatient()
    {
        this.agent.AddDbContext<DbContext, OliviaDbContext>(this.context);
        this.agent.AddSingleton<IMailSettings>(this.mailSettings);
        this.agent.AddScoped<IDatabase, DatabaseService>();
        this.agent.AddScoped<IChatService, ChatService>();
        this.agent.AddScoped<IPatientService, PatientService>();
        this.agent.AddScoped<IDoctorService, DoctorService>();
        this.agent.AddScoped<IProgramationService, ProgramationService>();
        this.agent.AddScoped<IMailService, SendGridService>();
        this.agent.AddPlugin<GeneralPlugin>();
        this.agent.AddPlugin<PatientManagerPlugin>();
        this.agent.Initialize();
    }

    private void InizializateAgentDoctor()
    {
        this.agent.AddDbContext<DbContext, OliviaDbContext>(this.context);
        this.agent.AddSingleton<IMailSettings>(this.mailSettings);
        this.agent.AddSingleton<IGoogleCalendarSettings>(this.calendarSettings);
        this.agent.AddScoped<IDatabase, DatabaseService>();
        this.agent.AddScoped<IChatService, ChatService>();
        this.agent.AddScoped<IPatientService, PatientService>();
        this.agent.AddScoped<IDoctorService, DoctorService>();
        this.agent.AddScoped<IProgramationService, ProgramationService>();
        this.agent.AddScoped<IMailService, SendGridService>();
        this.agent.AddScoped<ICalendarService, GoogleCalendarService>();
        this.agent.AddPlugin<GeneralPlugin>();
        this.agent.AddPlugin<DoctorsManagerPlugin>();
        this.agent.Initialize();
    }
}
