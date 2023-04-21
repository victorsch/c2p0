//using Newtonsoft.Json;
using System.Diagnostics;
using System;
using Microsoft.Extensions.DependencyInjection;
using c2p0.Lib.Interfaces;
using c2p0.Web;
using c2p0.Web.Lib;
using c2p0.Lib.Models;

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
            System.Console.WriteLine("Agent                 Listener        ");
            System.Console.WriteLine("--------------------------------------");
            var agents = am.GetAgents();

            foreach (var agent in agents)
            {
                System.Console.WriteLine("{0}              {1}", agent.AgentGuid, agent.ListenerGuid);
            }
        }

        public static void CreateListener(IListenerManager lm, IAgentManager am, IJobManager jm, string name, int port)
        {
            DemoListener dl = new DemoListener();
            dl.Init(name, port, am, jm);

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
                System.Console.Write("{0}>$ ", agent.AgentGuid);

                string command = System.Console.ReadLine();
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
                    break;
                case "ca":
                    EnterAgentShell(lm, am, jm, commandTokens[1]);
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
            System.Console.WriteLine("▐░░░░░░░░░░░▌▐░░░░░░░░░░░▌▐░░░░░░░░░░░▌ ▐░░░░░░░░░▌");
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

            while (true)
            {
                System.Console.Write("c2p0>$ ");
                string command = System.Console.ReadLine();

                HandleCommand(listenerManager, agentManager, jobManager, command);
            }


        }
    }
}