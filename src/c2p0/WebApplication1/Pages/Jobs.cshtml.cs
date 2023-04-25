using c2p0.Lib.Interfaces;
using c2p0.Lib.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication1.Pages;

namespace c2p0.Web.Pages
{
    public class JobsModel : PageModel
    {
        private readonly ILogger<JobsModel> _logger;
        private readonly IJobManager jobManager;
        private readonly IListenerManager listenerManager;

        public JobsModel(ILogger<JobsModel> logger,IJobManager jm ,IListenerManager lm)
        {
            _logger = logger;
            jobManager= jm;
            listenerManager = lm;
        }

        public void OnGet()
        {
            var jobs = jobManager.GetJobs();

            ViewData.Add("jobs", jobs);
        }
    }
}
