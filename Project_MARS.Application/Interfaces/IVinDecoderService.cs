using Project_MARS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Interfaces
{
    public interface IVinDecoderService
    {
        Task<VehicleVinDetailsDto?> DecodeVinAsync(string vin);
    }
}
