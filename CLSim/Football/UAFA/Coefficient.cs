﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simocracy.CLSim.Football.Base;

namespace Simocracy.CLSim.Football.UAFA
{
    /// <summary>
    /// Represents the UAFA Coefficient for one team
    /// </summary>
    public class Coefficient : INotifyPropertyChanged
    {

        #region Members

        private FootballTeam _Team;

        private int _Won;
        private int _Drawn;
        private HashSet<ETournamentRound> _RoundsPlayed;

        private int _Points;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance for the given team
        /// </summary>
        /// <param name="team">The football team</param>
        public Coefficient(FootballTeam team)
        {
            Team = team;
            RoundsPlayed = new HashSet<ETournamentRound>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// The team
        /// </summary>
        public FootballTeam Team
        {
            get => _Team;
            private set
            {
                _Team = value;
                Notify();
            }
        }

        /// <summary>
        /// Matches Won
        /// </summary>
        public int Won
        {
            get => _Won;
            private set
            {
                _Won = value;
                Notify();
            }
        }

        /// <summary>
        /// Matches Drawn
        /// </summary>
        public int Drawn
        {
            get => _Drawn;
            private set
            {
                _Drawn = value;
                Notify();
            }
        }

        /// <summary>
        /// All played rounds
        /// </summary>
        public HashSet<ETournamentRound> RoundsPlayed
        {
            get => _RoundsPlayed;
            set
            {
                _RoundsPlayed = value;
                Notify();
            }
        }

        /// <summary>
        /// Coefficient of the Team for the current season
        /// </summary>
        public int Points
        {
            get => _Points;
            private set
            {
                _Points = value;
                Notify();
            }
        }

        #endregion

        #region Calculation

        /// <summary>
        /// Adds the given <see cref="DoubleMatch"/> from the given round to the match list
        /// </summary>
        /// <param name="round">Tournament round</param>
        /// <param name="dMatch"><see cref="DoubleMatch"/> to add</param>
        public void AddMatch(ETournamentRound round, DoubleMatch dMatch)
        {
            AddMatch(round, dMatch.FirstLeg);
            AddMatch(round, dMatch.GetFullSecondLeg());
        }

        /// <summary>
        /// Adds the given match from the given round to the match list
        /// </summary>
        /// <param name="round">Tournament round</param>
        /// <param name="match">Match to add</param>
        public void AddMatch(ETournamentRound round, FootballMatch match)
        {
            RoundsPlayed.Add(round);
            if(match.ResultA == match.ResultB) Drawn++;
            else if(match.TeamA == Team && match.ResultA > match.ResultB) Won++;
            else if(match.TeamB == Team && match.ResultA < match.ResultB) Won++;
        }

        /// <summary>
        /// Calculates the coefficient points
        /// </summary>
        public void CalculatePoints()
        {
            Points = Won * 2 + Drawn;
            foreach(var round in RoundsPlayed)
            {
                switch(round)
                {
                    case ETournamentRound.CLGroupStage:
                        Points += 4;
                        break;
                    case ETournamentRound.CLRoundOf16:
                        Points += 5;
                        break;
                    case ETournamentRound.CLQuarterFinals:
                        Points += 2;
                        break;
                    case ETournamentRound.CLSemiFinals:
                        Points += 2;
                        break;
                    case ETournamentRound.CLFinal:
                        Points += 2;
                        break;
                    case ETournamentRound.ALGroupStage:
                        if(Points < 2) Points = 2;
                        break;
                    case ETournamentRound.ALRoundOf32:
                        Points += 1;
                        break;
                    case ETournamentRound.ALRoundOf16:
                        Points += 1;
                        break;
                    case ETournamentRound.ALQuarterFinals:
                        Points += 1;
                        break;
                    case ETournamentRound.ALSemiFinals:
                        Points += 1;
                        break;
                    case ETournamentRound.ALFinal:
                        Points += 1;
                        break;
                }
            }
        }

        #endregion

        #region Export

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
