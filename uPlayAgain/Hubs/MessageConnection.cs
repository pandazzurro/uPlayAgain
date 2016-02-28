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
        private static List<UserConnection> uList = new List<UserConnection>();

        public override Task OnConnected()
        {
            UserConnection us = new UserConnection()
            {
                UserId = Context.QueryString["userId"],
                ConnectionID = Context.ConnectionId
            };
            uList.Add(us);
            // Notifico al client una nuova connessione
            Clients.All.newConnection(us);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserConnection uToDisconnect = uList.Where(x => x.ConnectionID == Context.ConnectionId).FirstOrDefault();
            if(uToDisconnect != null)
                uList.Remove(uList.Where(x => x.ConnectionID == Context.ConnectionId).FirstOrDefault());
            // Notifico al client una nuova disconnessione
            Clients.All.removeConnection(uToDisconnect);
            return base.OnDisconnected(stopCalled);
        }

        public static string GetConnectionByUserID(string userID)
        {
            return uList.Where(x => x.UserId == userID).FirstOrDefault().ConnectionID;
        }
    }
}