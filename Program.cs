using System.Diagnostics;

namespace InvisiLauncher
{
    internal static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 1)
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = Environment.ExpandEnvironmentVariables(
                        @"%Windir%\System32\WindowsPowerShell\v1.0\powershell.exe"),
                    Arguments = "-NoLogo -Sta -NoProfile -WindowStyle Hidden -ExecutionPolicy Bypass -File \"" + args[0] + "\"",
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                Process process = new Process
                {
                    StartInfo = psi
                };
                process.Start();
                process.WaitForExit();
                return process.ExitCode;
            }
            else
            {
                return 666;
            }
        }
    }
}