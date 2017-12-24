using SimpleLogger;
using System;
using NDesk.Options;
using Simocracy.CLSim.GUI;
using Simocracy.CLSim.IO;

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
            try
            {
                Globals.Options.Parse(args);
            }
            catch(OptionException e)
            {
                ConsoleManager.Show();
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
                    ConsoleManager.Show();
                    UafaAlConsole.Simulate(Globals.TeamListFile);
                }
                else
                {
                    // GUI
                    //app.InitializeComponent();
                    //MainWindow window = new MainWindow();
                    //app.Run(window);
                }
            }
            catch(Exception e)
            {
                if(Globals.IsLogging)
                    SimpleLog.Log(e, framesToSkip: 1);
                Environment.ExitCode = 1;
            }

            app.Shutdown();
            ConsoleManager.Hide();
            if(Globals.IsLogging)
            {
                SimpleLog.Info($"{Globals.ProgramName} {Globals.ProgramVersion} closed");
                SimpleLog.Flush();
                SimpleLog.StopLogging();
            }
        }

        public static void ShowHelp()
        {
            ConsoleManager.Show();
            Console.WriteLine($@"Usage: {System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName} [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine($@"Starts {Globals.ProgramName} with the given options.");
            Console.WriteLine();
            Globals.Options.WriteOptionDescriptions(Console.Out);
        }

        #endregion

    }
}
