using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace c2p0.Lib.Interfaces
{
    public interface IGenerator
    {

    }
    public class Generator : IGenerator
    {
        public async Task<string> Compile(byte[] key, byte[] iv)
        {
            DeleteProjectCopy();
            await CreateProjectCopy();
            ChangeAgentSource(key, iv);

            string cwd = System.IO.Directory.GetCurrentDirectory();
            string sourcepath = cwd + @"\..\..\..\..\temp-new-agent-dir\c2p0.SampleAgent.csproj";
            string outpath = cwd + @"\..\..\..\..\agent-builds\agent.exe";
            string response = string.Empty;
            string command = $"dotnet build {sourcepath} -o {outpath} -p:PublishSingleFile=true --self-contained true";

            Task.Run(() => {
                using (Process cmd = new Process())
                {
                    //Process cmd = new Process();
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

                    cmd.StandardOutput.Close();
                    cmd.Kill();
                }
            }).Wait();

            return outpath;
        }

        public async Task CreateProjectCopy()
        {
            string response = string.Empty;

            string cwd = System.IO.Directory.GetCurrentDirectory();
            string projectDir = cwd + @"\..\..\..\..\c2p0.SampleAgent\";
            string copyProjectDir = cwd + @"\..\..\..\..\temp-new-agent-dir\";

            string command = $"Xcopy {projectDir} {copyProjectDir} /E/H/C/I/Y";

            Task.Run(() =>
            {
                using (Process cmd = new Process())
                {
                    cmd.StartInfo.FileName = "xcopy.exe";
                    cmd.StartInfo.Arguments = $"{projectDir} {copyProjectDir} /E/H/C/I/Y";
                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.CreateNoWindow = false;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    cmd.Start();

                    cmd.StandardInput.WriteLine(command);
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    response = cmd.StandardOutput.ReadToEnd();

                    cmd.WaitForExit();



                    cmd.StandardOutput.Close();
                    cmd.Kill();
                }
            }).Wait();

        }

        public void ChangeAgentSource(byte[] key, byte[] iv)
        {
            string cwd = System.IO.Directory.GetCurrentDirectory();
            string sourcePath = cwd + @"\..\..\..\..\temp-new-agent-dir\Program.cs";
            string fileSourceCode = File.ReadAllText(sourcePath);

            fileSourceCode = fileSourceCode.Replace("{B64KEY}", Convert.ToBase64String(key));
            fileSourceCode = fileSourceCode.Replace("{B64IV}", Convert.ToBase64String(iv));

            File.WriteAllText(sourcePath, fileSourceCode);
        }

        public void DeleteProjectCopy()
        {
            string response = string.Empty;

            string cwd = System.IO.Directory.GetCurrentDirectory();
            string copyProjectDir = cwd + @"\..\..\..\..\temp-new-agent-dir\";

            string command = $"rmdir {copyProjectDir}";

            Task.Run(() =>
            {
                using (Process cmd = new Process())
                {
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
                }
            });
        }
    }
}
