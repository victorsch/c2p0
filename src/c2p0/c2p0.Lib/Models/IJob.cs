using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2p0.Lib.Models
{
    public interface IJob
    {
        public string JobGuid { get; set; }
        public string AgentGuid { get; set; }
        public string Command { get; set; }
        public string Response { get; set; }
        public bool Completed { get; set; }
        public void CreateJob(string command);
        public void CompleteJob(string response);
    }

    public class Job : IJob
    {
        public string JobGuid { get; set; }
        public string AgentGuid { get; set; }
        public string Command { get; set; }
        public string Response { get; set; }
        public bool Completed { get; set; } = false;
        public void CreateJob(string command)
        {
            JobGuid = Guid.NewGuid().ToString();
            Command = command;
        }
        public void CompleteJob(string response)
        {
            Response = response;
            Completed = true;
        }
    }
}
