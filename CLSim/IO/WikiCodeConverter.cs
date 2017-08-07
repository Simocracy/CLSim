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

            return sb.ToString();
        }

        public static string ToWikiCode()
        {
            SimpleLog.Info($"Convert football league to wiki code.");
            var sb = new StringBuilder();



            return sb.ToString();
        }

        #endregion

    }
}
