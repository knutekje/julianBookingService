using System.Text.Json;
using BookingService.DTOs;
using BookingService.Models;

public class GuestServiceClient : IGuestServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GuestServiceClient> _logger;

    public GuestServiceClient(HttpClient httpClient, ILogger<GuestServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GuestDTO?> GetGuestByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/guests/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GuestDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            _logger.LogWarning("Failed to fetch guest with ID {Id}. Status Code: {StatusCode}", id, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching guest with ID {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<GuestDTO>> GetGuestsByIdsAsync(IEnumerable<int> guestIds)
    {
        try
        {
            var ids = string.Join(",", guestIds);
            var response = await _httpClient.GetAsync($"api/guests?ids={ids}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<GuestDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? Enumerable.Empty<GuestDTO>();
            }

            _logger.LogWarning("Failed to fetch guests. Status Code: {StatusCode}", response.StatusCode);
            return Enumerable.Empty<GuestDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching guests.");
            return Enumerable.Empty<GuestDTO>();
        }
    }
}
