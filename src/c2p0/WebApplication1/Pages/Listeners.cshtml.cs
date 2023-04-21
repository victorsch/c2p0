using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using c2p0.Web.Lib;
using c2p0.Lib.Interfaces;
using c2p0.Lib.Models;

namespace WebApplication1.Pages
{
    public class ListenersModel : PageModel
    {
        private readonly ILogger<ListenersModel> _logger;
        private readonly IListenerManager listenerManager;

        public ListenersModel(ILogger<ListenersModel> logger, IListenerManager lm)
        {
            _logger = logger;
            listenerManager = lm;
        }

        public void OnGet()
        {
            List<IListener> listeners = listenerManager.GetListeners();

            ViewData.Add("listeners", listeners);
        }

        public void OnPostCreate(string name, int port)
        {
            var listener = new DemoListener()
            {
                Name = name,
                Port = port
            };

            listenerManager.AddListener(listener);

            listener.Start();

            List<IListener> listeners = listenerManager.GetListeners();

            ViewData.Add("listeners", listeners);
        }
    }
}