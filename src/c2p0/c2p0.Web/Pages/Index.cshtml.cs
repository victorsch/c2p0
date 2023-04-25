using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using c2p0.Web.Lib;
using c2p0.Lib.Interfaces;

namespace WebApplication1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IListenerManager listenerManager;
        private readonly IAgentManager agentManager;
        private readonly IJobManager jobManager;

        public IndexModel(ILogger<IndexModel> logger, IListenerManager lm, IAgentManager am, IJobManager jm)
        {
            listenerManager = lm;
            agentManager = am;
            jobManager = jm;
            _logger = logger;
        }

        public void OnGet()
        {
            //DemoListener dl = new DemoListener();
            //dl.Init("test", 7869, agentManager, jobManager);
            //dl.Start();

            //listenerManager.AddListener(dl);
        }
    }
}