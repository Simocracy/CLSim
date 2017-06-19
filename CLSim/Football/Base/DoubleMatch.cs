using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simocracy.CLSim.Football.Base
{
    /// <summary>
    /// Football Match with 2 legs
    /// </summary>
    [DebuggerDisplay("Match:{" + nameof(Name) + "}")]
    public class DoubleMatch
    {

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
        public FootballMatch FirstLeg { get; set; }

        /// <summary>
        /// Second Leg after 90 min
        /// </summary>
        public FootballMatch SecondLegRegular { get; set; }

        /// <summary>
        /// Extra time
        /// </summary>
        public FootballMatch ExtraTime { get; set; }

        /// <summary>
        /// Result Team A
        /// </summary>
        public int ResultA => FirstLeg.ResultA + SecondLegRegular.ResultB + ExtraTime.ResultB;

        /// <summary>
        /// Result Team B
        /// </summary>
        public int ResultB => FirstLeg.ResultB + SecondLegRegular.ResultA + ExtraTime.ResultA;

        /// <summary>
        /// Penalty Shootout Result Team A, -1 if none
        /// </summary>
        public int PenaltyTeamA { get; set; }

        /// <summary>
        /// Penalty Shootout Result Team B, -1 if none
        /// </summary>
        public int PenaltyTeamB { get; set; }

        /// <summary>
        /// Gets the winner Team, null if drawn
        /// </summary>
        public FootballTeam Winner
        {
            get
            {
                if(ResultA > ResultB || PenaltyTeamA > PenaltyTeamB) return TeamA; // Win A
                if(ResultA == ResultB) return null; // Drawn
                return TeamB; // Else Win B
            }
        }

        /// <summary>
        /// Match name
        /// </summary>
        public string Name => $"{TeamA.FullName} vs. {TeamB.FullName}";
        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new double match
        /// </summary>
        /// <param name="teamA"></param>
        /// <param name="teamB"></param>
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
        /// Resets the match
        /// </summary>
        public void ResetMatch()
        {
            FirstLeg.Reset();
            SecondLegRegular.Reset();
            ExtraTime.Reset(30);

            PenaltyTeamA = -1;
            PenaltyTeamB = -1;
        }

        /// <summary>
        /// Swaps the teams
        /// </summary>
        public void SwapTeams()
        {
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
                $"TeamA={TeamA}, TeamB={TeamB}, ResultA={ResultA}, ResultB={ResultB}";
        }

        #endregion

        #region Simulation

        /// <summary>
        /// Simulates the match
        /// </summary>
        public void Simulate()
        {
            FirstLeg.Simulate();
            SecondLegRegular.Simulate();

            // Extra Time
            if(ResultA == ResultB)
            {
                ExtraTime.Simulate();

                // Penalty
                if(ResultA == ResultB)
                {
                    
                }
            }
        }

        #endregion

    }
}
