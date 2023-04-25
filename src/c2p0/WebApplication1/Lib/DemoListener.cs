using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.Net;
using c2p0.Lib.Models;
using c2p0.Lib.Interfaces;

namespace c2p0.Web.Lib
{
    public class DemoListener : IListener
    {
        protected IListener Self { get; set; }
        protected IAgentManager AgentManager { get; set; }
        protected IJobManager JobManager { get; set; }
        public string ListenerGuid { get; set; }
        public string Name { get; set; }
        public int Port { get; set; }
        public bool Running { get; set; } = false;
        public List<IAgent> Communicants { get; set; }

        public CancellationTokenSource _cancelToken;
        public bool Init(string name, int port, IAgentManager am, IJobManager jm)
        {
            ListenerGuid = Guid.NewGuid().ToString();
            Name = name;
            Port = port;
            AgentManager = am;
            JobManager = jm;
            Self = this;
            return true;
        }

        public bool Start()
        {
            var hostBuilder = new Microsoft.Extensions.Hosting.HostBuilder();
            hostBuilder.ConfigureWebHostDefaults(host =>
            {
                host.UseUrls($"http://0.0.0.0:{Port}");
                host.Configure(ConfigureApplication);
                host.ConfigureServices(ConfigureServices);
            });
            var host = hostBuilder.Build();

            _cancelToken = new CancellationTokenSource();
            host.RunAsync(_cancelToken.Token);
            Running = true;
            return true;
        }

        public bool Stop()
        {
            Running = false;
            return true;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(Self);
            services.AddSingleton(AgentManager);
            services.AddSingleton(JobManager);
        }

        private void ConfigureApplication(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseDeveloperExceptionPage();
            app.UseEndpoints(endpoint => {
                endpoint.MapControllerRoute("/", "/", new { controller = "Listener", action = "Index" });
                endpoint.MapControllerRoute("/Handshake", "/Handshake", new { controller = "Listener", action = "Handshake" });
                endpoint.MapControllerRoute("/GetJob", "/GetJob", new { controller = "Listener", action = "GetJob" });
                endpoint.MapControllerRoute("/CompleteJob", "/CompleteJob", new { controller = "Listener", action = "CompleteJob" });
            });
        }

        public bool AddCommunicant(IAgent agent)
        {
            throw new NotImplementedException();
        }
        public bool RemoveCommunicant(IAgent agent)
        {
            throw new NotImplementedException();
        }
        public bool BlacklistCommunicant(IAgent agent)
        {
            throw new NotImplementedException();
        }
        public IJob GetJob(string agentGuid)
        {
            throw new NotImplementedException();
        }
        public void CompleteJob(string agentGuid, string jobGuid, string response)
        {
            throw new NotImplementedException();
        }
    }
}