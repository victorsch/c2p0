using c2p0.Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2p0.Lib.Models
{
    public interface IListener
    {
        public string ListenerGuid { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public bool Running { get; set; }
        public List<IAgent> Communicants { get; set; }
        public bool Init(string name, int port, IAgentManager am, IJobManager jm);
        public bool Start();
        public bool Stop();
        public bool AddCommunicant(IAgent agent);
        public bool RemoveCommunicant(IAgent agent);
        public bool BlacklistCommunicant(IAgent agent);
        public IJob GetJob(string agentGuid);
        public void CompleteJob(string agentGuid, string jobGuid, string response);
    }
}
