using System.Diagnostics;
using System.Threading.Tasks;

namespace backgroundr.daemon.linux {
    public static class ShellHelper
    {
        public static async Task<string> Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            var result = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();
            return result;
        }
    }
}