using c2p0.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace c2p0.Lib.Interfaces
{
    public interface IJobManager
    {
        public List<IJob> Jobs { get; set; }
        public List<IJob> GetJobs();
        public IJob GetJob(string agentGuid);
        public void CreateJob(string agentGuid, string command);
        public void CompleteJob(string jobGuid, string agentGuid, string response);
    }

    public class JobManager : IJobManager
    {
        public List<IJob> Jobs { get; set; } = new List<IJob>();
        public List<IJob> GetJobs()
        {
            return Jobs;
        }
        public IJob GetJob(string agentGuid)
        {
            return Jobs.FirstOrDefault(x => !x.Completed && x.AgentGuid == agentGuid);
        }
        public void CreateJob(string agentGuid, string command)
        {
            Jobs.Add(new Job()
            {
                JobGuid = Guid.NewGuid().ToString(),
                AgentGuid = agentGuid,
                Command = command
            });
        }
        public void CompleteJob(string jobGuid, string agentGuid, string response)
        {
            var job = Jobs.FirstOrDefault(x => x.JobGuid == jobGuid);
            job.CompleteJob(response);
        }
    }
}
