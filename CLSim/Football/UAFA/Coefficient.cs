using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simocracy.CLSim.Football.Base;
using Simocracy.CLSim.IO;
using SimpleLogger;

namespace Simocracy.CLSim.Football.UAFA
{
    /// <summary>
    /// Represents the UAFA Coefficient for one team
    /// </summary>
    public class Coefficient : INotifyPropertyChanged
    {

        #region Members

        private FootballTeam _Team;

        private int _Won;
        private int _Drawn;
        private HashSet<ETournamentRound> _RoundsPlayed;

        private int _Points;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance for the given team
        /// </summary>
        /// <param name="team">The football team</param>
        public Coefficient(FootballTeam team)
        {
            Team = team;
            Points = -1;
            RoundsPlayed = new HashSet<ETournamentRound>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The team
        /// </summary>
        public FootballTeam Team
        {
            get => _Team;
            private set
            {
                _Team = value;
                Notify();
            }
        }

        /// <summary>
        /// Matches Won
        /// </summary>
        public int Won
        {
            get => _Won;
            set
            {
                _Won = value;
                Notify();
            }
        }

        /// <summary>
        /// Matches Drawn
        /// </summary>
        public int Drawn
        {
            get => _Drawn;
            set
            {
                _Drawn = value;
                Notify();
            }
        }

        /// <summary>
        /// All played rounds
        /// </summary>
        public HashSet<ETournamentRound> RoundsPlayed
        {
            get => _RoundsPlayed;
            private set
            {
                _RoundsPlayed = value;
                Notify();
            }
        }

        /// <summary>
        /// Coefficient of the Team for the current season
        /// </summary>
        public int Points
        {
            get
            {
                if(_Points < 0)
                    CalculatePoints();
                return _Points;
            }
            private set
            {
                _Points = value;
                Notify();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the latest Champions League round
        /// </summary>
        /// <returns>Reached Champions League round</returns>
        public ETournamentRound GetReachedCLRound()
        {
            var played = RoundsPlayed.Where(r => r.ToString().StartsWith("CL"));
            var eTournamentRounds = played as ETournamentRound[] ?? played.ToArray();
            return eTournamentRounds.Length < 1 ? ETournamentRound.None : eTournamentRounds.Max();
        }

        /// <summary>
        /// Returns the latest Champions League round as export string
        /// </summary>
        /// <returns>Reached Champions League round as string</returns>
        public string GetReachedCLRoundStr()
        {
            switch(GetReachedCLRound())
            {
                case ETournamentRound.CLQualification:
                    return "Quali";
                case ETournamentRound.CLGroupStage:
                    return "Gruppe";
                case ETournamentRound.CLRoundOf16:
                    return "Achtelfinale";
                case ETournamentRound.CLQuarterFinals:
                    return "Viertelfinale";
                case ETournamentRound.CLSemiFinals:
                    return "Halbfinale";
                case ETournamentRound.CLFinal:
                    return "Finale";
                default:
                    return "-";
            }
        }

        /// <summary>
        /// Returns the latest America League round
        /// </summary>
        /// <returns>Reached America League round</returns>
        public ETournamentRound GetReachedALRound()
        {
            var played = RoundsPlayed.Where(r => r.ToString().StartsWith("AL"));
            var eTournamentRounds = played as ETournamentRound[] ?? played.ToArray();
            return eTournamentRounds.Length < 1 ? ETournamentRound.None : eTournamentRounds.Max();
        }

        /// <summary>
        /// Returns the latest America League round as export string
        /// </summary>
        /// <returns>Reached America League round as string</returns>
        public string GetReachedALRoundStr()
        {
            switch(GetReachedALRound())
            {
                case ETournamentRound.ALQualification:
                    return "Quali";
                case ETournamentRound.ALGroupStage:
                    return "Gruppe";
                case ETournamentRound.ALRoundOf32:
                    return "Sechz.final.";
                case ETournamentRound.ALRoundOf16:
                    return "Achtelfinale";
                case ETournamentRound.ALQuarterFinals:
                    return "Viertelfinale";
                case ETournamentRound.ALSemiFinals:
                    return "Halbfinale";
                case ETournamentRound.ALFinal:
                    return "Finale";
                default:
                    return "-";
            }
        }

        public override string ToString()
        {
            return $"Coefficient: Team={Team.FullName}, Points={Points}";
        }

        #endregion

        #region Calculation

        /// <summary>
        /// Adds the given <see cref="DoubleMatch"/> from the given round to the match list
        /// </summary>
        /// <param name="round">Tournament round</param>
        /// <param name="dMatch"><see cref="DoubleMatch"/> to add</param>
        public void AddMatch(ETournamentRound round, DoubleMatch dMatch)
        {
            AddMatch(round, dMatch.FirstLeg);
            AddMatch(round, dMatch.SecondLeg);
        }

        /// <summary>
        /// Adds the given match from the given round to the match list
        /// </summary>
        /// <param name="round">Tournament round</param>
        /// <param name="match">Match to add</param>
        public void AddMatch(ETournamentRound round, FootballMatch match)
        {
            RoundsPlayed.Add(round);
            if(match.ResultA == match.ResultB) Drawn++;
            else if(match.TeamA == Team && match.ResultA > match.ResultB) Won++;
            else if(match.TeamB == Team && match.ResultA < match.ResultB) Won++;
        }

        /// <summary>
        /// Calculates the coefficient points
        /// </summary>
        public void CalculatePoints()
        {
            Points = Won * 2 + Drawn;
            foreach(var round in RoundsPlayed)
            {
                switch(round)
                {
                    case ETournamentRound.CLGroupStage:
                        Points += 4;
                        break;
                    case ETournamentRound.CLRoundOf16:
                        Points += 5;
                        break;
                    case ETournamentRound.CLQuarterFinals:
                        Points += 2;
                        break;
                    case ETournamentRound.CLSemiFinals:
                        Points += 2;
                        break;
                    case ETournamentRound.CLFinal:
                        Points += 2;
                        break;
                    case ETournamentRound.ALGroupStage:
                        if(Points < 2) Points = 2;
                        break;
                    case ETournamentRound.ALRoundOf32:
                        Points += 1;
                        break;
                    case ETournamentRound.ALRoundOf16:
                        Points += 1;
                        break;
                    case ETournamentRound.ALQuarterFinals:
                        Points += 1;
                        break;
                    case ETournamentRound.ALSemiFinals:
                        Points += 1;
                        break;
                    case ETournamentRound.ALFinal:
                        Points += 1;
                        break;
                }
            }
        }

        #endregion

        #region Export

        /// <summary>
        /// Exports the coefficient into a csv string.
        /// Format: <see cref="FootballTeam.State"/>; <see cref="FootballTeam.Name"/>; <see cref="Won"/>;
        ///     <see cref="Drawn"/>; <see cref="GetReachedCLRound"/>; <see cref="GetReachedALRound"/>; <see cref="Points"/>
        /// </summary>
        /// <param name="seperator">CSV seperator</param>
        public string GetCsvString(string seperator = ";")
        {
            return
                $"{Team.State}{seperator} {Team.Name}{seperator} {Won}{seperator} {Drawn}{seperator} " +
                $"{GetReachedCLRoundStr()}{seperator} {GetReachedALRoundStr()}{seperator} {Points}";
        }

        /// <summary>
        /// Exports the given coefficients as xlsx, or if not successfully as csv
        /// </summary>
        /// <param name="coeffs">Coefficient list</param>
        /// <param name="season">season to export</param>
        /// <param name="fileName">file name without extension</param>
        public static async Task<bool> Export(IEnumerable<Coefficient> coeffs, string season, string fileName)
        {
            // todo: better export syntax
            // Try export as xlsx
            var isXlsSuccess = await ExcelHandler.ExportCoefficientsAsync(coeffs, season, fileName);
            bool isCsvSucces = false;
            if (!isXlsSuccess)
            {
                // If not successfull: csv
                SimpleLog.Info(
                    $"Export UAFA Coefficient for season {season} as Excel File failed. Exporting as CSV.");

                if (fileName.EndsWith("xlsx"))
                    fileName = fileName.Replace(".xlsx", ".csv");

                isCsvSucces = await CSVHandler.ExportCoefficient(coeffs, season, fileName);
            }

            return isXlsSuccess || isCsvSucces;
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Observer-Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Observer
        /// </summary>
        /// <param name="propertyName">Property</param>
        protected void Notify([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
