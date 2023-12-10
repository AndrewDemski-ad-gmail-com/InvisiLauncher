using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;

namespace InvisiLauncher
{
    public static class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            if (args is null || args.Length == 0)
            {
                Debug.WriteLine("args[] is null or empty! We need arguments in commandline, I cannot launch raw invisible PS console");
                return -2;
            }
            else // if (args.Length > 1)    //we expect only path to PS1 file, but accept everything else behind it
            {
                try
                {
                    string? filename_template;
                    string? arguments_template;

                    string ownPath = AppContext.BaseDirectory;
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreWhitespace = true;
                    settings.IgnoreComments = true;
                    using (var fileStream = File.Open("InvisiLauncher.config", FileMode.Open))
                    {
                        XPathDocument xPath = new XPathDocument(fileStream);
                        XPathNavigator navigator = xPath.CreateNavigator();
                        XmlNamespaceManager nameSpace = new XmlNamespaceManager(navigator.NameTable);
                        nameSpace.AddNamespace("ns", "https://nonexistenthost.com/namespace");
                        XPathExpression query;
                        query = navigator.Compile("ns:configuration/ns:filename");
                        query.SetContext(nameSpace);
                        if (navigator.SelectSingleNode(query) is not null)
                        {
                            filename_template = navigator.SelectSingleNode(query).Value;
                            Debug.WriteLine(string.Format("config value of {0} = {1}", nameof(filename_template), filename_template));
                        }
                        else
                        {
                            filename_template = @"%Windir%\System32\WINDOWSPOWERSHELL\v1.0\powershell.exe";
                            Debug.WriteLine(string.Format("returning to default value of {0} = {1}", nameof(filename_template), filename_template));
                        }
                        query = navigator.Compile("ns:configuration/ns:args");
                        query.SetContext(nameSpace);
                        if (navigator.SelectSingleNode(query) is not null)
                        {
                            arguments_template = navigator.SelectSingleNode(query).Value;
                            Debug.WriteLine(string.Format("config value of {0} = {1}", nameof(arguments_template), arguments_template));
                        }
                        else
                        {
                            arguments_template = @"-ExecutionPolicy Bypass -sta -noprofile -File {0}";
                            Debug.WriteLine(string.Format("returning to default value of {0} = {1}", nameof(arguments_template), arguments_template));
                        }
                        Debug.WriteLine(string.Format("{0} Value = {1}", nameof(arguments_template), arguments_template));
                    }
                    string final_filename = Environment.ExpandEnvironmentVariables(filename_template);
                    string final_arguments = string.Format(arguments_template, string.Join(" ", args));
                    Debug.WriteLine(string.Format("{0} Value = {1}", nameof(final_filename), final_filename));
                    Debug.WriteLine(string.Format("{0} Value = {1}", nameof(final_arguments), final_arguments));
                    ProcessStartInfo info = new ProcessStartInfo(final_filename, final_arguments);
                    info.UseShellExecute = false;
                    info.RedirectStandardOutput = true;
                    info.RedirectStandardError = true;
                    info.RedirectStandardInput = false;
                    info.WindowStyle = ProcessWindowStyle.Hidden;
                    Process process = new()
                    {
                        StartInfo = info
                    };
                    process.Start();
                    process.WaitForExit();
                    int exitCode = process.ExitCode;
                    return exitCode;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return -3;
                }
            }
        }
    }
}