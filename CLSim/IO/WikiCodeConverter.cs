using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
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
            AlGruppe,

            /// <summary>
            /// 
            /// </summary>
            Gruppentabelle5Kreuz,
        }

        #endregion

        #region Constants

        public const string TeamStateMacPvKey = "zzymacpv";
        public const string TeamStateTvKey = "aaatv";
        public const string TeamStateClRelKeyPrefix = "zzz";

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

            for (int i = 1; i <= table.Rows.Count; i++)
            {
                var row = table.Rows[i - 1];

                var clas = String.Empty;
                if (i <= qual1Count)
                    clas = "class=\"qual1\"";
                else if (i <= qual1Count + qual2Count)
                    clas = "class=\"qual2\"";

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

            switch (template)
            {
                case ELeagueTemplate.AlGruppe:
                    sb.AppendLine(ToWikiCode(league.Table, qual1Count, qual2Count));
                    sb.Append(TemplateAlGroupTable(league));
                    break;
                case ELeagueTemplate.Gruppentabelle5Kreuz:
                    sb.Append(Template5GroupTableCross(league, qual1Count, qual2Count));
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

            sb.AppendLine($"|A{teamNo1}={doubleMatch.TeamA}");
            sb.AppendLine($"|A{teamNo2}={doubleMatch.TeamB}");
            sb.AppendLine($"|A{teamNo1}-A{teamNo2}={doubleMatch.FirstLeg.ResultA}|{doubleMatch.FirstLeg.ResultB}");
            sb.AppendLine($"|A{teamNo2}-A{teamNo1}={doubleMatch.SecondLeg.ResultA}|{doubleMatch.SecondLeg.ResultB}");
            var extraTime = doubleMatch.SecondLeg.IsExtraTime ? "t" : String.Empty;
            sb.AppendLine($"|A{teamNo1}-A{teamNo2}-Verl={extraTime}");
            var p1 = doubleMatch.SecondLeg.IsPenalty ? doubleMatch.PenaltyA.ToString() : String.Empty;
            var p2 = doubleMatch.SecondLeg.IsPenalty ? doubleMatch.PenaltyB.ToString() : String.Empty;
            sb.Append($"|A{teamNo1}-A{teamNo2}-Elfm={p1}|{p2}");

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
            sb.AppendLine($"|Stadion={match.Stadium ?? String.Empty}");
            sb.AppendLine($"|Stadion-Land={match.City ?? String.Empty}");

            var resStr = $"{match.ResultA}:{match.ResultB}";
            var extMatch = match as ExtendedFootballMatch;
            if (extMatch != null && extMatch.IsExtraTime)
                resStr += " n. V.";
            sb.AppendLine($"|Ergebnis={resStr}");

            sb.AppendLine("|Zuschauer=");
            sb.AppendLine($"|Schiri={match.Refere ?? String.Empty}");
            sb.Append("}}");

            return sb.ToString();
        }

        #endregion

        #region Article Tables

        /// <summary>
        /// Builds the participant table sorted by state for UAFA AL.
        /// First state must be last winner team (or filler), last state MAC-PV followed by CL relegation teams.
        /// </summary>
        /// <param name="participants">Participants sorted by state</param>
        /// <param name="startYear">Start year of the AL (4 digits)</param>
        /// <param name="finalYear">Final year of the AL (2 digits)</param>
        /// <returns>Participant table wiki code</returns>
        public static string GetUafaAlParticipantTable(SortedDictionary<string, Dictionary<FootballTeam, Color>> participants, int startYear = 0, int finalYear = 0)
        {
            SimpleLog.Info("Building participant table for UAFA CL/AL.");
            var sb = new StringBuilder();

            sb.AppendLine("{| class=\"wikitable\" style=\"width:100%;\"");
            sb.AppendLine("|-");
            sb.AppendLine("! style=\"width:25%;\" | Meister");
            sb.AppendLine("! style=\"width:25%;\" | Zweitplatzierter");
            sb.AppendLine("! rowspan=\"11\" style=\"width:1%;\" | ");
            sb.AppendLine("! style=\"width:25%;\" | Meister");
            sb.AppendLine("! style=\"width:25%;\" | Zweitplatzierter");
            sb.AppendLine("|-");

            for(int i = 1; i <= participants.Count; i++)
            {
                var state = participants.ElementAt(i-1);
                if (state.Key == TeamStateMacPvKey)
                {
                    sb.AppendLine($"| style=\"background-color:#{state.Value.First().Value.ToString().Substring(3)}; colspan=\"5\" | Verterter Puerta Venturas: {state.Value.First().Key}");
                }
                else if (state.Key == TeamStateTvKey)
                {
                    sb.AppendLine($"| style=\"background-color:#{participants.First().Value.First().Value.ToString().Substring(3)};\" colspan=\"2\" | Titelverteidiger: {participants.First().Value.First().Key}");
                }
                else if (!state.Key.StartsWith(TeamStateClRelKeyPrefix))
                {

                    foreach (var team in state.Value)
                        sb.AppendLine($"| style=\"background-color:#{team.Value.ToString().Substring(3)};\" | {team.Key.GetWikiCodeWithRemarks()}");

                    if (i % 2 == 0)
                        sb.AppendLine("|-");
                }
            }

            sb.AppendLine("|}");
            sb.Append("<small><references group=\"A\" /></small>");

            // add cl relegation teams
            var clRelegations = participants.Where(x => x.Key.StartsWith(TeamStateClRelKeyPrefix));
            var clRels = clRelegations as KeyValuePair<string, Dictionary<FootballTeam, Color>>[] ?? clRelegations.ToArray();
            if (clRels.Any())
            {
                sb.AppendLine();
                sb.AppendLine("{| class=\"wikitable\" style=\"width:100%\"");
                sb.AppendLine($"! colspan=\"4\" | Dritter der Gruppenphase der [[UAFA Champions League " +
                    $"{startYear}/{finalYear}|Champions League]] (Einstieg in der KO-Runde)");
                sb.AppendLine("|-");
                
                for (int i = 1; i <= clRels.Length; i++)
                {
                    var team = clRels.ElementAt(i - 1).Value.First();
                    sb.AppendLine($"| style=\"background-color:#{team.Value.ToString().Substring(3)};\" | {team.Key.GetWikiCodeWithRemarks()}");

                    if (i % 4 == 0)
                        sb.AppendLine("|-");
                }
                sb.Append("|}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the group teams table code for 8 groups with 5 teams
        /// </summary>
        /// <param name="teams">the teams sorted per group</param>
        /// <returns>The team group table</returns>
        public static string GetGroupTeamsTable(IEnumerable<string> teams)
        {
            SimpleLog.Info("Building group teams table.");
            var sb = new StringBuilder();

            var tArr = teams.ToArray();

            sb.AppendLine("{| class=\"wikitable\" style=\"width:100%;\"");
            sb.AppendLine("|-");
            sb.AppendLine("! style=\"width:25%;\" | Gruppe A");
            sb.AppendLine("! style=\"width:25%;\" | Gruppe B");
            sb.AppendLine("! style=\"width:25%;\" | Gruppe C");
            sb.AppendLine("! style=\"width:25%;\" | Gruppe D");
            sb.AppendLine("|-");

            for (int i = 0; i < 5; i++)
            {
                sb.AppendLine($"| {tArr[i]}");
                sb.AppendLine($"| {tArr[i + 5]}");
                sb.AppendLine($"| {tArr[i + 10]}");
                sb.AppendLine($"| {tArr[i + 15]}");
                sb.AppendLine("|-");
            }

            sb.AppendLine("! style=\"width:25%;\" | Gruppe E");
            sb.AppendLine("! style=\"width:25%;\" | Gruppe F");
            sb.AppendLine("! style=\"width:25%;\" | Gruppe G");
            sb.AppendLine("! style=\"width:25%;\" | Gruppe H");
            sb.AppendLine("|-");

            for (int i = 20; i < 25; i++)
            {
                sb.AppendLine($"| {tArr[i]}");
                sb.AppendLine($"| {tArr[i + 5]}");
                sb.AppendLine($"| {tArr[i + 10]}");
                sb.AppendLine($"| {tArr[i + 15]}");
                sb.AppendLine("|-");
            }

            sb.Append("|}");

            return sb.ToString();
        }

        #endregion

        #region Group Template Converting

        /// <summary>
        /// Creates the match code with the template Vorlage:AL-Gruppe
        /// </summary>
        /// <param name="league">league</param>
        /// <returns>the wiki code</returns>
        private static string TemplateAlGroupTable(FootballLeague league)
        {
            SimpleLog.Info($"Create the match code using Vorlage:AL-Gruppe for league {league.ID}.");

            if (league.TeamCount != 5)
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

            for (int ta = 1; ta < 5; ta++)
                for (int tb = ta + 1; tb <= 5; tb++)
                {
                    sb.Append($"|A{ta}-A{tb}={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[ta] && m.TeamB == league.Teams[tb])?.FullResultStr ?? String.Empty}\t");
                    sb.AppendLine($"|A{tb}-A{ta}={league.Matches.FirstOrDefault(m => m.TeamA == league.Teams[tb] && m.TeamB == league.Teams[ta])?.FullResultStr ?? String.Empty}");
                }

            sb.Append("}}");

            return sb.ToString();
        }

        /// <summary>
        /// Creates the match code with the template Vorlage:5er-Gruppentabelle Kreuz (default code)
        /// </summary>
        /// <param name="league">league</param>
        /// <param name="qual1Count">qual1 count</param>
        /// <param name="qual2Count">qual2 count</param>
        /// <returns>the wiki code</returns>
        private static string Template5GroupTableCross(FootballLeague league, int qual1Count = 2, int qual2Count = 1)
        {
            SimpleLog.Info($"Create the match code using Vorlage:5er-Gruppentabelle Kreuz for league {league.ID}.");

            if (league.TeamCount != 5)
            {
                SimpleLog.Error($"Cannot create match code using Vorlage:5er-Gruppentabelle Kreuz with {league.TeamCount} teams, need 5.");
                return String.Empty;
            }

            var sb = new StringBuilder();

            sb.AppendLine("{{5er-Gruppentabelle Kreuz");
            sb.AppendLine($"|A={league.Teams[0].Name}|A-fl={league.Teams[0].State}");
            sb.AppendLine($"|B={league.Teams[1].Name}|B-fl={league.Teams[1].State}");
            sb.AppendLine($"|C={league.Teams[2].Name}|C-fl={league.Teams[2].State}");
            sb.AppendLine($"|D={league.Teams[3].Name}|D-fl={league.Teams[3].State}");
            sb.AppendLine($"|E={league.Teams[4].Name}|E-fl={league.Teams[4].State}");

            for(int ta = 0; ta < 5; ta++)
                for(int tb = 0; tb < 5; tb++)
                    if(ta != tb)
                    {
                        var match = league.Matches.FirstOrDefault(
                            m => m.TeamA == league.Teams[ta] && m.TeamB == league.Teams[tb]);
                        sb.AppendLine($"|{Convert.ToChar(ta + 'A')}-{Convert.ToChar(tb + 'A')}={match?.ResultA}|{match?.ResultB}");
                    }

            for(int i = 1; i <= 5; i++)
            {
                sb.Append($"|c{i}=");
                if(i <= qual1Count)
                    sb.Append("Auf");
                else if(i <= qual1Count + qual2Count)
                    sb.Append("PO");
            }
            sb.AppendLine();
            sb.Append("}}");

            return sb.ToString();
        }

        #endregion
    }
}
