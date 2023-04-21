using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace c2p0.Web.Controllers
{
    public class AgentController : Controller
    {
        [HttpGet("{agentGuid}/jobs/{jobGuid}")]
        public ActionResult GetJob()
        {
            return Ok("thanks");
        }

        [HttpPost("{agentGuid}/jobs/{jobGuid}")]
        public IActionResult CompleteJob()
        {
            return Ok();
        }
    }
}
