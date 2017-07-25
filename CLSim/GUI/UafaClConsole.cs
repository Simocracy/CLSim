using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simocracy.CLSim.Football.UAFA;
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
        public static void Simulate()
        {
            WriteLine("UAFA Champions League simulation.");
            WriteLine();
            Write("Simulated season: ");
            var season = Read();
            var cl = new UafaClConsole(season);
            WriteLine($"UAFA Champions League season {cl.Cl.Season} created.");

            cl.SimulateGroupStage();
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
        public void SimulateGroupStage()
        {
            
        }

        /// <summary>
        /// Simulates the round of 16
        /// </summary>
        public void SimulateRoundOf16()
        {

        }

        /// <summary>
        /// Simulates the quarter finals
        /// </summary>
        public void SimulateQuarterFinals()
        {

        }

        /// <summary>
        /// Simulates the semi finals
        /// </summary>
        public void SimulateSemiFinals()
        {

        }

        /// <summary>
        /// Simulates the final
        /// </summary>
        public void SimulateFinal()
        {

        }

        /// <summary>
        /// Manages the exports 
        /// </summary>
        public void ManageExport()
        {

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
