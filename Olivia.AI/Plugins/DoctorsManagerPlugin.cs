// Copyright (c) Olivia Inc.. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Olivia.AI.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.SemanticKernel;
    using Olivia.Services;
    using Olivia.Shared.Dtos;
    using Olivia.Shared.Entities;
    using Olivia.Shared.Enums;
    using Olivia.Shared.Interfaces;

    /// <summary>
    /// Plugin for managing doctors.
    /// </summary>
    public class DoctorsManagerPlugin : IPlugin
    {
        private readonly IPatientService patientService;
        private readonly IDoctorService doctorService;
        private readonly IChatService chatService;
        private readonly ICalendarService calendarService;
        private readonly IProgramationService programationService;
        private readonly IMailService mailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoctorsManagerPlugin"/> class.
        /// </summary>
        /// <param name="patientService">Patient service.</param>
        /// <param name="doctorService">Doctor service.</param>
        /// <param name="chatService">Chat service.</param>
        /// <param name="calendarService">Calendar service.</param>
        /// <param name="programationService">Programation service.</param>
        /// <param name="mailService">Mail service.</param>
        public DoctorsManagerPlugin(IPatientService patientService, IDoctorService doctorService, IChatService chatService, ICalendarService calendarService, IProgramationService programationService, IMailService mailService)
        {
            this.patientService = patientService;
            this.doctorService = doctorService;
            this.chatService = chatService;
            this.calendarService = calendarService;
            this.programationService = programationService;
            this.mailService = mailService;
        }

        /// <summary>
        /// Gets pending patients by approval.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <param name="chatId">Chat id.</param>
        /// <param name="doctorId">Doctor id.</param>
        /// <param name="start">Start date.</param>
        /// <param name="end">End date.</param>
        /// <returns>Doctor.</returns>
        [KernelFunction("GetPatientsPendingByApproval")]
        [Description("Get patients pending by approval")]
        public async Task<IEnumerable<PatientAppointmentDto>> GetPatientsPendingByApproval(
            Kernel kernel,
            [Description("Chat id")] string chatId,
            [Description("Doctor id")] string doctorId,
            [Description("Date start")] string start,
            [Description("Date end")] string end)
        {
            try
            {
                var patient = await this.doctorService.GetPatientsPendingByDoctorByDate(Guid.Parse(doctorId), DateTime.Parse(start), DateTime.Parse(end), AppointmentStateEnum.PendingApproval);
                StringBuilder sbPatient = new StringBuilder();
                sbPatient.AppendLine("[PatientsPendingByApproval]");

                foreach (var item in patient)
                {
                    sbPatient.AppendLine($"Id: {item.PatientId}");
                    sbPatient.AppendLine($"Name: {item.FullName}");
                    sbPatient.AppendLine($"Status: {item.Status}");
                    sbPatient.AppendLine($"Datetime: {item.Datetime}");
                    sbPatient.AppendLine();
                }

                await this.chatService.NewMessage(Guid.Parse(chatId), MessageTypeEnum.Prompt, sbPatient.ToString());

                return patient;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Array.Empty<PatientAppointmentDto>();
            }
        }

        /// <summary>
        /// Gets patients pending by payment.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <param name="chatId">Chat id.</param>
        /// <param name="doctorId">Doctor id.</param>
        /// <param name="start">Start date.</param>
        /// <param name="end">End date.</param>
        /// <returns>Doctor.</returns>
        [KernelFunction("GetPatientsPendingByPayment")]
        [Description("Get patients pending by payment")]
        public async Task<IEnumerable<PatientAppointmentDto>> GetPatientsPendingByPayment(
            Kernel kernel,
            [Description("Chat id")] string chatId,
            [Description("Doctor id")] string doctorId,
            [Description("Date start")] string start,
            [Description("Date end")] string end)
        {
            try
            {
                var patients = await this.doctorService.GetPatientsPendingByDoctorByDate(Guid.Parse(doctorId), DateTime.Parse(start), DateTime.Parse(end), AppointmentStateEnum.PendingPayment);
                StringBuilder sbPatient = new StringBuilder();
                sbPatient.AppendLine("[PatientsPendingByPayment]");

                foreach (var item in patients)
                {
                    sbPatient.AppendLine($"Id: {item.PatientId}");
                    sbPatient.AppendLine($"Name: {item.FullName}");
                    sbPatient.AppendLine($"Status: {item.Status}");
                    sbPatient.AppendLine($"Datetime: {item.Datetime}");
                    sbPatient.AppendLine();
                }

                await this.chatService.NewMessage(Guid.Parse(chatId), MessageTypeEnum.Prompt, sbPatient.ToString());
                return patients;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Array.Empty<PatientAppointmentDto>();
            }
        }

        /// <summary>
        /// Approves a patient.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <param name="patientId">Patient id.</param>
        /// <param name="doctorId">Doctor id.</param>
        /// <returns>Doctor.</returns>
        [KernelFunction("ApprovePatient")]
        [Description("Approve patient")]
        public async Task<bool> ApprovePatient(
            Kernel kernel,
            [Description("Patient id")] string patientId,
            [Description("Doctor id")] string doctorId)
        {
            try
            {
                var appointment = await this.programationService.Find(Guid.Parse(patientId), Guid.Parse(doctorId));
                var patient = await this.patientService.Find(Guid.Parse(patientId));
                var doctor = await this.doctorService.Find(Guid.Parse(doctorId));

                await this.doctorService.ApprovePatient(Guid.Parse(patientId));

                await this.mailService.SendEmailTemplateAsync(
                    "d-803d4f775cfd46bca6dbf81d38fc2605",
                    new List<string> { patient!.Email },
                    new Dictionary<string, string>
                {
                    { "Patient_Name", patient!.Name },
                    { "Doctor_Full_Name", doctor!.Name + " " + doctor!.LastName },
                    { "Fecha", appointment.Date.ToString("yyyy/MM/dd hh:mm") },
                    { "Doctor_Mail", doctor!.Email },
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Refused a patient.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <param name="patientId">Patient id.</param>
        /// <param name="doctorId">Doctor id.</param>
        /// <returns>Doctor.</returns>
        [KernelFunction("RefusedPatient")]
        [Description("Refused patient")]
        public async Task<bool> RefusedPatient(
            Kernel kernel,
            [Description("Patient id")] string patientId,
            [Description("Doctor id")] string doctorId)
        {
            try
            {
                var patient = await this.patientService.Find(Guid.Parse(patientId));
                var doctor = await this.doctorService.Find(Guid.Parse(doctorId));
                var appointment = await this.programationService.Find(Guid.Parse(patientId), Guid.Parse(doctorId));

                if (patient is null)
                {
                    return false;
                }

                await this.doctorService.RefusedPatient(Guid.Parse(patientId));

                await this.mailService.SendEmailTemplateAsync(
                    "d-1995c1af0601487ba5abf97d3fe94b48",
                    new List<string> { patient!.Email },
                    new Dictionary<string, string>
                {
                    { "Patient_Name", patient!.Name },
                    { "Doctor_Full_Name", doctor!.Name + " " + doctor!.LastName },
                    { "Fecha", appointment.Date.ToString("yyyy/MM/dd hh:mm") },
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Approves a patient.
        /// </summary>
        /// <param name="kernel">Kernel.</param>
        /// <param name="patientId">Patient id.</param>
        /// <param name="doctorId">Doctor id.</param>
        /// <returns>Doctor.</returns>
        [KernelFunction("PayPatient")]
        [Description("Pay patient")]
        public async Task<bool> PayPatient(
            Kernel kernel,
            [Description("Patient id")] string patientId,
            [Description("Doctor id")] string doctorId)
        {
            try
            {
                var appointment = await this.programationService.Find(Guid.Parse(patientId), Guid.Parse(doctorId));
                var patient = await this.patientService.Find(Guid.Parse(patientId));
                var doctor = await this.doctorService.Find(Guid.Parse(doctorId));

                if (await this.doctorService.PayPatient(Guid.Parse(patientId)))
                {
                    await this.calendarService.CreateEvent(
                        "Cita: " + patient!.Name + " " + patient!.LastName,
                        "Razón: " + appointment.Reason,
                        appointment.Date,
                        appointment.Date.AddHours(1),
                        CancellationToken.None);

                    await this.mailService.SendEmailTemplateAsync(
                        "d-d841b30d479247f8981b3544712a281e",
                        new List<string> { patient!.Email },
                        new Dictionary<string, string>
                    {
                        { "Patient_Name", patient!.Name },
                        { "Doctor_Full_Name", doctor!.Name + " " + doctor!.LastName },
                        { "Fecha", appointment.Date.ToString("yyyy/MM/dd hh:mm") },
                    });
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}