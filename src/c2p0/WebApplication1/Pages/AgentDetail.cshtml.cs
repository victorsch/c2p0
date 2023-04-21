using c2p0.Lib.Interfaces;
using c2p0.Lib.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Pages;

namespace c2p0.Web.Pages
{
    public class AgentDetailModel : PageModel
    {
        private readonly ILogger<AgentDetailModel> _logger;
        private readonly IAgentManager agentManager;
        private readonly IJobManager jobManager;
        private readonly IListenerManager listenerManager;
        [BindProperty(SupportsGet = true)]
        public string Guid { get; set; }

        public AgentDetailModel(ILogger<AgentDetailModel> logger, IAgentManager am, IJobManager jm, IListenerManager lm)
        {
            _logger = logger;
            agentManager = am;
            jobManager= jm;
            listenerManager = lm;
        }

        public void OnGet()
        {
            var agent = agentManager.GetAgentById(Guid);

            ViewData.Add("agent", agent);
        }
    }
}
