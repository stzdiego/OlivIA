using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Olivia.Services;
using Olivia.Shared.Entities;
using Olivia.Shared.Interfaces;

namespace Olivia.AI.Plugins
{
    public class DoctorsManagerPlugin
    {
        private readonly DoctorService _doctors;
        private readonly ChatService _chats;

        public DoctorsManagerPlugin(DoctorService doctors, ChatService chats)
        {
            _doctors = doctors;
            _chats = chats;
        }


        [KernelFunction]
        [Description("Obtiene la información de los doctores")]
        public async Task<IEnumerable<Doctor>> GetInformation(Kernel kernel)
        {
            try
            {
                return await _doctors.Get();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}