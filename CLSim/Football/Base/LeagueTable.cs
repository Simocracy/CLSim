using System.Collections.Generic;
using System.Data;
using System.Linq;
using SimpleLogger;

namespace Simocracy.CLSim.Football.Base
{
    /// <summary>
    /// Table of <see cref="FootballLeague"/>
    /// </summary>
    [System.ComponentModel.DesignerCategory("Code")]
    public class LeagueTable : DataTable
    {
        #region Constants

        public const string TeamRow = "Team";
        public const string MatchCountRow = "Matches";
        public const string WinCountRow = "Win";
        public const string DrawnCountRow = "Drawn";
        public const string LoseCountRow = "Lose";
        public const string GoalsForCountRow = "GoalsFor";
        public const string GoalsAgainstCountRow = "GoalsAgainst";
        public const string GoalDiffRow = "GoalDiff";
        public const string PointsRow = "Points";

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new table
        /// </summary>
        public LeagueTable()
        {
            CreateTable();
        }

        /// <summary>
        /// Creates a new table and calculates it
        /// </summary>
        /// <param name="teams">The teams of the league</param>
        /// <param name="matches">The matches of the league</param>
        public LeagueTable(IEnumerable<FootballTeam> teams, IEnumerable<FootballMatch> matches) : this()
        {
            CalculateTable(teams, matches);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Team on Position 1
        /// </summary>
        public FootballTeam Pos1 => Rows[0][TeamRow] as FootballTeam;

        /// <summary>
        /// Team on Position 2
        /// </summary>
        public FootballTeam Pos2 => Rows[1][TeamRow] as FootballTeam;

        /// <summary>
        /// Team on Position 3
        /// </summary>
        public FootballTeam Pos3 => Rows[2][TeamRow] as FootballTeam;

        /// <summary>
        /// Team on Position 4
        /// </summary>
        public FootballTeam Pos4 => Rows[3][TeamRow] as FootballTeam;

        /// <summary>
        /// Team on Position 5
        /// </summary>
        public FootballTeam Pos5 => Rows[4][TeamRow] as FootballTeam;

        #endregion

        #region Methods

        /// <summary>
        /// Calculates the Table
        /// </summary>
        /// <param name="teams">The teams of the league</param>
        /// <param name="matches">The matches of the league</param>
        public void CalculateTable(IEnumerable<FootballTeam> teams, IEnumerable<FootballMatch> matches)
        {
            var footballTeams = teams as FootballTeam[] ?? teams.ToArray();
            var footballMatches = matches as FootballMatch[] ?? matches.ToArray();

            SimpleLog.Info($"Calculate Table with {footballTeams.Length} teams and {footballMatches.Length} matches.");

            var subTable = CalculateSubTable(footballTeams, footballMatches);
            foreach(DataRow row in subTable)
                Rows.Add(row);

            SortFootballTable(footballMatches);
        }

        /// <summary>
        /// Creates table
        /// </summary>
        private void CreateTable()
        {
            Columns.Add(TeamRow, typeof(FootballTeam));
            Columns.Add(MatchCountRow, typeof(int));
            Columns.Add(WinCountRow, typeof(int));
            Columns.Add(DrawnCountRow, typeof(int));
            Columns.Add(LoseCountRow, typeof(int));
            Columns.Add(GoalsForCountRow, typeof(int));
            Columns.Add(GoalsAgainstCountRow, typeof(int));
            Columns.Add(GoalDiffRow, typeof(int));
            Columns.Add(PointsRow, typeof(int));
        }

        /// <summary>
        /// Calculates the subtable
        /// </summary>
        /// <param name="teams">Teams of subtable</param>
        /// <param name="matches">Matches of Subtable</param>
        /// <returns>The subtable</returns>
        private List<DataRow> CalculateSubTable(FootballTeam[] teams, FootballMatch[] matches)
        {
            List<DataRow> rows = new List<DataRow>(teams.Length);

            foreach(var team in teams)
            {
                var row = NewRow();

                int drawn, lose, goalsFor, goalsAgainst;
                var win = drawn = lose = goalsFor = goalsAgainst = 0;

                foreach(var match in matches)
                {
                    if(match.TeamA == team)
                    {
                        if(match.ResultA > match.ResultB)
                            win++;
                        else if(match.ResultA == match.ResultB)
                            drawn++;
                        else
                            lose++;

                        goalsFor += match.ResultA;
                        goalsAgainst += match.ResultB;
                    }
                    else if(match.TeamB == team)
                    {
                        if(match.ResultB > match.ResultA)
                            win++;
                        else if(match.ResultB == match.ResultA)
                            drawn++;
                        else
                            lose++;

                        goalsFor += match.ResultB;
                        goalsAgainst += match.ResultA;
                    }
                }

                row[TeamRow] = team;
                row[MatchCountRow] = win + drawn + lose;
                row[WinCountRow] = win;
                row[DrawnCountRow] = drawn;
                row[LoseCountRow] = lose;
                row[GoalsForCountRow] = goalsFor;
                row[GoalsAgainstCountRow] = goalsAgainst;
                row[GoalDiffRow] = goalsFor - goalsAgainst;
                row[PointsRow] = win * 3 + drawn;

                rows.Add(row);
            }

            return rows;
        }

        /// <summary>
        /// Sorts the Table as a football table from <see cref="FootballLeague"/>
        /// </summary>
        /// <param name="matchList">Original match list of the league</param>
        private void SortFootballTable(FootballMatch[] matchList)
        {
            List<DataRow> rows = new List<DataRow>();
            foreach(DataRow row in Rows)
                rows.Add(row);

            // Base sorting
            rows.Sort(new FootballLeagueBaseComparer());

            // Direct matching
            var directGroups =
                from r in rows
                group r by new
                {
                    pts = r.Field<int>(PointsRow),
                    gdiff = r.Field<int>(GoalDiffRow),
                    gfor = r.Field<int>(GoalsForCountRow)
                }
                into rGroups
                //where rGroups.Count() > 1
                select rGroups;

            var directComparer = new FootballLeagueDirectComparer();
            var finalTable = new List<DataRow>(rows.Count);
            foreach(var dGroup in directGroups)
            {
                if(dGroup.Count() > 1)
                {
                    // Get matches
                    var groupTeams = dGroup.Select(t => t.Field<FootballTeam>(TeamRow)).ToArray();
                    //var matches = matchList.Where(m => groupTeams.All(t => t == m.TeamA || t == m.TeamB));
                    var matches = matchList.Where(m => m.AllTeams.Intersect(groupTeams).Count() > 1).ToArray();

                    // Get subtable
                    var subTable = CalculateSubTable(groupTeams, matches);
                    subTable.Sort(directComparer);

                    // Put group into final table
                    finalTable.AddRange(subTable);
                }
                else
                {
                    // One Team
                    //finalTable.Add(dGroup.First());
                    // Workaround to have the Rule 1-3 sorted teams
                    var newRow = NewRow();
                    newRow.ItemArray = dGroup.First().ItemArray;
                    finalTable.Add(newRow);
                }
            }

            // Save rows
            Rows.Clear();
            foreach(DataRow row in finalTable)
                Rows.Add(row);
        }

        #endregion
    }
}
