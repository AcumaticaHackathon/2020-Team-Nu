using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using PX.Data;

namespace Hackathon
{
    public class ChatHub : Hub
    {
        public void Send(string message)
        {
            Clients.All.broadcastMessage(message);
        }

        public void SendToUser(string userName, string message)
        {
            Clients.Group(userName).addChatMessage(userName, message);
        }

        public override Task OnConnected()
        {
            var name = PXAccess.GetUserName();
            Groups.Add(Context.ConnectionId, name);
            return base.OnConnected();
        }
    }
}