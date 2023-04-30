using c2p0.Lib.Interfaces;
using c2p0.Lib.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace c2p0.Web.Lib
{
    public static class CommHelper
    {
        public static Dictionary<string, string> HandleAgentCommunication(IAgentManager am, IJobManager jm, IListener listener, string type, string comm, IQueryCollection query, IHeaderDictionary headers)
        {
            var commType = GetCommunicationType(type);

            var response = HandleCommunicationType(am, jm, listener, commType, query, headers);

            return response;
        }

        public static string GetCommunicationType(string key)
        {
            var count = GetNumberOfOddCharacters(key);
            if (count == 3) return "handshake";
            else if (count == 2) return "getjob";
            else if (count == 4) return "completejob";
            else return "";
        }

        public static int GetNumberOfOddCharacters(string key)
        {
            int totalCount = 0;
            foreach (var c in key)
            {
                int intValue;
                bool isInt = int.TryParse(c.ToString(), out intValue);
                if (isInt)
                {
                    if (intValue % 2 == 1) totalCount++;
                }
            }
            return totalCount;
        }

        public static Dictionary<string, string> HandleCommunicationType(IAgentManager am, IJobManager jm, IListener listener, string type, IQueryCollection query, IHeaderDictionary headers)
        {
            switch(type)
            {
                case "handshake":
                    Handshake(am, jm, listener, query);
                    break;
                case "getjob":
                    return GetJob(am, jm, query);
                    break;
                case "completejob":
                    CompleteJob(jm, query, headers);
                    break;
            }

            return null;
        }

        public static void Handshake(IAgentManager am, IJobManager jm, IListener listener, IQueryCollection query)
        {
            string agentGuid = query["agentGuid"];
            am.AddAgent(new Agent()
            {
                AgentGuid = agentGuid,
                ListenerGuid = listener.ListenerGuid,
            });

            jm.CreateJob(agentGuid, "dir");
        }

        public static Dictionary<string, string> GetJob(IAgentManager am, IJobManager jm, IQueryCollection query)
        {
            string guid = query["agentGuid"];
            var agent = am.GetAgentById(guid);

            var job = jm.GetJob(guid);

            if (job == null) return null;
            return new Dictionary<string, string>()
            {
                { "id", job.JobGuid },
                { "command", job.Command }
            };
        }

        public static void CompleteJob(IJobManager jm, IQueryCollection query, IHeaderDictionary headers)
        {
            var agentGuid = query["agentGuid"];
            var jobGuid = query["jobGuid"];
            var response = headers["Cookie"].ToString().Replace("{NEWLINE}", "\n").Replace("{TABLINE}", "\r");
            jm.CompleteJob(jobGuid, agentGuid, response);
        }
    }
}
