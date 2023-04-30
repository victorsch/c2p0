using c2p0.Lib.Interfaces;
using c2p0.Lib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace c2p0.Web.Controllers
{
    public class ListenerController : Controller
    {
        private readonly IAgentManager agentManager;
        private readonly IJobManager jobManager;
        private readonly IListener listener;
        private readonly byte[] Key;
        private readonly byte[] Iv;
        public ListenerController(IAgentManager am, IJobManager jm, IListener _listener, string key, byte[] iv)
        {
            agentManager = am;
            jobManager = jm;
            listener = _listener;
            Key = Convert.FromBase64String(key);
            Iv = iv;
        }
        public ActionResult Index()
        {
            var key = HttpContext.Request.Query.Keys.FirstOrDefault();
            var responseHeader = HttpContext.Request.Headers;
            var query = HttpContext.Request.Query;
            if (HttpContext.Request.Query.TryGetValue(key, out Microsoft.Extensions.Primitives.StringValues comm))
            {
                var rsp = Lib.CommHelper.HandleAgentCommunication(agentManager, jobManager, listener, key, comm.ToString(), query, responseHeader);
                if (rsp == null) return Ok();
                return Ok(Encrypt(Newtonsoft.Json.JsonConvert.SerializeObject(rsp), Key, Iv));
            }
            return Ok();
        }
        internal static string Encrypt(string message, byte[] key, byte[] iv)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(message); // Write all data to the stream.
            }
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
