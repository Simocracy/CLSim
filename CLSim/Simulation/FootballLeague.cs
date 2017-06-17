using SimpleLogger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simocracy.CLSim.Extensions;

namespace Simocracy.CLSim.Simulation
{
    /// <summary>
    /// Football group simulation
    /// </summary>
    [DebuggerDisplay("TeamCount={" + nameof(TeamCount) + "}")]
    public class FootballLeague : INotifyPropertyChanged
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
        public const string DirectMatchPos = "DirectPos";

        #endregion

        #region Members

        private ObservableCollection<FootballTeam> _Teams;
        private ObservableCollection<FootballMatch> _Matches;
        private DataTable _Table;

        #endregion

        #region Constuctor

        /// <summary>
        /// Creates a new group
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <param name="teams">Teams</param>
        public FootballLeague(string id, params FootballTeam[] teams)
        {
            ID = id;
            Teams = new ObservableCollection<FootballTeam>(teams);

            SimpleLog.Info($"Create Football League: {ToString()}");

            Matches = new ObservableCollection<FootballMatch>();
            CreateMatches();
            CreateTable();

            SimpleLog.Info($"Football League created with ID={ID}");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Group ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Teams of groupt
        /// </summary>
        public ObservableCollection<FootballTeam> Teams
        {
            get => _Teams;
            set { _Teams = value; Notify(); }
        }

        /// <summary>
        /// Group Matches
        /// </summary>
        public ObservableCollection<FootballMatch> Matches
        {
            get => _Matches;
            set { _Matches = value; Notify(); }
        }

        /// <summary>
        /// Team count
        /// </summary>
        public int TeamCount => Teams.Count;

        /// <summary>
        /// Group table.
        /// Info for DirectMatchPos Column: -1 not needed, 0 position drawing needed, >= 1 position
        /// </summary>
        public DataTable Table
        {
            get => _Table;
            set { _Table = value; Notify(); }
        }

        #endregion

        #region Simulation

        /// <summary>
        /// Creates matches
        /// </summary>
        public void CreateMatches()
        {
            Matches.Clear();

            foreach (var teamA in Teams)
            {
                foreach (var teamB in Teams)
                {
                    if (teamA != teamB)
                        Matches.Add(new FootballMatch(teamA, teamB));
                }
            }

            SimpleLog.Info($"Matches Created in Football League ID={ID}");
        }

        /// <summary>
        /// Simulate matches
        /// </summary>
        public void Simulate()
        {
            SimpleLog.Info($"Simulate Matches in Football League ID={ID}");
            foreach(var match in Matches)
                match.Simulate();
        }

        /// <summary>
        /// Simulate matches async
        /// </summary>
        public async Task SimulateAsync()
        {
            await Task.Run(() => Simulate());
        }

        /// <summary>
        /// Calculates table async
        /// </summary>
        public async Task CalculateTableAsync()
        {
            await Task.Run(() => CalculateTable());
        }

        /// <summary>
        /// Calculates Table
        /// </summary>
        public void CalculateTable()
        {
            SimpleLog.Info($"Calculate Table in Football League ID={ID}");
            CreateTable();

            foreach(var team in Teams)
            {
                var row = Table.NewRow();

                int drawn, lose, goalsFor, goalsAgainst;
                var win = drawn = lose = goalsFor = goalsAgainst = 0;

                foreach(var match in Matches)
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
                row[DirectMatchPos] = -1;

                Table.Rows.Add(row);
            }

            //DataView dv = Table.DefaultView;
            //dv.Sort = "Points DESC, GoalDiff DESC, GoalsFor DESC";
            //Table = dv.ToTable();

            Table.SortFootballTable(Matches);
        }

        /// <summary>
        /// Creates table
        /// </summary>
        private void CreateTable()
        {
            var table = new DataTable();

            table.Columns.Add(TeamRow, typeof(FootballTeam));
            table.Columns.Add(MatchCountRow, typeof(int));
            table.Columns.Add(WinCountRow, typeof(int));
            table.Columns.Add(DrawnCountRow, typeof(int));
            table.Columns.Add(LoseCountRow, typeof(int));
            table.Columns.Add(GoalsForCountRow, typeof(int));
            table.Columns.Add(GoalsAgainstCountRow, typeof(int));
            table.Columns.Add(GoalDiffRow, typeof(int));
            table.Columns.Add(PointsRow, typeof(int));
            table.Columns.Add(DirectMatchPos, typeof(int));

            Table = table;
        }

        /// <summary>
        /// Gibt einen <see cref="String"/> zurück, der das Objekt darstellt.
        /// </summary>
        /// <returns>Objekt als String</returns>
        public sealed override string ToString()
        {
            return $"ID={ID}, TeamCount={TeamCount}";
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
