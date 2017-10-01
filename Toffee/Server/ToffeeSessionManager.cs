using System.Collections.Generic;

namespace Toffee.Server
{
    public class ToffeeSessionManager
    {
        public ToffeeServer Server { get; private set; }
        private List<ToffeeSession> _Sessions { get; set; }
        public ToffeeSession[] Sessions
        {
            get
            {
                return _Sessions.ToArray();
            }
        }
        private Dictionary<int, ToffeeSession> SessionLookup { get; set; }

        private List<bool> UniqueIdentifiers { get; set; }

        public ToffeeSessionManager(ToffeeServer server)
        {
            Server = server;
            _Sessions = new List<ToffeeSession>();
            SessionLookup = new Dictionary<int, ToffeeSession>();
            UniqueIdentifiers = new List<bool>();
        }

        public void NewSession(ToffeeSession session)
        {
            session.SessionId = GetUniqueIdentifier();
            _Sessions.Add(session);
            SessionLookup.Add(session.SessionId, session);
        }

        public void SessionDisconnected(ToffeeSession session)
        {
            if (session.SessionId >= UniqueIdentifiers.Count)
                return;
            UniqueIdentifiers[session.SessionId] = false;
            _Sessions.Remove(session);
            SessionLookup.Remove(session.SessionId);

            if (Server.Log != null)
                Server.Log.Info("Session {0} disconnected.", session.SessionId);
            Server.SessionDisconnected();
        }

        private int GetUniqueIdentifier()
        {
            for (int i = 0; i < UniqueIdentifiers.Count; i++)
            {
                if (!UniqueIdentifiers[i])
                {
                    UniqueIdentifiers[i] = true;
                    return i;
                }
            }
            int id = UniqueIdentifiers.Count;
            UniqueIdentifiers.Add(true);
            return id;
        }
    }
}
