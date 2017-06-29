using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Simocracy.CLSim.Football.UAFA;
using SimpleLogger;
using System.IO;

namespace Simocracy.CLSim.IO
{
    /// <summary>
    /// Handler for Excel Files for the UAFA Coefficient
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
        /// Finalizer, release the resources
        /// </summary>
        ~ExcelHandler()
        {
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

        #region Methods

        /// <summary>
        /// Initialize a new file
        /// </summary>
        public void Init()
        {
            try
            {       
                ExcelApp = new Application();
                ExcelApp.Visible = true;
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
        /// Writes the headers to the file
        /// </summary>
        /// <param name="season">The current season</param>
        public void WriteHeaders(string season)
        {
            if(!CanUse) return;

            SimpleLog.Info("Write Excel Header Lines.");

            // Season line
            Worksheet.Cells[1, 1] = $"Saison {season}";
            var headRange = Worksheet.Range["A1", "E1"];
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

            var fistEmptyLine = Worksheet.Range["A3", "A1000"].End[XlDirection.xlUp].Row;

            Worksheet.Cells[fistEmptyLine, StateCol] = coeff.Team.State;
            Worksheet.Cells[fistEmptyLine, TeamNameCol] = coeff.Team.Name;
            Worksheet.Cells[fistEmptyLine, WonCol] = coeff.Won;
            Worksheet.Cells[fistEmptyLine, DrawnCol] = coeff.Drawn;
            Worksheet.Cells[fistEmptyLine, CLRoundCol] = coeff.GetReachedCLRoundStr();
            Worksheet.Cells[fistEmptyLine, ALRoundCol] = coeff.GetReachedALRoundStr();
            Worksheet.Cells[fistEmptyLine, PointsCol] = coeff.Points;
        }

        /// <summary>
        /// Saves the file to the fiven file name
        /// </summary>
        /// <param name="fileName">File name for the file</param>
        public void CloseFile(string fileName)
        {
            if(!CanUse) return;

            try
            {
                SimpleLog.Info($"Save Excel coefficient file into {fileName}");
                Workbook.SaveAs(fileName);
            }
            catch(Exception e)
            {
                SimpleLog.Log($"Failed to write file:{Environment.NewLine}{e}");
            }
        }

        #endregion

    }
}
