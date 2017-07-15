using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Simocracy.CLSim.GUI;

namespace Simocracy.CLSim
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {

        #region Members

        private static bool _IsLogging = true;
        private static SimpleLog.Severity _LogLevel = SimpleLog.Severity.Warning;

        #endregion

        #region Main methods

        [STAThread]
        public static void Main(params string[] args)
        {
            // get args
            SetArgs();

            // Setup logger
            if(_IsLogging)
            {
                SimpleLog.SetLogFile(logDir: "CLSim-Log", writeText: true, check: false, logLevel: _LogLevel);
                SimpleLog.Check($"{Globals.ProgramName} {Globals.ProgramVersion} started.");
            }

            // Load Settings
            //Settings.LoadSettings();

            // Open MainWindow
            App app = new App();
            try
            {
                app.InitializeComponent();
                MainWindow window = new MainWindow();
                app.Run(window);
            }
            catch(Exception e)
            {
                //SimpleLog.Error(e.ToString());
                SimpleLog.Log(e, framesToSkip: 1);
                Environment.ExitCode = 1;
            }

            app.Shutdown();
            SimpleLog.Info($"{Globals.ProgramName} {Globals.ProgramVersion} closed");
            SimpleLog.Flush();
            SimpleLog.StopLogging();
        }

        /// <summary>
        /// Sets the arguments
        /// </summary>
        /// <param name="args">arguments</param>
        public static void SetArgs(params string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                switch(args[i].ToLower())
                {
                    case "-nolog":
                        // No Logging
                        _IsLogging = false;
                        break;
                    case "-loglevel":
                        // Sets the log level
                        if(args.Length > i && !String.IsNullOrEmpty(args[i + 1]))
                        {
                            switch(args[i + 1].ToLower())
                            {
                                case "info":
                                    _LogLevel = SimpleLog.Severity.Info;
                                    break;
                                case "warning":
                                    _LogLevel = SimpleLog.Severity.Warning;
                                    break;
                                case "error":
                                    _LogLevel = SimpleLog.Severity.Error;
                                    break;
                                case "exception":
                                    _LogLevel = SimpleLog.Severity.Exception;
                                    break;
                            }
                            i++;
                        }
                        break;
                }
            }
        }

        #endregion

    }
}
