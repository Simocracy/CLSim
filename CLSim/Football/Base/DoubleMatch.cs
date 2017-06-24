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

        #region Members

        private int _PenaltyTeamA;
        private int _PenaltyTeamB;

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

            ResetMatch();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the winner Team, null if drawn
        /// </summary>
        private FootballTeam GetWinner()
        {
            if(FullResultA > FullResultB) return TeamA;
            if(FullResultB > FullResultA) return TeamB;

            if(AwayGoalsA > AwayGoalsB) return TeamA;
            if(AwayGoalsB > AwayGoalsA) return TeamB;

            if(PenaltyTeamA > PenaltyTeamB) return TeamA;
            if(PenaltyTeamB > PenaltyTeamA) return TeamB;

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

        public override string ToString()
        {
            return
                $"Double Match: TeamA={TeamA}, TeamB={TeamB}, ResultA={FullResultA}, ResultB={FullResultB}";
        }

        #endregion

        #region Simulation

        /// <summary>
        /// Simulates the match
        /// </summary>
        public void Simulate()
        {
            SimpleLog.Info($"Simulate {this}");

            FirstLeg.Simulate();
            SecondLegRegular.Simulate();

            // Extra Time
            if(Winner == null)
            {
                ExtraTime.Simulate();

                // Penalty
                if(Winner == null)
                {
                    
                }
            }

            SimpleLog.Info($"{this} simulated.");
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
