using Microsoft.Extensions.Configuration;
using Project_MARS.Application.DTOs;
using Project_MARS.Application.Interfaces;
using System.Net.Http.Json;


namespace Project_MARS.Application.Services
{
    public class VinDecoderService : IVinDecoderService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public VinDecoderService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;


            var apiKey = _configuration["AutoDev:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }
        }

        public async Task<VehicleVinDetailsDto?> DecodeVinAsync(string vin)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://api.auto.dev/vin/{vin}");

                if (!response.IsSuccessStatusCode)
                {
                    return null; 
                }

                var result = await response.Content.ReadFromJsonAsync<VehicleVinDetailsDto>();
                return result;
            }
            catch
            {
                return false ? null : null; 
            }
        }
    }
}
