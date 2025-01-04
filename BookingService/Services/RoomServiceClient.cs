using System.Net.Http.Json;
using BookingService.Models;

namespace BookingService.Services;

public class RoomServiceClient : IRoomServiceClient
{
    private readonly HttpClient _httpClient;

    public RoomServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://room-service:8081/"); 

    }

    public async Task<IEnumerable<Room>> GetAllRoomsAsync()
    {
        var response = await _httpClient.GetAsync("api/rooms/");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<IEnumerable<Room>>() ?? new List<Room>();
    }

    public async Task<Room?> GetRoomByNumberAsync(string roomNumber)
    {
        var response = await _httpClient.GetAsync($"api/rooms/{roomNumber}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Room>();
        }
        return null;
    }
}