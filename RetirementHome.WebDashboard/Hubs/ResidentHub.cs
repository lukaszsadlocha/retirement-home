using Microsoft.AspNetCore.SignalR;
using RetirementHome.WebDashboard.Models;
using System.Text.Json;

namespace RetirementHome.WebDashboard.Hubs
{
    public class ResidentHub : Hub
    {
        public async Task NotifyNewResident(string message)
        {
            var option = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            

            var newResident = JsonSerializer.Deserialize<Resident>(message, option);
            await Clients.All.SendAsync("recievedNewResident", newResident);
        }
    }
}
