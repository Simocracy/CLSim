using System;
using System.ComponentModel;
using System.Diagnostics;
using SimpleLogger;

namespace Simocracy.CLSim.Football.Base
{

    /// <summary>
    /// Football Match
    /// </summary>
    /// <remarks>By Laserdisc/Flux, adapted from Simocracy Sport Simulator</remarks>
    [DebuggerDisplay("Match:{" + nameof(Name) + "}")]
    public class FootballMatch : INotifyPropertyChanged
    {
        #region Members

        private FootballTeam _TeamA;
        private FootballTeam _TeamB;
        private int? _ResultA;
        private int? _ResultB;
        private DateTime _Date;
        private string _Stadium;
        private string _City;
        private string _Refere;

        private bool _IsSimulated;

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
        public virtual int? ResultA
        {
            get => _ResultA;
            set
            {
                _ResultA = value;
                Notify();
                if(value >= 0) IsSimulated = true;
            }
        }

        /// <summary>
        /// Result Team B
        /// </summary>
        public virtual int? ResultB
        {
            get => _ResultB;
            set
            {
                _ResultB = value;
                Notify();
                if (value >= 0) IsSimulated = true;
            }
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
        /// Refere (with Flag)
        /// </summary>
        public string Refere
        {
            get => _Refere;
            set { _Refere = value; Notify(); }
        }

        /// <summary>
        /// True if the match is simulated
        /// </summary>
        public bool IsSimulated
        {
            get => _IsSimulated;
            set { _IsSimulated = value; Notify(); }
        }

        /// <summary>
        /// Match name
        /// </summary>
        public string Name => $"{TeamA.FullName} vs. {TeamB.FullName}";

        /// <summary>
        /// Array with all teams
        /// </summary>
        public FootballTeam[] AllTeams { get; }

        /// <summary>
        /// Match time
        /// </summary>
        protected int MatchTime
        {
            get => _MatchTime;
            set { _MatchTime = value; Notify(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the match
        /// </summary>
        public void Reset()
        {
            Reset(90);
        }

        /// <summary>
        /// Resets the match
        /// </summary>
        /// <param name="time">specific match time</param>
        public void Reset(int time)
        {
            ResultA = null;
            ResultB = null;
            _Ball = 0;
            MatchTime = time;
            _Start = 0;
            _IsSimulated = false;

            SimpleLog.Info($"{GetType()}: {TeamA} vs. {TeamB} initialized.");
        }

        /// <summary>
        /// Swaps the teams
        /// </summary>
        public virtual void SwapTeams()
        {
            SimpleLog.Info($"Swap teams in Football Match {TeamA} vs. {TeamB}");

            var oldTeamA = TeamA;
            var oldResultA = ResultA;
            TeamA = TeamB;
            ResultA = ResultB;
            TeamB = oldTeamA;
            ResultB = oldResultA;
        }

        public override string ToString()
        {
            return $"{TeamA} {ResultA}:{ResultB} {TeamB}";
        }

        #endregion

        #region Simulation

        /// <summary>
        /// Simulates the Match
        /// </summary>
        public virtual void Simulate()
        {
            SimpleLog.Info($"Simulate Match: TeamA={TeamA}, TeamB={TeamB}");

            var res = MatchSim(MatchTime);

            ResultA = res.Item1;
            ResultB = res.Item2;

            SimpleLog.Info($"Match Result: ResultA={ResultA}, ResultB={ResultB}");
        }

        /// <summary>
        /// Simulates a match the given time and returns the result for team A and B
        /// </summary>
        /// <param name="time">time to simulate</param>
        /// <returns>Tuple with result for team A and B</returns>
        protected Tuple<int, int> MatchSim(int time)
        {
            int resA = 0;
            int resB = 0;

            _Ball = Kickoff();
            for (int i = 1; i <= time; i++)
            {
                if (i == time / 2)
                {
                    if (_Ball == TorA)
                        resA++;
                    else if (_Ball == TorB)
                        resB++;
                    _Ball = _Start;
                }

                switch (_Ball)
                {
                    case TorwartA:
                        _Ball = Turn(TeamA.GoalkeeperStrength, TeamB.ForwardStrength);
                        break;
                    case AbwehrA:
                        _Ball = Turn(TeamA.DefenseStrength, TeamB.MidfieldStrength);
                        break;
                    case MittelfeldA:
                        _Ball = Turn(TeamA.MidfieldStrength, TeamB.DefenseStrength);
                        break;
                    case SturmA:
                        _Ball = Turn(TeamA.ForwardStrength, TeamB.GoalkeeperStrength + TeamB.DefenseStrength / 2);
                        break;
                    case TorA:
                        resA++;
                        _Ball = MittelfeldB;
                        break;
                    case TorwartB:
                        _Ball = Turn(TeamB.GoalkeeperStrength, TeamA.ForwardStrength);
                        break;
                    case AbwehrB:
                        _Ball = Turn(TeamB.DefenseStrength, TeamA.MidfieldStrength);
                        break;
                    case MittelfeldB:
                        _Ball = Turn(TeamB.MidfieldStrength, TeamA.DefenseStrength);
                        break;
                    case SturmB:
                        _Ball = Turn(TeamB.ForwardStrength, TeamA.GoalkeeperStrength + TeamA.DefenseStrength / 2);
                        break;
                    case TorB:
                        resB++;
                        _Ball = MittelfeldA;
                        break;
                }
            }

            return Tuple.Create(resA, resB);
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
