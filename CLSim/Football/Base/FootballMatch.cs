using System;
using System.ComponentModel;
using System.Diagnostics;
using SimpleLogger;

namespace Simocracy.CLSim.Football.Base
{

    /// <summary>
    /// Football Match
    /// </summary>
    /// <remarks>By Laserdisc, adapted from Simocracy Sport Simulator</remarks>
    [DebuggerDisplay("Match:{" + nameof(Name) + "}")]
    public class FootballMatch : INotifyPropertyChanged
    {
        #region Members

        private FootballTeam _TeamA;
        private FootballTeam _TeamB;
        private int _ResultA;
        private int _ResultB;
        private DateTime _Date;
        private string _Stadium;
        private string _City;

        private int _Ball;
        private int _MatchTime;
        private int _Start;

        private const int TorwartA = 10;
        private const int TorwartB = 20;
        private const int AbwehrA = 11;
        private const int AbwehrB = 21;
        private const int MittelfeldA = 12;
        private const int MittelfeldB = 22;
        private const int SturmA = 13;
        private const int SturmB = 23;
        private const int TorA = 14;
        private const int TorB = 24;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new match
        /// </summary>
        /// <param name="teamA">Team A (Home)</param>
        /// <param name="teamB">Team B (Away)</param>
        public FootballMatch(FootballTeam teamA, FootballTeam teamB)
        {
            AllTeams = new FootballTeam[2];
            TeamA = teamA;
            TeamB = teamB;

            Reset();
        }

        /// <summary>
        /// Creates a new match
        /// </summary>
        /// <param name="teamA">Team A (Home)</param>
        /// <param name="teamB">Team B (Away)</param>
        /// <param name="matchTime">Specific match time</param>
        public FootballMatch(FootballTeam teamA, FootballTeam teamB, int matchTime)
            : this(teamA, teamB)
        {
            Reset(matchTime);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Team A (Home)
        /// </summary>
        public FootballTeam TeamA
        {
            get => _TeamA;
            set { _TeamA = value;
                AllTeams[0] = value;
                Notify();
            }
        }

        /// <summary>
        /// Team B (Away)
        /// </summary>
        public FootballTeam TeamB
        {
            get => _TeamB;
            set { _TeamB = value;
                AllTeams[1] = value;
                Notify();
            }
        }

        /// <summary>
        /// Result Team A
        /// </summary>
        public int ResultA
        {
            get => _ResultA;
            set { _ResultA = value; Notify(); }
        }

        /// <summary>
        /// Result Team B
        /// </summary>
        public int ResultB
        {
            get => _ResultB;
            set { _ResultB = value; Notify(); }
        }

        /// <summary>
        /// Match date
        /// </summary>
        public DateTime Date
        {
            get => _Date;
            set { _Date = value; Notify(); }
        }

        /// <summary>
        /// Match stadium
        /// </summary>
        public string Stadium
        {
            get => _Stadium;
            set { _Stadium = value; Notify(); }
        }

        /// <summary>
        /// Match city (with Flag)
        /// </summary>
        public string City
        {
            get => _City;
            set { _City = value; Notify(); }
        }

        /// <summary>
        /// Match name
        /// </summary>
        public string Name => $"{TeamA.FullName} vs. {TeamB.FullName}";

        /// <summary>
        /// Array with all teams
        /// </summary>
        public FootballTeam[] AllTeams { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the match
        /// </summary>
        public void Reset(int time = 0)
        {
            ResultA = 0;
            ResultB = 0;
            _Ball = 0;
            _MatchTime = time;
            _Start = 0;
        }

        /// <summary>
        /// Swaps the teams
        /// </summary>
        public void SwapTeams()
        {
            var oldTeamA = TeamA;
            var oldResultA = ResultA;
            TeamA = TeamB;
            ResultA = ResultB;
            TeamB = oldTeamA;
            ResultB = oldResultA;
        }

        public override string ToString()
        {
            return
                $"TeamA={TeamA}, TeamB={TeamB}, ResultA={ResultA}, ResultB={ResultB}";
        }

        #endregion

        #region Simulation

        /// <summary>
        /// Simulates the Match
        /// </summary>
        public void Simulate()
        {
            SimpleLog.Info($"Simulate Match: TeamA={TeamA}, TeamB={TeamB}");

            int resA = 0;
            int resB = 0;

            Reset(90);

            _Ball = Kickoff();
            for(int i = 1; i <= _MatchTime; i++)
            {
                if(i == 45)
                {
                    if(_Ball == TorA)
                        resA++;
                    else if(_Ball == TorB)
                        resB++;
                    _Ball = _Start;
                }

                switch(_Ball)
                {
                    case TorwartA:
                        _Ball = Turn(TeamA.Strength, TeamB.Strength);
                        break;
                    case AbwehrA:
                        _Ball = Turn(TeamA.Strength, TeamB.Strength);
                        break;
                    case MittelfeldA:
                        _Ball = Turn(TeamA.Strength, TeamB.Strength);
                        break;
                    case SturmA:
                        _Ball = Turn(TeamA.Strength, TeamB.Strength + TeamB.Strength / 2);
                        break;
                    case TorA:
                        resA++;
                        _Ball = MittelfeldB;
                        break;
                    case TorwartB:
                        _Ball = Turn(TeamB.Strength, TeamA.Strength);
                        break;
                    case AbwehrB:
                        _Ball = Turn(TeamB.Strength, TeamA.Strength);
                        break;
                    case MittelfeldB:
                        _Ball = Turn(TeamB.Strength, TeamA.Strength);
                        break;
                    case SturmB:
                        _Ball = Turn(TeamB.Strength, TeamA.Strength + TeamA.Strength / 2);
                        break;
                    case TorB:
                        resB++;
                        _Ball = MittelfeldA;
                        break;
                }
            }

            ResultA = resA;
            ResultB = resB;

            SimpleLog.Info($"Match Result: ResultA={ResultA}, ResultB={ResultB}");
        }

        private int Turn(int strength1, int strength2)
        {
            int random = Globals.Random.Next(strength1 + strength2);
            if(random < strength1)
                return ++_Ball;
            else
                switch(_Ball)
                {
                    case TorwartA:
                        return SturmB;
                    case AbwehrA:
                        return MittelfeldB;
                    case MittelfeldA:
                        return AbwehrB;
                    case SturmA:
                        return TorwartB;
                    case TorwartB:
                        return SturmA;
                    case AbwehrB:
                        return MittelfeldA;
                    case MittelfeldB:
                        return AbwehrA;
                    case SturmB:
                        return TorwartA;
                }

            return 0;
        }

        private int Kickoff()
        {
            int random = Globals.Random.Next(2);
            if(random == 0)
            { _Start = MittelfeldB; return MittelfeldA; }
            if(random == 1)
            { _Start = MittelfeldA; return MittelfeldB; }
            else
                return 0;
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
