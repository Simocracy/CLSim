using System;
using System.Linq;
using System.Text;
using Simocracy.CLSim.Football.Base;
using SimpleLogger;

namespace Simocracy.CLSim.IO
{
    /// <summary>
    /// Provides service methods for conversions to wiki code
    /// </summary>
    public static class WikiCodeConverter
    {

        #region ELeagueTemplate

        /// <summary>
        /// Wiki templates for groups
        /// </summary>
        public enum ELeagueTemplate
        {
            /// <summary>
            /// No match output
            /// </summary>
            None,

            /// <summary>
            /// Vorlage:AL-Gruppe
            /// </summary>
            AlGruppe
        }

        #endregion

        #region Football Base Classes

        /// <summary>
        /// Gets the wiki code for <see cref="LeagueTable"/>
        /// </summary>
        /// <param name="table">The table</param>
        /// <param name="qual1Count">qual1 count</param>
        /// <param name="qual2Count">qual2 count</param>
        /// <returns></returns>
        public static string ToWikiCode(LeagueTable table, int qual1Count = 2, int qual2Count = 1)
        {
            SimpleLog.Info($"Convert league table {table.TableName} to wiki code.");

            var sb = new StringBuilder();

            sb.AppendLine("{| class=\"wikitable\" style=\"width: 65%; text-align: center; background-color: white;\"");
            sb.AppendLine("|-");
            sb.AppendLine("! style=\"width: 10%;\" | Pl.");
            sb.AppendLine("! style=\"width: 50%;\" | Team");
            sb.AppendLine("! style=\"width: 5%;\" | Sp.");
            sb.AppendLine("! style=\"width: 5%;\" | S");
            sb.AppendLine("! style=\"width: 5%;\" | U");
            sb.AppendLine("! style=\"width: 5%;\" | N");
            sb.AppendLine("! style=\"width: 12%;\" | Tore");
            sb.AppendLine("! style=\"width: 8%;\" | Pkt.");

            for(int i = 1; i <= table.Rows.Count; i++)
            {
                var row = table.Rows[i - 1];

                var clas = String.Empty;
                if(i <= qual1Count)
                    clas = "class\"=qual1\"";
                else if(i <= qual1Count + qual2Count)
                    clas = "class\"=qual2\"";

                sb.AppendLine($"|- {clas}");
                sb.Append($"| '''{i}''' |");
                sb.AppendLine($"| style=\"text-align: left;\" | {row[LeagueTable.TeamRow]}");
                sb.Append($"| {row[LeagueTable.MatchCountRow]} |");
                sb.Append($"| {row[LeagueTable.WinCountRow]} |");
                sb.Append($"| {row[LeagueTable.DrawnCountRow]} |");
                sb.Append($"| {row[LeagueTable.LoseCountRow]} |");
                sb.Append($"| {row[LeagueTable.GoalsRow]} |");
                sb.AppendLine($"| '''{row[LeagueTable.PointsRow]}'''");
            }

            sb.Append("|}");

            return sb.ToString();
        }

