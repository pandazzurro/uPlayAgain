using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using uPlayAgain.Data.EF.Models;
using System.Collections.Concurrent;

namespace uPlayAgain.Hubs
{
    [HubName("messagehub")]
    public class MessageConnection : Hub
    {
        private static ConcurrentDictionary<string, List<int>> _mapping = new ConcurrentDictionary<string, List<int>>();

        public MessageConnection()
        {
            //_mapping.TryAdd(Context.ConnectionId, new List<int>());
            //Clients.All.newConnection(Context.ConnectionId);
            //var a = Clients.All;
        }

        public override Task OnConnected()
        {
            _mapping.TryAdd(Context.ConnectionId, new List<int>());
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            //foreach (var id in _mapping[Context.ConnectionId])
            //{
            //    UnlockHelper(id);
            //}
            var list = new List<int>();
            _mapping.TryRemove(Context.ConnectionId, out list);
            Clients.All.removeConnection(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }


        public void Lock(int id)
        {

            var a = "lock";
        }

        public void Unlock(int id)
        {
            var a = "unlock";
        }

        public void Login(string userId)
        {
            Clients.User(userId);
            Clients.All.sendMessageHub(Context.ConnectionId, "Ciao Ciao");
        }
    }
}