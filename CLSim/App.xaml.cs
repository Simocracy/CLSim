using SimpleLogger;
using System;
using NDesk.Options;
using Simocracy.CLSim.GUI;

namespace Simocracy.CLSim
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App
    {

        #region Main methods

        [STAThread]
        public static void Main(params string[] args)
        {
            // get args
            //List<string> arguments;
            try
            {
                Globals.Options.Parse(args);
            }
            catch(OptionException e)
            {
                Console.Write($@"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName}: ");
                Console.WriteLine(e.Message);
                Console.WriteLine($@"Try '{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName} --help' for more information.");
                return;
            }

            SimpleLog.Severity logSeverity;
            if(!Enum.TryParse(Globals.LogLevel, true, out logSeverity) || Globals.ShowHelp)
            {
                ShowHelp();
                return;
            }

            // Setup logger
            if(Globals.IsLogging)
            {
                SimpleLog.SetLogFile(logDir: "CLSim-Log", writeText: true, check: false, logLevel: logSeverity);
                SimpleLog.Check($"{Globals.ProgramName} {Globals.ProgramVersion} started.");
            }

            // Load Settings
            //Settings.LoadSettings();

            // Start Program
            App app = new App();
            try
            {
                if(Globals.IsUafaClSimulation)
                {
                    // UAFA CL in Console
                    
                }
                else
                {
                    // GUI
                    app.InitializeComponent();
                    MainWindow window = new MainWindow();
                    app.Run(window);
                }
            }
            catch(Exception e)
            {
                if(Globals.IsLogging)
                    SimpleLog.Log(e, framesToSkip: 1);
                Environment.ExitCode = 1;
            }

            app.Shutdown();
            if(Globals.IsLogging)
            {
                SimpleLog.Info($"{Globals.ProgramName} {Globals.ProgramVersion} closed");
                SimpleLog.Flush();
                SimpleLog.StopLogging();
            }
        }

        public static void ShowHelp()
        {
            Console.WriteLine($@"Usage: {System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName} [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine($@"Starts {Globals.ProgramName} with the given options.");
            Console.WriteLine();
            Globals.Options.WriteOptionDescriptions(Console.Out);
        }

        #endregion

    }
}