        /// <summary>
        /// Creates the wiki code for the given group with the given match template
        /// </summary>
        /// <param name="league">league</param>
        /// <param name="template">match template</param>
        /// <param name="qual1Count">qual1 count</param>
        /// <param name="qual2Count">qual2 count</param>
        /// <returns>the wiki code for the group</returns>
        public static string ToWikiCode(FootballLeague league, ELeagueTemplate template, int qual1Count = 2, int qual2Count = 1)
        {
            SimpleLog.Info($"Convert football league {league.ID} to wiki code.");
            var sb = new StringBuilder();

            sb.AppendLine(ToWikiCode(league.Table, qual1Count, qual2Count));

            switch(template)
            {
                case ELeagueTemplate.AlGruppe:
                    sb.Append(AlGroup(league));
                    break;
            }

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Converts the double match to wiki code for using Vorlage:K.O.-Runde
        /// </summary>
        /// <param name="doubleMatch"><see cref="DoubleMatch"/> to export</param>
        /// <param name="teamNo1">Number for team 1 for the template</param>
        /// <param name="teamNo2">Number for team 2 for the template</param>
        /// <returns>The wiki code</returns>
        public static string ToWikiCode(DoubleMatch doubleMatch, int teamNo1, int teamNo2)
        {
            SimpleLog.Info($"Convert double match {doubleMatch} to wiki code.");
            var sb = new StringBuilder();

            sb.AppendLine($"|A1={doubleMatch.TeamA}");
            sb.AppendLine($"|A2={doubleMatch.TeamB}");
            sb.AppendLine($"|A1-A2={doubleMatch.FirstLeg.ResultA}|{doubleMatch.FirstLeg.ResultB}");
            sb.AppendLine($"|A2-A1={doubleMatch.SecondLeg.ResultA}|{doubleMatch.SecondLeg.ResultB}");
            var extraTime = doubleMatch.SecondLeg.IsExtraTime ? "t" : String.Empty;
            sb.AppendLine($"|A1-A2-Verl={extraTime}");
            var p1 = doubleMatch.SecondLeg.IsPenalty ? doubleMatch.PenaltyA.ToString() : String.Empty;
            var p2 = doubleMatch.SecondLeg.IsPenalty ? doubleMatch.PenaltyB.ToString() : String.Empty;
            sb.Append($"|A1-A2-Elfm={p1}|{p2}");

            return sb.ToString();
        }

        /// <summary>
        /// Converts the football match to wiki code using Vorlage:Turnierfinale
        /// </summary>
        /// <param name="match"><see cref="FootballMatch"/> to export</param>
        /// <returns>The wiki code</returns>
        public static string ToWikiCode(FootballMatch match)
        {
            SimpleLog.Info($"Convert football match {match} to wiki code.");
            var sb = new StringBuilder();

            sb.AppendLine("{{Turnierfinale");
            sb.AppendLine($"|Team1={match.TeamA.Name}");
            sb.AppendLine($"|Logo1={match.TeamA.Logo}");
            sb.AppendLine("|Tore1=");
            sb.AppendLine("|Aufstellung1=");
            sb.AppendLine($"|Team2={match.TeamB.Name}");
            sb.AppendLine($"|Logo2={match.TeamB.Logo}");
            sb.AppendLine("|Tore2=");
            sb.AppendLine("|Aufstellung2=");

            var date = match.Date != DateTime.MinValue ? match.Date.ToShortDateString() : String.Empty;
            sb.AppendLine($"|Datum={date}");
            sb.AppendLine($"Stadion={match.Stadium ?? String.Empty}");
            sb.AppendLine($"Stadion-Land={match.City ?? String.Empty}");

            var resStr = $"{match.ResultA}:{match.ResultB}";
            var extMatch = match as ExtendedFootballMatch;
            if(extMatch != null && extMatch.IsExtraTime)
                resStr += " n. V.";
            sb.AppendLine($"Ergebnis={resStr}");

            sb.AppendLine("Zuschauer=");
            sb.AppendLine($"Schiri={match.Refere ?? String.Empty}");
            sb.Append("}}");

            return sb.ToString();
        }

        #endregion

        #region Group Template Converting

        /// <summary>
        /// Creates the match code with the template Vorlage:AL-Gruppe
        /// </summary>
        /// <param name="league">league</param>
        /// <returns>the wiki code</returns>
        private static string AlGroup(FootballLeague league)
        {
            SimpleLog.Info($"Create the match code using Vorlage:AL-Gruppe for league {league.ID}.");

            if(league.TeamCount != 5)
            {
                SimpleLog.Error($"Cannot create match code using Vorlage:AL-Gruppe with {league.TeamCount} teams, need 5.");
                return String.Empty;
            }

            var sb = new StringBuilder();

            sb.AppendLine("{{AL-Gruppe");
            sb.AppendLine($"|A1={league.Teams[0]}");
            sb.AppendLine($"|A2={league.Teams[1]}");
            sb.AppendLine($"|A3={league.Teams[2]}");
            sb.AppendLine($"|A4={league.Teams[3]}");
            sb.AppendLine($"|A5={league.Teams[4]}");

            sb.Append($"|A1-A2={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[0] && m.TeamB == league.Teams[1])}\t");
            sb.AppendLine($"|A2-A1={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[1] && m.TeamB == league.Teams[0])}");
            sb.Append($"|A3-A4={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[2] && m.TeamB == league.Teams[3])}\t");
            sb.AppendLine($"|A4-A3={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[3] && m.TeamB == league.Teams[2])}");
            sb.Append($"|A2-A3={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[1] && m.TeamB == league.Teams[2])}\t");
            sb.AppendLine($"|A3-A2={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[2] && m.TeamB == league.Teams[1])}");
            sb.Append($"|A4-A5={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[3] && m.TeamB == league.Teams[4])}\t");
            sb.AppendLine($"|A5-A4={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[4] && m.TeamB == league.Teams[3])}");
            sb.Append($"|A5-A1={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[4] && m.TeamB == league.Teams[0])}\t");
            sb.AppendLine($"|A1-A5={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[0] && m.TeamB == league.Teams[4])}");
            sb.Append($"|A4-A2={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[3] && m.TeamB == league.Teams[1])}\t");
            sb.AppendLine($"|A2-A4={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[1] && m.TeamB == league.Teams[3])}");
            sb.Append($"|A3-A1={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[2] && m.TeamB == league.Teams[0])}\t");
            sb.AppendLine($"|A1-A3={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[0] && m.TeamB == league.Teams[2])}");
            sb.Append($"|A2-A5={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[1] && m.TeamB == league.Teams[4])}\t");
            sb.AppendLine($"|A5-A2={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[4] && m.TeamB == league.Teams[1])}");
            sb.Append($"|A1-A4={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[0] && m.TeamB == league.Teams[3])}\t");
            sb.AppendLine($"|A4-A1={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[3] && m.TeamB == league.Teams[0])}");
            sb.Append($"|A5-A3={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[4] && m.TeamB == league.Teams[2])}\t");
            sb.AppendLine($"|A3-A5={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[2] && m.TeamB == league.Teams[4])}");

            sb.Append("}}");

            return sb.ToString();
        }

        #endregion

    }
}
