using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            // qual1 rows
            for(int i = 1; i <= qual1Count; i++)
            {
                if(i >= table.Rows.Count) break;

                var row = table.Rows[i - 1];

                sb.AppendLine("|- class=\"qual1\"");
                sb.AppendLine($"| '''{i}'''");
                sb.AppendLine($"| style=\"text-align: left;\" | {row[LeagueTable.TeamRow]}");
                sb.AppendLine($"| {row[LeagueTable.MatchCountRow]} |" +
                              $"| {row[LeagueTable.WinCountRow]} |" +
                              $"| {row[LeagueTable.DrawnCountRow]} |" +
                              $"| {row[LeagueTable.LoseCountRow]} |" +
                              $"| {row[LeagueTable.GoalsRow]} |" +
                              $"| {row[LeagueTable.PointsRow]}");
            }

            // qual2 rows
            for(int i = qual1Count + 1; i <= qual1Count + qual2Count; i++)
            {
                if(i >= table.Rows.Count) break;

                var row = table.Rows[i - 1];

                sb.AppendLine("|- class=\"qual2\"");
                sb.AppendLine($"| '''{i}'''");
                sb.AppendLine($"| style=\"text-align: left;\" | {row[LeagueTable.TeamRow]}");
                sb.AppendLine($"| {row[LeagueTable.MatchCountRow]} |" +
                              $"| {row[LeagueTable.WinCountRow]} |" +
                              $"| {row[LeagueTable.DrawnCountRow]} |" +
                              $"| {row[LeagueTable.LoseCountRow]} |" +
                              $"| {row[LeagueTable.GoalsRow]} |" +
                              $"| {row[LeagueTable.PointsRow]}");
            }

            // other rows
            for(int i = qual1Count + qual2Count + 1; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i - 1];

                sb.AppendLine("|-");
                sb.AppendLine($"| '''{i}'''");
                sb.AppendLine($"| style=\"text-align: left;\" | {row[LeagueTable.TeamRow]}");
                sb.AppendLine($"| {row[LeagueTable.MatchCountRow]} |" +
                              $"| {row[LeagueTable.WinCountRow]} |" +
                              $"| {row[LeagueTable.DrawnCountRow]} |" +
                              $"| {row[LeagueTable.LoseCountRow]} |" +
                              $"| {row[LeagueTable.GoalsRow]} |" +
                              $"| {row[LeagueTable.PointsRow]}");
            }

            return sb.ToString().Trim();
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
