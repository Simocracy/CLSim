using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        private int _PenaltyTeamA;
        private int _PenaltyTeamB;

        private EDoubleMatchState _MatchState;

        #endregion

        #region Properties

        /// <summary>
        /// Team A (Home in <see cref="FirstLeg"/>, Away in <see cref="SecondLegRegular"/> and <see cref="ExtraTime"/>)
        /// </summary>
        public FootballTeam TeamA => FirstLeg.TeamA;

        /// <summary>
        /// Team B (Away in <see cref="FirstLeg"/>, Home in <see cref="SecondLegRegular"/> and <see cref="ExtraTime"/>)
        /// </summary>
        public FootballTeam TeamB => FirstLeg.TeamB;

        /// <summary>
        /// First leg
        /// </summary>
        public FootballMatch FirstLeg { get; }

        /// <summary>
        /// Second Leg after 90 min
        /// </summary>
        public FootballMatch SecondLegRegular { get; }

        /// <summary>
        /// Extra time
        /// </summary>
        public FootballMatch ExtraTime { get; }

        /// <summary>
        /// Gets a new <see cref="FootballMatch"/> instance with the full second leg
        /// </summary>
        public FootballMatch FullSecondLeg => GetFullSecondLeg();

        /// <summary>
        /// Full Result Team A
        /// </summary>
        public int FullResultA => FirstLeg.ResultA + SecondLegRegular.ResultB + ExtraTime.ResultB;

        /// <summary>
        /// Away goals Team A
        /// </summary>
        public int AwayGoalsA => SecondLegRegular.ResultB + ExtraTime.ResultB;

        /// <summary>
        /// Full Result Team B
        /// </summary>
        public int FullResultB => FirstLeg.ResultB + SecondLegRegular.ResultA + ExtraTime.ResultA;

        /// <summary>
        /// Away goals Team B
        /// </summary>
        public int AwayGoalsB => FirstLeg.ResultB;

        /// <summary>
        /// Penalty Shootout Result Team A, -1 if none
        /// </summary>
        public int PenaltyTeamA
        {
            get => _PenaltyTeamA;
            set { _PenaltyTeamA = value; Notify(); }
        }

        /// <summary>
        /// Penalty Shootout Result Team B, -1 if none
        /// </summary>
        public int PenaltyTeamB
        {
            get => _PenaltyTeamB;
            set { _PenaltyTeamB = value; Notify(); }
        }

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
        /// <param name="teamA">Team A (Home in <see cref="FirstLeg"/>, Away in <see cref="SecondLegRegular"/> and <see cref="ExtraTime"/>)</param>
        /// <param name="teamB">Team B (Away in <see cref="FirstLeg"/>, Home in <see cref="SecondLegRegular"/> and <see cref="ExtraTime"/>)</param>
        public DoubleMatch(FootballTeam teamA, FootballTeam teamB)
        {
            FirstLeg = new FootballMatch(teamA, teamB);
            SecondLegRegular = new FootballMatch(teamB, teamA);
            ExtraTime = new FootballMatch(teamB, teamA, 30);

            FirstLeg.PropertyChanged += PropertyChangedPropagator.Create(nameof(FootballMatch.IsSimulated), nameof(Winner), Notify);
            SecondLegRegular.PropertyChanged += PropertyChangedPropagator.Create(nameof(FootballMatch.IsSimulated), nameof(Winner), Notify);
            ExtraTime.PropertyChanged += PropertyChangedPropagator.Create(nameof(FootballMatch.IsSimulated), nameof(Winner), Notify);

            SecondLegRegular.PropertyChanged += PropertyChangedPropagator.Create(nameof(FootballMatch.IsSimulated), nameof(MatchState), Notify);
            ExtraTime.PropertyChanged += PropertyChangedPropagator.Create(nameof(FootballMatch.IsSimulated), nameof(MatchState), Notify);

            ResetMatch();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the winner Team, null if drawn
        /// </summary>
        private FootballTeam GetWinner()
        {
            MatchState = (ExtraTime.IsSimulated) ? EDoubleMatchState.ExtraTime : EDoubleMatchState.Regular;

            if(FullResultA > FullResultB) return TeamA;
            if(FullResultB > FullResultA) return TeamB;

            if(AwayGoalsA > AwayGoalsB) return TeamA;
            if(AwayGoalsB > AwayGoalsA) return TeamB;

            MatchState = EDoubleMatchState.Penalty;
            if(PenaltyTeamA > PenaltyTeamB) return TeamA;
            if(PenaltyTeamB > PenaltyTeamA) return TeamB;

            MatchState = EDoubleMatchState.None;
            return null;
        }

        /// <summary>
        /// Resets the match
        /// </summary>
        public void ResetMatch()
        {
            FirstLeg.Reset();
            SecondLegRegular.Reset();
            ExtraTime.Reset(30);

            PenaltyTeamA = -1;
            PenaltyTeamB = -1;

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
            SecondLegRegular.SwapTeams();
            ExtraTime.SwapTeams();
            
            var oldPenaltyA = PenaltyTeamA;
            PenaltyTeamA = PenaltyTeamB;
            PenaltyTeamB = oldPenaltyA;
        }

        /// <summary>
        /// Returns the full second leg with extra time
        /// </summary>
        /// <returns>New <see cref="FootballMatch"/> instance for the second leg</returns>
        public FootballMatch GetFullSecondLeg()
        {
            var secLeg = new FootballMatch(SecondLegRegular.TeamA, SecondLegRegular.TeamB)
            {
                ResultA = SecondLegRegular.ResultA + ExtraTime.ResultA,
                ResultB = SecondLegRegular.ResultB + ExtraTime.ResultB,
                Date = SecondLegRegular.Date,
                City = SecondLegRegular.City,
                Stadium = SecondLegRegular.Stadium
            };

            return secLeg;
        }

        public override string ToString()
        {
            return $"Double Match {Name}";
        }

        #endregion

        #region Simulation

        /// <summary>
        /// Simulates the match
        /// </summary>
        public void Simulate()
        {
            SimpleLog.Info($"Simulate {this}.");

            FirstLeg.Simulate();
            SecondLegRegular.Simulate();
            MatchState = EDoubleMatchState.Regular;

            // Extra Time
            if(Winner == null)
            {
                SimpleLog.Info($"Extra time needed on {this}.");

                ExtraTime.Simulate();
                MatchState = EDoubleMatchState.ExtraTime;

                // Penalty
                if (Winner == null)
                {
                    SimpleLog.Info($"Penalty Shootout needed on {this}.");

                    PenaltyShootout();
                    MatchState = EDoubleMatchState.Penalty;
                }
            }

            SimpleLog.Info($"{this} simulated.");
        }

        /// <summary>
        /// Simulates a penalty shootout
        /// </summary>
        /// <remarks>Based on Algorithm by Laserdisc/Flux:
        /// <code>
        ///     //simulates a rudimentary penalty shoot-out
        ///     int[] goals = new int[2][6]; //in the first row the total amount of goals is stored. in the second to sixth row there is stored whether the team scored or not.
        ///     var round = 0;
        ///     var totalStrength = TeamA.Strength + TeamB.Strength;
        ///     
        ///     goals[0][0] = 0; //set goals for team A to 0
        ///     goals[1][0] = 0; //set goals for team B to 0
        ///     
        ///     for(int i = 1; i &lt;= 5; i++)
        ///     {
        ///         var valueA = Random.Next(0, totalStrength);
        ///         var valueB = Random.Next(0, totalStrength);
        ///     
        ///         if(valueA &lt; TeamA.Strength)
        ///         {
        ///             goals[0][0]++;
        ///             goals[0][i] = 1;
        ///         }
        ///         else goals[0][i] = 0;
        ///     
        ///         if(valueB &lt; TeamB.Strength)
        ///         {
        ///             goals[1][0]++;
        ///             goals[1][i] = 1;
        ///         }
        ///         else goals[1][i] = 0;
        ///     }
        ///     
        ///     if(goals[0][0] &gt; goals[1][0]) "A hat gewonnen, tu irgendwas";
        ///     elseif(goals[0][0] &lt; goals[1][0]) "B hat gewonnen, tu irgendwas";
        ///     else {
        ///         //additional penalties
        ///         var value = Random.Next(0, totalStrength);
        ///     
        ///         if(value &lt; TeamA.Strength) "A hat gewonnen, tu irgendwas";
        ///         else "B hat gewonnen, tu irgendwas";
        /// </code>
        /// </remarks>
        private void PenaltyShootout()
        {
            SimpleLog.Info($"Simulate Penalty Shootout on {this}.");

            var totalStrength = TeamA.AvgStrength + TeamB.AvgStrength;
            
            var firVal = Globals.Random.Next(0, 1);
            var firTeam = firVal == 0 ? TeamA : TeamB;
            var secTeam = firVal == 1 ? TeamA : TeamB;

            // first 5 penalties
            var firstPenalties = 5;
            int remainA = 0, remainB = 0;
            int neededA, neededB;
            int penaltyA = 0, penaltyB = 0;
            for(int i = 0; i < firstPenalties; i++)
            {
                var valueA = Globals.Random.Next(0, totalStrength);
                var valueB = Globals.Random.Next(0, totalStrength);

                if(valueA < firTeam.AvgStrength)
                    penaltyA++;

                // break
                remainA = firstPenalties - i;
                neededB = firstPenalties - penaltyA;
                if(neededB > remainB) break;

                if(valueB < secTeam.AvgStrength)
                    penaltyB++;

                // break
                remainB = firstPenalties - i;
                neededA = firstPenalties - penaltyB;
                if(neededA > remainA) break;
            }

            PenaltyTeamA = firVal == 0 ? penaltyA : penaltyB;
            PenaltyTeamB = firVal == 1 ? penaltyA : penaltyB;

            // additional penalties
            while(Winner == null)
            {
                var valueA = Globals.Random.Next(0, totalStrength);
                var valueB = Globals.Random.Next(0, totalStrength);

                if(valueA < firTeam.AvgStrength)
                    penaltyA++;
                if(valueB < secTeam.AvgStrength)
                    penaltyB++;

                PenaltyTeamA = firVal == 0 ? penaltyA : penaltyB;
                PenaltyTeamB = firVal == 1 ? penaltyA : penaltyB;
            }

            SimpleLog.Info($"Penalty Shootout simulated: PenaltyTeamA={PenaltyTeamA}, PenaltyTeamB={PenaltyTeamB}.");
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
