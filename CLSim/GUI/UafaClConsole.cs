using System;
using Simocracy.CLSim.Football.UAFA;
using Simocracy.CLSim.IO;
using Simocracy.CLSim.PwrBot;
using SimpleLogger;

namespace Simocracy.CLSim.GUI
{
    /// <summary>
    /// Handler for simulating uafa cl per console
    /// </summary>
    public class UafaClConsole
    {

        #region Constuctor

        /// <summary>
        /// Starts a new uafa cl simulation in console
        /// </summary>
        /// <param name="season">The season to be simulated</param>
        public UafaClConsole(string season)
        {
            Cl = new ChampionsLeague(season);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The CL instance
        /// </summary>
        public ChampionsLeague Cl { get; set; }

        #endregion

        #region Main Interaction Methods

        /// <summary>
        /// Initiates the simulation of the Champions League
        /// </summary>
        /// <param name="teamListFile">team list file</param>
        public static void Simulate(string teamListFile = null)
        {
            WriteLine("UAFA Champions League simulation.");
            WriteLine();
            Write("Simulated season: ");
            var season = Read();
            var cl = new UafaClConsole(season);
            WriteLine($"UAFA Champions League season {cl.Cl.Season} created.");

            cl.SimulateGroupStage(teamListFile);
            cl.SimulateRoundOf16();
            cl.SimulateQuarterFinals();
            cl.SimulateSemiFinals();
            cl.SimulateFinal();
            cl.ManageExport();

            WriteLine($"Simulation of UAFA Champions League season {cl.Cl.Season} finished.");
        }

        /// <summary>
        /// Simulates the group stage
        /// </summary>
        /// <param name="teamListFile">team list file</param>
        public void SimulateGroupStage(string teamListFile = null)
        {
            WriteLine("Start simulating group stage.");
            var firstDoing = true;
            do
            {
                if(!firstDoing || String.IsNullOrWhiteSpace(teamListFile))
                {
                    WriteLine("Enter path to team list file:");
                    teamListFile = Read();
                }
                try
                {
                    var teamList = TeamListFileHandler.ReadTeamList(teamListFile, 40);
                    var teamCount = Cl.ReadTeamlist(teamList);
                    if(teamCount != 40)
                    {
                        throw new ArgumentOutOfRangeException(nameof(teamCount), teamCount,
                            @"Teams could not be readed. Wrong file format?");
                    }
                }
                catch(Exception e)
                {
                    WriteEx("Error reading team list", e);
                }
                firstDoing = false;
            } while(Cl.AllTeamsRaw.Count != 40);

            WriteLine("Simulating drawing...");
            do Cl.DrawGroups(); while(Cl.IsGroupsSimulatable != true);

            WriteLine("Simulating groups...");
            Cl.SimulateGroups();

            WriteLine("Group stage simulated.");
        }

        /// <summary>
        /// Simulates the round of 16
        /// </summary>
        public void SimulateRoundOf16()
        {
            WriteLine("Start simulating round of 16.");

            WriteLine("Simulating drawing...");
            do Cl.DrawRoundOf16(); while(Cl.IsRoundOf16Simulatable != true);

            WriteLine("Simulating matches...");
            Cl.SimulateRoundOf16();

            WriteLine("Round of 16 simulated.");
        }

        /// <summary>
        /// Simulates the quarter finals
        /// </summary>
        public void SimulateQuarterFinals()
        {
            WriteLine("Start simulating quarter finals.");

            WriteLine("Simulating drawing...");
            Cl.DrawQuarterFinals();

            WriteLine("Simulating matches...");
            Cl.SimulateQuarterFinals();

            WriteLine("Quarter finals simulated.");
        }

        /// <summary>
        /// Simulates the semi finals
        /// </summary>
        public void SimulateSemiFinals()
        {
            WriteLine("Start simulating semi finals.");

            WriteLine("Simulating drawing...");
            Cl.DrawSemiFinals();

            WriteLine($"Enter team logo for {Cl.SemiFinals[0].TeamA}:");
            Cl.SemiFinals[0].TeamA.Logo = Read();

            WriteLine($"Enter team logo for {Cl.SemiFinals[0].TeamB}:");
            Cl.SemiFinals[0].TeamB.Logo = Read();

            WriteLine($"Enter team logo for {Cl.SemiFinals[1].TeamA}:");
            Cl.SemiFinals[1].TeamA.Logo = Read();

            WriteLine($"Enter team logo for {Cl.SemiFinals[1].TeamB}:");
            Cl.SemiFinals[1].TeamB.Logo = Read();

            WriteLine("Simulating matches...");
            Cl.SimulateSemiFinals();

            WriteLine("Semi finals simulated.");
        }

        /// <summary>
        /// Simulates the final
        /// </summary>
        public void SimulateFinal()
        {
            WriteLine("Start simulating final.");

            WriteLine("Enter name of the final city with flag:");
            var city = Read();

            WriteLine("Enter name of the final stadium:");
            var stadium = Read();

            DateTime date = DateTime.MinValue;
            string dateStr;
            do
            {
                WriteLine("Enter name of the final date:");
                dateStr = Read();
                DateTime.TryParse(dateStr, out date);
            } while(date == DateTime.MinValue);

            WriteLine("Initializing final...");
            Cl.InitFinal(stadium, city, date);

            WriteLine("Simulating final...");
            Cl.SimulateFinal();

            WriteLine($"Final result: {Cl.Final}");

            WriteLine("Final simulated.");
        }

        /// <summary>
        /// Manages the exports 
        /// </summary>
        public void ManageExport()
        {
            WriteLine("Start exporting results.");

            WriteLine("Enter file name for the UAFA Coefficient file or leave blank to skip:");
            var coeffFile = Read();
            if(!String.IsNullOrWhiteSpace(coeffFile))
            {
                WriteLine("Exporting UAFA Coefficient...");
                Cl.ExportCoefficient(coeffFile).RunSynchronously();
            }

            WriteLine("Create wiki page...");
            var exportSucc = ClWikiHandler.CreateWikiPage(Cl);
            WriteLine(exportSucc ? "Export successfully." : "Errors on export. See log file for details.");

            WriteLine("Exporting finished.");
        }

        #endregion

        #region Console I/O Helper Methods for Logging

        /// <summary>
        /// Writes the given text to Console and the logger
        /// </summary>
        public static void WriteLine()
        {
            Console.WriteLine();
        }

        /// <summary>
        /// Writes the given text to Console and the logger
        /// </summary>
        /// <param name="text">The text to write</param>
        public static void WriteLine(string text)
        {
            Console.WriteLine(text);
            SimpleLog.Info(text);
        }

        /// <summary>
        /// Writes the given text to Console and the logger
        /// </summary>
        /// <param name="text">The text to write</param>
        public static void Write(string text)
        {
            Console.Write(text);
            SimpleLog.Info(text);
        }

        /// <summary>
        /// Writes the given exception to Console and the logger
        /// </summary>
        /// <param name="e">The exception</param>
        public static void WriteEx(Exception e)
        {
            WriteEx(String.Empty, e);
        }

        /// <summary>
        /// Writes the given message and exception to Console and the logger
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="e">The exception</param>
        public static void WriteEx(string message, Exception e)
        {
            Console.WriteLine($@"{message}: {e.Message}");
            SimpleLog.Error($@"{message}: {e}");
        }

        /// <summary>
        /// Reads the input to the end of the line
        /// </summary>
        /// <returns>The input</returns>
        public static string Read()
        {
            var r = Console.ReadLine();
            SimpleLog.Info("Input: " + r);
            return r;
        }

        #endregion

    }
}
