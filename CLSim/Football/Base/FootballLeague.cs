using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using SimpleLogger;

namespace Simocracy.CLSim.Football.Base
{
    /// <summary>
    /// Football group simulation
    /// </summary>
    [DebuggerDisplay("TeamCount={" + nameof(TeamCount) + "}")]
    public class FootballLeague : INotifyPropertyChanged
    {
        #region Members

        private ObservableCollection<FootballTeam> _Teams;
        private ObservableCollection<FootballMatch> _Matches;
        private LeagueTable _Table;

        #endregion

        #region Constuctor

        /// <summary>
        /// Creates a new group
        /// </summary>
        /// <param name="id">Group ID</param>
        /// <param name="teams">Teams</param>
        public FootballLeague(string id, params FootballTeam[] teams)
        {
            SimpleLog.Info("Create new Football League");

            ID = id;
            Teams = new ObservableCollection<FootballTeam>(teams);
            
            Matches = new ObservableCollection<FootballMatch>();
            CreateMatches();

            SimpleLog.Info($"Football League created with ID={ID}");
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

        #endregion

        #region Simulation

        /// <summary>
        /// Creates matches
        /// </summary>
        public void CreateMatches()
        {
            Matches.Clear();

            for(int a = 0; a < TeamCount-1; a++)
            for(int b = a + 1; b < TeamCount; b++)
                Matches.Add(new FootballMatch(Teams[a], Teams[b]));

            SimpleLog.Info($"Matches Created in Football League ID={ID}");
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

        public void CalculateTable()
        {
            SimpleLog.Info($"Calculate table in Football League ID={ID}");

            Table = new LeagueTable();
            Table.CalculateTable(Teams, Matches);

            SimpleLog.Info($"Finished calculating table in Football League ID={ID}");
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
