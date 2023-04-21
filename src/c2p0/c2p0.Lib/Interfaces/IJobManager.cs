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
        public IJob GetJobByGuid(string jobGuid);
        public Job CreateJob(string agentGuid, string command);
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

        public IJob GetJobByGuid(string jobGuid)
        {
            return Jobs.FirstOrDefault(x => x.JobGuid == jobGuid);
        }

        public Job CreateJob(string agentGuid, string command)
        {
            var job = new Job()
            {
                JobGuid = Guid.NewGuid().ToString(),
                AgentGuid = agentGuid,
                Command = command
            };

            Jobs.Add(job);

            return job;
        }
        public void CompleteJob(string jobGuid, string agentGuid, string response)
        {
            var job = Jobs.FirstOrDefault(x => x.JobGuid == jobGuid);
            job.CompleteJob(response);
        }
    }
}
