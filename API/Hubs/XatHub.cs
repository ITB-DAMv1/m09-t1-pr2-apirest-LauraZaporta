using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class XatHub : Hub
    {
        // Mètode invocable des del client
        public async Task EnviaMissatge(string usuari, string missatge)
        {
            // Envia el missatge a tots els clients connectats
            await Clients.All.SendAsync("RepMissatge", usuari, missatge);
        }
    }
}