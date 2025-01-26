using BookingService.DTOs;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookingService.External
{
    public class RoomServiceClient : IRoomServiceClient
    {
        private readonly HttpClient _httpClient;

        public RoomServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RoomDTO?> GetRoomByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/rooms/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RoomDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<RoomDTO?> GetRoomByRoomNumberAsync(string roomNumber)
        {
            var response = await _httpClient.GetAsync($"/api/rooms/room-number/{roomNumber}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RoomDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return null;
        }

        public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync()
        {
            var response = await _httpClient.GetAsync("/api/rooms");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<RoomDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? Enumerable.Empty<RoomDTO>();
            }
            return Enumerable.Empty<RoomDTO>();
        }

        public async Task<IEnumerable<RoomDTO>> GetDirtyRoomsAsync()
        {
            var response = await _httpClient.GetAsync("/api/rooms/dirty");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<RoomDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? Enumerable.Empty<RoomDTO>();
            }
            return Enumerable.Empty<RoomDTO>();
        }
        public async Task<IEnumerable<RoomDTO>> GetRoomsExcludingIdsAsync(IEnumerable<int> excludedRoomIds)
        {
            var ids = string.Join(",", excludedRoomIds);
            var response = await _httpClient.GetAsync($"api/rooms/excluding-ids?ids={ids}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<IEnumerable<RoomDTO>>();
        }
    }
}
