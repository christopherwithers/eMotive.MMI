using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace eMotive.MMI.SignalR
{//
    [HubName("MMIHub")]
    public class MMIHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public void Send(string test)
        {
            Clients.All.addNewMessageToPage(test);
        }

        public void MessageTestA(string message)
        {
            Clients.Group("GroupA").doMessage(message);
        }

        public void MessageTestB(string message)
        {
            Clients.Group("GroupB").doMessage(message);
        }

        public async Task JoinGroup(string group)
        {
            await Groups.Add(Context.ConnectionId, group);
            Clients.Group(group).doMessage("Joined Group:" + group);
        }
        
        public async Task LeaveGroup(string group)
        {
            await Groups.Remove(Context.ConnectionId, group);
            Clients.Group(group).doMessage("Left Group:" + group);
        }

    }
}