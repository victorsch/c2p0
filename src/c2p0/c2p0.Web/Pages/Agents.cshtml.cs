using c2p0.Lib.Interfaces;
using c2p0.Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Pages;

namespace c2p0.Web.Pages
{
    public class AgentsModel : PageModel
    {
        private readonly ILogger<AgentsModel> _logger;
        private readonly IAgentManager agentManager;
        private readonly IJobManager jobManager;
        private readonly IListenerManager listenerManager;

        public AgentsModel(ILogger<AgentsModel> logger, IAgentManager am, IJobManager jm, IListenerManager lm)
        {
            _logger = logger;
            agentManager = am;
            jobManager= jm;
            listenerManager = lm;
        }

        public void OnGet()
        {
            var agents = agentManager.GetAgents();

            ViewData.Add("agents", agents);
        }
    }
}
