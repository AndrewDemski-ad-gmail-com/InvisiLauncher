using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace InvisiLauncher
{
    public static class Program
    {
        private const string configName = "InvisiLauncher.config";
        private static readonly string ownPath = AppContext.BaseDirectory;
        private static readonly string configFilePath = Path.Join(ownPath, configName);
        private static int RC;
        private static string[]? Args;

        [STAThread]
        public static int Main(string[] args)
        {
            RC = 0;
            Args = args;

            if (!File.Exists(configFilePath))
            {
                RC = -1;
                Debug.WriteLine(
                    string.Format(
                        "Config file NOT found at {0}, returncode = {1}",
                        configFilePath,
                        RC.ToString()
                    )
                );
            }
            else
            {
                Debug.WriteLine(
                    string.Format(
                        "Config file found at {0}",
                        configFilePath
                    )
                );
                if (args.Length == 0)
                {
                    Debug.WriteLine(
                        string.Format(
                            "args[] is null or empty! We need arguments in commandline, by default I will try to launch whatever is set in {0} file.",
                            configName
                        )
                    );
                }
                else
                {
                    if (args.Length == 1)
                    {
                        Debug.WriteLine(
                            string.Format(
                                "args[] contains 1 element. I expect it to be a filepath which will be used to build arguments string from template stored in configuration at {0}.",
                                configFilePath
                            )
                        );
                    }
                    if (args.Length > 1)    //we expect only path to PS1 file, but accept everything else behind it
                    {
                        Debug.WriteLine(
                            string.Format(
                                "args[] contains more than 1 element, {0} in total",
                                args.Length
                            )
                        );
                    }
                }
                RC = RunProcess(configFilePath, args.Length);
            }
            return RC;
        }

        public static int RunProcess(string _configFilePath, int _iArg)
        {
            try
            {
                Process process = new Process
                {
                    StartInfo = GetProcessStartInfoFromConfigFile(_configFilePath, _iArg)
                };

                Debug.WriteLine(
                    string.Format(
                        "Launching new {0} process with these args: {1}",
                        process.StartInfo.FileName,
                        process.StartInfo.Arguments
                    )
                );
                process.Start();
                process.WaitForExit();
                return process.ExitCode;
            }
            catch (Exception ex)
            {
                RC = -3;
                Debug.WriteLine(ex.Message);
                return RC;
            }
        }

        /// <summary>
        /// Path shenaningans, Iwant full path to PS1 file
        /// </summary>
        /// <param name="arg">it can be relative path (relative to launcher.exe), it will be converted to full path if necessary</param>
        /// <returns></returns>
        public static string FixArg1()
        {
            string arg = Args[0];
            bool isFullPath = Path.IsPathRooted(arg) && !Path.GetPathRoot(arg).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
            if (!isFullPath)
            {
                Debug.WriteLine(string.Format("Expanding {0} to full path", arg));
                string new_arg = Path.GetFullPath(Path.Combine(ownPath, arg));
                Debug.WriteLine(string.Format("{0} was converted to {1}", arg, new_arg));
                Args[0] = new_arg;
                return Args[0];
            } 
            else
            {
                return Args[0];
            }
            
        }

        public static ProcessStartInfo GetProcessStartInfoFromConfigFile(string _configfilepath, int _iArg)
        {
            string default_filename = @"%Windir%\System32\WINDOWSPOWERSHELL\v1.0\powershell.exe";
            string default_arguments = @"-ExecutionPolicy Bypass -sta -noprofile -File {0}";
            
            string custom_filename;
            string custom_arguments;
            
            string filename;
            string arguments;

            string final_filename;
            string final_arguments;

            using (var fileStream = File.Open(_configfilepath, FileMode.Open))
            {
                try
                {


                #region XML setup
                XPathDocument xPath = new XPathDocument(fileStream);
                XPathNavigator navigator = xPath.CreateNavigator();
                XmlNamespaceManager nameSpace = new XmlNamespaceManager(navigator.NameTable);
                nameSpace.AddNamespace("ns", "https://nonexistenthost.com/namespace");
                XPathExpression query;
                #endregion

                #region default_filename
                query = navigator.Compile("ns:configuration/ns:filename");
                query.SetContext(nameSpace);
                if (navigator.SelectSingleNode(query) != null)
                {
                    custom_filename = navigator.SelectSingleNode(query).Value;
                    Debug.WriteLine(
                        string.Format(
                            "setting custom config value of {0} = {1}",
                            nameof(custom_filename),
                            custom_filename
                        )
                    );
                    filename = custom_filename;
                }
                else
                {
                    Debug.WriteLine(
                        string.Format(
                            "returning to default value of {0} = {1}",
                            nameof(default_filename),
                            default_filename
                        )
                    );
                    filename = default_filename;
                }
                #endregion

                #region default_arguments
                query = navigator.Compile("ns:configuration/ns:args");
                query.SetContext(nameSpace);
                if (navigator.SelectSingleNode(query) != null)
                {
                    custom_arguments = navigator.SelectSingleNode(query).Value;
                    Debug.WriteLine(
                        string.Format(
                            "setting custom config value of {0} = {1}",
                            nameof(custom_arguments),
                            custom_arguments
                        )
                    );
                    arguments = custom_arguments;
                }
                else
                {
                    Debug.WriteLine(
                        string.Format(
                            "returning to default value of {0} = {1}",
                            nameof(default_arguments),
                            default_arguments
                        )
                    );
                    arguments = default_arguments;
                }
                    #endregion
                }
                catch (Exception ex)
                {
                    RC = - 2;
                    Debug.WriteLine(ex.Message);
                    return RC;
                }
            }
            final_filename = Environment.ExpandEnvironmentVariables(filename);

            switch (_iArg)
            {
                case 0:
                    final_arguments = Environment.ExpandEnvironmentVariables(arguments);
                    break;
                case 1:
                    FixArg1();
                    final_arguments = Environment.ExpandEnvironmentVariables(
                        string.Format(
                            arguments, 
                            Args[0]
                        )
                    );
                    break;
                default: //count >1 
                    FixArg1();
                    final_arguments = Environment.ExpandEnvironmentVariables(
                        string.Format(
                            arguments, 
                            string.Join(
                                " ",
                                string.Join(" ", Args)
                            )
                        )
                    );
                    break;
            }

            //string final_arguments = Environment.ExpandEnvironmentVariables(string.Format(default_arguments, string.Join(" ", _args)));
            //string final_arguments = Environment.ExpandEnvironmentVariables(default_arguments);
            Debug.WriteLine(string.Format("{0} Value = {1}", nameof(final_filename), final_filename));
            Debug.WriteLine(string.Format("{0} Value = {1}", nameof(final_arguments), final_arguments));
            ProcessStartInfo info = new ProcessStartInfo(final_filename, final_arguments);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = false;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            return info;
        }
    }
}