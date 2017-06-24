using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Simocracy.CLSim
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {

        #region Members

        private static bool _IsLogging = true;

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
                SimpleLog.SetLogFile(logDir: "CLSim-Log", writeText: true, check: false);
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
            foreach(var arg in args)
            {
                switch(arg.ToLower())
                {
                    case "-nolog":
                        _IsLogging = false;
                        break;
                }
            }
        }

        #endregion

    }
}
