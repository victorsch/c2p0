using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using c2p0.Lib.Models;

namespace c2p0.Lib.Interfaces
{
    public interface IAgentManager
    {
        public List<IAgent> GetAgents();
        public void AddAgent(IAgent agent);
        public bool RemoveAgent(IAgent agent);
        public IAgent GetAgentById(string agentGuid);
    }
    public class AgentManager : IAgentManager
    {
        private List<IAgent> Agents = new List<IAgent>();
        public List<IAgent> GetAgents()
        {
            return Agents;
        }

        public void AddAgent(IAgent agent)
        {
            Agents.Add(agent);
        }

        public bool RemoveAgent(IAgent agent)
        {
            return Agents.Remove(agent);
        }

        public IAgent GetAgentById(string agentGuid)
        {
            return Agents.FirstOrDefault(a => a.AgentGuid == agentGuid);
        }
    }
}
