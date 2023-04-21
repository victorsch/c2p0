using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2p0.Lib.Models
{
    // This is only an example of how an agent would be implented in C#, the agent <-> listener contract is further defined in the README for implementation in other languages.
    public interface IAgent
    {
        public string AgentGuid { get; set; }
        public string ListenerGuid { get; set; }
        public string HostName { get; set; }
        public List<IJob> AssignedJobs { get; set; }
        public bool Create(string guid, string hostName);
        public void AssignJob(IJob job);
    }
    public class Agent : IAgent
    {
        public string AgentGuid { get; set; }
        public string ListenerGuid { get; set; }
        public string HostName { get; set; }
        public List<IJob> AssignedJobs { get; set; }
        public bool Create(string guid, string hostName)
        {
            AgentGuid = guid;
            HostName = hostName;
            return true;
        }
        public void AssignJob(IJob job) 
        { 
            AssignedJobs.Add(job);
        }
    }
}
