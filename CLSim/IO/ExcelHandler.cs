using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Simocracy.CLSim.Football.UAFA;
using SimpleLogger;

namespace Simocracy.CLSim.IO
{
    /// <summary>
    /// Handler for Excel Files for the UAFA Coefficient.
    /// Works only if MS Excel is installed on the local machine.
    /// </summary>
    public class ExcelHandler
    {

        #region Consts

        public const int StateCol = 1;
        public const int TeamNameCol = 2;
        public const int WonCol = 3;
        public const int DrawnCol = 4;
        public const int CLRoundCol = 5;
        public const int ALRoundCol = 6;
        public const int PointsCol = 7;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Instance for a new File and initialize it
        /// </summary>
        public ExcelHandler()
        {
            Init();
        }

        /// <summary>
        /// Finalizer, close excel and release the resources
        /// </summary>
        ~ExcelHandler()
        {
            if(CanUse)
            {
                ExcelApp.DisplayAlerts = false;
                ExcelApp.Quit();
            }

            ExcelApp = null;
            Workbook = null;
            Worksheet = null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The current Excel Application
        /// </summary>
        public Application ExcelApp { get; set; }

        /// <summary>
        /// The current Excel Workbook
        /// </summary>
        public Workbook Workbook { get; set; }

        /// <summary>
        /// The current Excel Worksheet
        /// </summary>
        public Worksheet Worksheet { get; set; }

        /// <summary>
        /// Indicates if Excel can be used
        /// </summary>
        public bool CanUse => ExcelApp != null && Workbook != null && Worksheet != null;

        #endregion

        #region Main Methods

        /// <summary>
        /// Initialize a new file
        /// </summary>
        public void Init()
        {
            try
            {       
                ExcelApp = new Application();
                //ExcelApp.Visible = true;
                ExcelApp.DisplayAlerts = false;
                Workbook = ExcelApp.Workbooks.Add(1);
                Worksheet = (Worksheet)Workbook.Sheets[1];
            }
            catch (Exception e)
            {
                SimpleLog.Error("Excel export cannot be used. Is MS Excel installed on your machine?");
                SimpleLog.Error(e.ToString());
            }
        }

        /// <summary>
        /// Saves the file to the fiven file name and returns true, if file saved succesfully.
        /// </summary>
        /// <param name="fileName">File name for the file</param>
        /// <returns>True if file saved successfully</returns>
        public bool Close(string fileName)
        {
            if(!CanUse) return false;

            if(!fileName.EndsWith(".xlsx")) fileName += ".xlsx";
            bool retVal = false;

            try
            {
                SimpleLog.Info($"Save Excel file \"{fileName}\".");
                Workbook.SaveAs(Filename: fileName);
                ExcelApp.Quit();
                retVal = true;
                SimpleLog.Info($"Excel file \"{fileName}\" saved.");
            }
            catch(Exception e)
            {
                SimpleLog.Log($"Failed to write file \"{fileName}\".");
                SimpleLog.Error(e.ToString());
            }
            return retVal;
        }

        #endregion

        #region Coefficient

        /// <summary>
        /// Exports the given <see cref="Coefficient"/> to the given file name asynchroniously
        /// </summary>
        /// <param name="coefficients">The <see cref="Coefficient"/> to export</param>
        /// <param name="season">The season to export</param>
        /// <param name="fileName">The file name</param>
        /// <returns></returns>
        public static async Task<bool> ExportCoefficientsAsync(IEnumerable<Coefficient> coefficients, string season,
                                              string fileName)
        {
            return await Task.Run(() => ExportCoefficients(coefficients, season, fileName));
        }

        /// <summary>
        /// Exports the given <see cref="Coefficient"/> to the given file name
        /// </summary>
        /// <param name="coefficients">The <see cref="Coefficient"/> to export</param>
        /// <param name="season">The season to export</param>
        /// <param name="fileName">The file name</param>
        /// <returns></returns>
        public static bool ExportCoefficients(IEnumerable<Coefficient> coefficients, string season,
                                              string fileName)
        {
            var coeffs = coefficients as Coefficient[] ?? coefficients.ToArray();

            SimpleLog.Info($"Export {coeffs.Length} coefficients to Excel file \"{fileName}\".");

            var handler = new ExcelHandler();
            if(!handler.CanUse)
                return false;
            handler.WriteHeaders(season);
            foreach(var coeff in coeffs)
                handler.WriteTeamCoefficientLine(coeff);
            return handler.Close(fileName);
        }

        /// <summary>
        /// Writes the coefficient headers to the file
        /// </summary>
        /// <param name="season">The current season</param>
        public void WriteHeaders(string season)
        {
            if(!CanUse) return;

            SimpleLog.Info("Write Excel Header Lines.");

            // Season line
            Worksheet.Cells[1, 1] = $"Saison {season}";
            var headRange = Worksheet.Range["A1", "G1"];
            headRange.Merge();

            // Coefficient Header
            Worksheet.Cells[2, StateCol] = "Verein";
            var teamRange = Worksheet.Range["A2", "B2"];
            teamRange.Merge();
            
            Worksheet.Cells[2, WonCol] = "Anz. Siege";
            Worksheet.Cells[2, DrawnCol] = "Anz. Remis";
            Worksheet.Cells[2, CLRoundCol] = "Champ.League";
            Worksheet.Cells[2, ALRoundCol] = "Am.League";
            Worksheet.Cells[2, PointsCol] = "Koeff.";
        }

        /// <summary>
        /// Writes the given coefficient to the file
        /// </summary>
        /// <param name="coeff">The coefficient to write</param>
        public void WriteTeamCoefficientLine(Coefficient coeff)
        {
            if(!CanUse) return;

            SimpleLog.Info($"Add coefficient {coeff} to Excel file.");

            var xlRange = (Range)Worksheet.Cells[Worksheet.Rows.Count, 1];
            long lastRow = xlRange.End[XlDirection.xlUp].Row;
            long newRow = lastRow + 1;

            Worksheet.Cells[newRow, StateCol] = coeff.Team.State;
            Worksheet.Cells[newRow, TeamNameCol] = coeff.Team.Name;
            Worksheet.Cells[newRow, WonCol] = coeff.Won;
            Worksheet.Cells[newRow, DrawnCol] = coeff.Drawn;
            Worksheet.Cells[newRow, CLRoundCol] = coeff.GetReachedClRoundStr();
            Worksheet.Cells[newRow, ALRoundCol] = coeff.GetReachedAlRoundStr();
            Worksheet.Cells[newRow, PointsCol] = coeff.Points;
        }

        #endregion

    }
}
