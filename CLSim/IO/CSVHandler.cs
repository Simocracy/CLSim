﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simocracy.CLSim.Football.UAFA;
using SimpleLogger;

namespace Simocracy.CLSim.IO
{
    /// <summary>
    /// Handler for CSV files
    /// </summary>
    public class CSVHandler
    {

        #region Coefficients

        /// <summary>
        /// Exports the given <see cref="Coefficient"/> to the given file name
        /// </summary>
        /// <param name="coefficients">The <see cref="Coefficient"/> to export</param>
        /// <param name="season">The season to export</param>
        /// <param name="fileName">The file name</param>
        /// <param name="seperator">Custom csv seperator</param>
        public static async Task WriteCoefficientCSV(IEnumerable<Coefficient> coefficients, string season, string fileName, string seperator = ";")
        {
            var contentSb = new StringBuilder();
            contentSb.Append($"Saison {season}{seperator}{seperator}{seperator}{seperator}{seperator}{seperator}");
            contentSb.Append(
                $"Verein{seperator}{seperator}Anz. Siege{seperator}Anz. Remis{seperator}Champ.League{seperator}Am.League{seperator}Koeff.");
            contentSb.Append(GenerateCoefficientFile(coefficients));
            await SaveCSV(fileName, contentSb.ToString());
        }

        /// <summary>
        /// Creates a CSV file string from the given <see cref="Coefficient"/>
        /// </summary>
        /// <param name="coefficients">The coefficients to export</param>
        /// <param name="seperator">Custom csv seperator</param>
        /// <returns></returns>
        public static string GenerateCoefficientFile(IEnumerable<Coefficient> coefficients, string seperator = ";")
        {
            SimpleLog.Info($"Export {coefficients.Count()} coefficients to csv file format.");

            var sb = new StringBuilder();
            foreach(var coeff in coefficients)
            {
                sb.AppendLine(coeff.ExportAsCSV(seperator));
            }
            return sb.ToString();
        }

        #endregion

        #region File Handling

        /// <summary>
        /// Saves the given content to the given file name
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <param name="content">The content</param>
        public static async Task SaveCSV(string fileName, string content)
        {
            if(!fileName.EndsWith(".csv")) fileName += ".csv";

            SimpleLog.Info($"Write csv file {fileName}.");

            try
            {
                using(var sw = new StreamWriter(fileName))
                {
                    await sw.WriteAsync(content);
                }
            }
            catch(Exception e)
            {
                SimpleLog.Log($"Error writing file {fileName}.{Environment.NewLine}{e}");
            }
        }

        #endregion

    }
}