using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleLogger;

namespace Simocracy.CLSim.Football.Base
{
    /// <summary>
    /// Football group simulation
    /// </summary>
    [DebuggerDisplay("TeamCount={" + nameof(TeamCount) + "}, Teams={" + nameof(TeamListStr) + "}")]
    public class FootballLeague : INotifyPropertyChanged
    {
        #region MatchMode Enum

        /// <summary>
        /// Match creation mode
        /// </summary>
        public enum EMatchMode
        {
            Default,
            UafaCl
        }

        #endregion

        #region Members

        private ObservableCollection<FootballTeam> _Teams;
        private ObservableCollection<FootballMatch> _Matches;
        private LeagueTable _Table;

        private EMatchMode _MatchMode;

        private string _TeamListStr;

        #endregion

        #region Constuctor

        /// <summary>
        /// Creates a new group
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <param name="teams">Teams</param>
        public FootballLeague(string id, params FootballTeam[] teams)
            : this(id, EMatchMode.Default, teams) { }

        /// <summary>
        /// Creates a new group
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <param name="teams">Teams</param>
        /// <param name="matchMode">The match mode for the creation</param>
        public FootballLeague(string id, EMatchMode matchMode, params FootballTeam[] teams)
        {
            SimpleLog.Info("Create new Football League");

            ID = id;
            Teams = new ObservableCollection<FootballTeam>(teams);
            
            Matches = new ObservableCollection<FootballMatch>();
            MatchMode = matchMode;
            CreateMatches();

            SimpleLog.Info($"Football League {this} created.");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Group ID
        /// </summary>
        public string ID { get; }

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
        /// Group table
        /// </summary>
        public LeagueTable Table
        {
            get => _Table;
            private set { _Table = value; Notify(); }
        }

        /// <summary>
        /// Match creation mode
        /// </summary>
        public EMatchMode MatchMode
        {
            get => _MatchMode;
            private set { _MatchMode = value; Notify(); }
        }

        /// <summary>
        /// True if all matches are simulated
        /// </summary>
        public bool IsAllMatchesSimulated => (Matches != null && Matches.Count > 0) && Matches.All(g => g.IsSimulated);

        /// <summary>
        /// True if table is calculated
        /// </summary>
        public bool IsTableCalculated => Table?.IsTableCalculated ?? false;

        /// <summary>
        /// List of all teams as String
        /// </summary>
        public string TeamListStr
        {
            get
            {
                if(_TeamListStr == null)
                {
                    var sb = new StringBuilder();
                    foreach(var team in Teams)
                        sb.Append($"{team.FullName}, ");
                    sb.Remove(sb.Length - 2, 2);
                    _TeamListStr = sb.ToString();
                }
                return _TeamListStr;
            }
        }

        #endregion

        #region Simulation

        /// <summary>
        /// Creates matches
        /// </summary>
        public void CreateMatches()
        {
            Matches.Clear();

            if(MatchMode == EMatchMode.Default)
            {
                for(int a = 0; a < TeamCount; a++)
                    for(int b = 0; b < TeamCount; b++)
                        if(a != b)
                            Matches.Add(new FootballMatch(Teams[a], Teams[b]));
            }

            else if(MatchMode == EMatchMode.UafaCl)
            {
                // Based on https://simocracy.de/Vorlage:5er-Gruppe
                Matches.Add(new FootballMatch(Teams[0], Teams[1]));
                Matches.Add(new FootballMatch(Teams[2], Teams[3]));
                Matches.Add(new FootballMatch(Teams[1], Teams[2]));
                Matches.Add(new FootballMatch(Teams[3], Teams[4]));
                Matches.Add(new FootballMatch(Teams[4], Teams[0]));
                Matches.Add(new FootballMatch(Teams[3], Teams[1]));
                Matches.Add(new FootballMatch(Teams[2], Teams[0]));
                Matches.Add(new FootballMatch(Teams[1], Teams[4]));
                Matches.Add(new FootballMatch(Teams[0], Teams[3]));
                Matches.Add(new FootballMatch(Teams[4], Teams[2]));
                Matches.Add(new FootballMatch(Teams[1], Teams[0]));
                Matches.Add(new FootballMatch(Teams[3], Teams[2]));
                Matches.Add(new FootballMatch(Teams[2], Teams[1]));
                Matches.Add(new FootballMatch(Teams[4], Teams[3]));
                Matches.Add(new FootballMatch(Teams[0], Teams[4]));
                Matches.Add(new FootballMatch(Teams[1], Teams[3]));
                Matches.Add(new FootballMatch(Teams[0], Teams[2]));
                Matches.Add(new FootballMatch(Teams[4], Teams[1]));
                Matches.Add(new FootballMatch(Teams[3], Teams[0]));
                Matches.Add(new FootballMatch(Teams[2], Teams[4]));
            }

            // Notify IsSimulated
            foreach(var m in Matches)
                m.PropertyChanged += PropertyChangedPropagator.Create(nameof(FootballMatch.IsSimulated),
                    nameof(IsAllMatchesSimulated), Notify);

            SimpleLog.Info($"Matches Created in Football League ID={ID} with MatchMode={MatchMode}");
        }

        /// <summary>
        /// Simulate matches
        /// </summary>
        public void Simulate()
        {
            SimpleLog.Info($"Simulate matches in Football League ID={ID}");
            foreach(var match in Matches)
                match.Simulate();
            SimpleLog.Info($"Finished simulating matches in Football League ID={ID}");
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
        /// Calculates the table
        /// </summary>
        public void CalculateTable()
        {
            SimpleLog.Info($"Calculate table in Football League ID={ID}");

            Table = new LeagueTable();
            Table.TableName = ID;
            Table.PropertyChanged += PropertyChangedPropagator.Create(nameof(LeagueTable.IsTableCalculated),
                nameof(IsTableCalculated), Notify);
            Table.CalculateTable(Teams, Matches);

            SimpleLog.Info($"Finished calculating table in Football League ID={ID}");
        }

        /// <summary>
        /// Gibt einen <see cref="String"/> zurück, der das Objekt darstellt.
        /// </summary>
        /// <returns>Objekt als String</returns>
        public sealed override string ToString()
        {
            return $"Group {ID}";
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
