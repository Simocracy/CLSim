using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using SimpleLogger;

namespace Simocracy.CLSim.Football.Base
{
    /// <summary>
    /// Football Match with 2 legs
    /// </summary>
    [DebuggerDisplay("Match:{" + nameof(Name) + "}")]
    public class DoubleMatch : INotifyPropertyChanged
    {

        #region EDoubleMatchState

        /// <summary>
        /// Enum for match state, when the winner team won
        /// </summary>
        public enum EDoubleMatchState
        {
            None,
            Regular,
            ExtraTime,
            Penalty
        }

        #endregion

        #region Members

        private EDoubleMatchState _MatchState;

        #endregion

        #region Properties

        /// <summary>
        /// Team A (Home in <see cref="FirstLeg"/>, Away in <see cref="SecondLeg"/>)
        /// </summary>
        public FootballTeam TeamA => FirstLeg.TeamA;

        /// <summary>
        /// Team B (Away in <see cref="FirstLeg"/>, Home in <see cref="SecondLeg"/>)
        /// </summary>
        public FootballTeam TeamB => FirstLeg.TeamB;

        /// <summary>
        /// First leg
        /// </summary>
        public FootballMatch FirstLeg { get; }

        /// <summary>
        /// Second Leg
        /// </summary>
        public ExtendedFootballMatch SecondLeg { get; }

        /// <summary>
        /// Full Result Team A
        /// </summary>
        public int FullResultA => FirstLeg.ResultA.GetValueOrDefault() +
                                  SecondLeg.ResultB.GetValueOrDefault();

        /// <summary>
        /// Away goals Team A
        /// </summary>
        public int AwayGoalsA => SecondLeg.ResultB.GetValueOrDefault();

        /// <summary>
        /// Full Result Team B
        /// </summary>
        public int FullResultB => FirstLeg.ResultB.GetValueOrDefault() +
                                  SecondLeg.ResultA.GetValueOrDefault();

        /// <summary>
        /// Away goals Team B
        /// </summary>
        public int AwayGoalsB => FirstLeg.ResultB.GetValueOrDefault();

        /// <summary>
        /// Penalty Shootout Result Team A, -1 if none
        /// </summary>
        public int? PenaltyTeamA => SecondLeg.PenaltyB;

        /// <summary>
        /// Penalty Shootout Result Team B, -1 if none
        /// </summary>
        public int? PenaltyTeamB => SecondLeg.PenaltyA;

        /// <summary>
        /// Match name
        /// </summary>
        public string Name => $"{TeamA.FullName} vs. {TeamB.FullName}";

        /// <summary>
        /// Gets the winner Team, null if drawn
        /// </summary>
        public FootballTeam Winner => GetWinner();

        /// <summary>
        /// Match state, when the winner team won
        /// </summary>
        public EDoubleMatchState MatchState
        {
            get => _MatchState;
            set { _MatchState = value; Notify(); }
        }

        /// <summary>
        /// Array with all teams
        /// </summary>
        public FootballTeam[] AllTeams => FirstLeg.AllTeams;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new double match
        /// </summary>
        /// <param name="teamA">Team A (Home in <see cref="FirstLeg"/>, Away in <see cref="SecondLeg"/>)</param>
        /// <param name="teamB">Team B (Away in <see cref="FirstLeg"/>, Home in <see cref="SecondLeg"/>)</param>
        public DoubleMatch(FootballTeam teamA, FootballTeam teamB)
        {
            FirstLeg = new FootballMatch(teamA, teamB);
            SecondLeg = new ExtendedFootballMatch(teamB, teamA);

            FirstLeg.PropertyChanged += PropertyChangedPropagator.Create(nameof(FootballMatch.IsSimulated), nameof(Winner), Notify);
            SecondLeg.PropertyChanged += PropertyChangedPropagator.Create(nameof(ExtendedFootballMatch.IsSimulated), nameof(Winner), Notify);

            SecondLeg.PropertyChanged += PropertyChangedPropagator.Create(nameof(ExtendedFootballMatch.IsSimulated), nameof(MatchState), Notify);
            SecondLeg.PropertyChanged += PropertyChangedPropagator.Create(nameof(ExtendedFootballMatch.PenaltyB), nameof(PenaltyTeamA), Notify);
            SecondLeg.PropertyChanged += PropertyChangedPropagator.Create(nameof(ExtendedFootballMatch.PenaltyA), nameof(PenaltyTeamB), Notify);

            ResetMatch();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the winner Team, null if drawn
        /// </summary>
        private FootballTeam GetWinner()
        {
            //MatchState = (ExtraTime.IsSimulated) ? EDoubleMatchState.ExtraTime : EDoubleMatchState.Regular;

            if(FullResultA > FullResultB) return TeamA;
            if(FullResultB > FullResultA) return TeamB;

            if(AwayGoalsA > AwayGoalsB) return TeamA;
            if(AwayGoalsB > AwayGoalsA) return TeamB;

            //MatchState = EDoubleMatchState.Penalty;
            if(PenaltyTeamA > PenaltyTeamB) return TeamA;
            if(PenaltyTeamB > PenaltyTeamA) return TeamB;

            //MatchState = EDoubleMatchState.None;
            return null;
        }

        /// <summary>
        /// Resets the match
        /// </summary>
        public void ResetMatch()
        {
            FirstLeg.Reset();
            SecondLeg.Reset();

            MatchState = EDoubleMatchState.None;

            SimpleLog.Info($"{this} initialized.");
        }

        /// <summary>
        /// Swaps the teams
        /// </summary>
        public void SwapTeams()
        {
            SimpleLog.Info($"Swap teams in {this}");

            FirstLeg.SwapTeams();
            SecondLeg.SwapTeams();
        }

        public override string ToString()
        {
            return $"Double Match {Name}";
        }

        #endregion

        #region Simulation

        /// <summary>
        /// Simulate matches async
        /// </summary>
        public async Task SimulateAsync()
        {
            await Task.Run(() => Simulate());
        }

        /// <summary>
        /// Simulates the match
        /// </summary>
        public void Simulate()
        {
            SimpleLog.Info($"Simulate {this}.");

            FirstLeg.Simulate();
            SecondLeg.SimulateRegular();
            MatchState = EDoubleMatchState.Regular;

            if(Winner == null)
            {
                MatchState = EDoubleMatchState.ExtraTime;
                SecondLeg.SimulateExtra();

                if(Winner == null)
                {
                    MatchState = EDoubleMatchState.Penalty;
                    SecondLeg.SimulatePenaltyShootout();
                }
            }

            // Winner output to force Notify in Winner
            SimpleLog.Info($"{this} simulated, winner = {Winner}.");
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
