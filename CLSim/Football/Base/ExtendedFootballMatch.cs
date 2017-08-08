using SimpleLogger;

namespace Simocracy.CLSim.Football.Base
{
    /// <summary>
    /// <see cref="FootballMatch"/> with extra time and penalty shootout
    /// </summary>
    public class ExtendedFootballMatch : FootballMatch
    {

        #region Members

        private int _ExtraMatchTime;

        private int? _RegularResultA;
        private int? _RegularResultB;
        private int? _ExtraResultA;
        private int? _ExtraResultB;
        private int? _PenaltyA;
        private int? _PenaltyB;

        private bool _IsExtraTime;
        private bool _IsPenalty;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new match
        /// </summary>
        /// <param name="teamA">Team A (Home)</param>
        /// <param name="teamB">Team B (Away)</param>
        public ExtendedFootballMatch(FootballTeam teamA, FootballTeam teamB) : base(teamA, teamB)
        {
            PropertyChanged += PropertyChangedPropagator.Create(nameof(RegularResultA), nameof(ResultA), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(ExtraResultA), nameof(ResultA), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(RegularResultB), nameof(ResultB), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(ExtraResultB), nameof(ResultB), Notify);

            PropertyChanged += PropertyChangedPropagator.Create(nameof(RegularResultA), nameof(Winner), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(ExtraResultA), nameof(Winner), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(PenaltyA), nameof(Winner), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(RegularResultB), nameof(Winner), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(ExtraResultB), nameof(Winner), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(PenaltyB), nameof(Winner), Notify);

            Reset();
        }

        /// <summary>
        /// Creates a new match
        /// </summary>
        /// <param name="teamA">Team A (Home)</param>
        /// <param name="teamB">Team B (Away)</param>
        /// <param name="regularMatchTime">Specific regular match time</param>
        /// <param name="extraMatchTime">Specific extra match time</param>
        public ExtendedFootballMatch(FootballTeam teamA, FootballTeam teamB, int regularMatchTime,
                                     int extraMatchTime) : this(teamA, teamB)
        {
            Reset(regularMatchTime, extraMatchTime);
        }

        #endregion

        #region Properties

        /// <summary>
        /// full result Team A
        /// </summary>
        public override int? ResultA => RegularResultA.GetValueOrDefault() + ExtraResultA.GetValueOrDefault();

        /// <summary>
        /// full result Team B
        /// </summary>
        public override int? ResultB => RegularResultB.GetValueOrDefault() + ExtraResultB.GetValueOrDefault();

        /// <summary>
        /// Regular time result Team A
        /// </summary>
        public int? RegularResultA
        {
            get => _RegularResultA;
            set { _RegularResultA = value; Notify(); if (value >= 0) IsSimulated = true; }
        }

        /// <summary>
        /// Regular time result Team B
        /// </summary>
        public int? RegularResultB
        {
            get => _RegularResultB;
            set { _RegularResultB = value; Notify(); if (value >= 0) IsSimulated = true; }
        }

        /// <summary>
        /// Regular time result Team A
        /// </summary>
        public int? ExtraResultA
        {
            get => _ExtraResultA;
            set { _ExtraResultA = value; Notify(); if (value >= 0) IsSimulated = true; }
        }

        /// <summary>
        /// Regular time result Team B
        /// </summary>
        public int? ExtraResultB
        {
            get => _ExtraResultB;
            set { _ExtraResultB = value; Notify(); if (value >= 0) IsSimulated = true; }
        }

        /// <summary>
        /// Penalty Shootout Result Team A, -1 if none
        /// </summary>
        public int? PenaltyA
        {
            get => _PenaltyA;
            set { _PenaltyA = value; Notify(); if (value >= 0) IsSimulated = true; }
        }

        /// <summary>
        /// Penalty Shootout Result Team B, -1 if none
        /// </summary>
        public int? PenaltyB
        {
            get => _PenaltyB;
            set { _PenaltyB = value; Notify(); if (value >= 0) IsSimulated = true; }
        }

        /// <summary>
        /// Extra match time
        /// </summary>
        protected int ExtraMatchTime
        {
            get => _ExtraMatchTime;
            set { _ExtraMatchTime = value; Notify(); }
        }

        /// <summary>
        /// If match is in extra time
        /// </summary>
        public bool IsExtraTime
        {
            get => _IsExtraTime;
            set { _IsExtraTime = value; Notify(); }
        }

        /// <summary>
        /// If match is in penalty shootout
        /// </summary>
        public bool IsPenalty
        {
            get => _IsPenalty;
            set { _IsPenalty = value; Notify(); }
        }

        /// <summary>
        /// Gets the winner team
        /// </summary>
        public FootballTeam Winner => GetWinner();

        #endregion

        #region Methods

        /// <summary>
        /// Resets the match
        /// </summary>
        public new void Reset()
        {
            Reset(90, 30);
        }

        /// <summary>
        /// Resets the match
        /// </summary>
        /// <param name="regularTime">specific regular match time</param>
        /// <param name="extraTime">specific extra match time</param>
        public void Reset(int regularTime, int extraTime)
        {
            RegularResultA = null;
            RegularResultB = null;
            ExtraResultA = null;
            ExtraResultB = null;
            PenaltyA = null;
            PenaltyB = null;
            ExtraMatchTime = extraTime;

            IsExtraTime = false;
            IsPenalty = false;
            
            Reset(regularTime);
        }
        
        /// <summary>
        /// Swaps the teams
        /// </summary>
        public override void SwapTeams()
        {
            base.SwapTeams();

            var oldPenaltyA = PenaltyA;
            PenaltyA = PenaltyB;
            PenaltyB = oldPenaltyA;
        }

        /// <summary>
        /// Returns the winning team
        /// </summary>
        /// <returns>The winner team</returns>
        public FootballTeam GetWinner()
        {
            if(ResultA > ResultB) return TeamA;
            if(ResultB > ResultA) return TeamB;

            if(PenaltyA > PenaltyB) return TeamA;
            if(PenaltyB > PenaltyA) return TeamB;

            return null;
        }

        #endregion

        #region Simulation

        /// <summary>
        /// Simulates the Match with extra time and penalty
        /// </summary>
        public override void Simulate()
        {
            SimpleLog.Info($"Simulate Match with full time: TeamA={TeamA}, TeamB={TeamB}");

            SimulateRegular();

            if (RegularResultA == RegularResultB)
            {
                SimulateExtra();

                if(ExtraResultA == ExtraResultB)
                    SimulatePenaltyShootout();
            }

            SimpleLog.Info($"Full Match Result: ResultA={ResultA}, ResultB={ResultB}");
        }

        /// <summary>
        /// Simulates the regular match time
        /// </summary>
        public void SimulateRegular()
        {
            SimpleLog.Info($"Simulate Match with regular time {MatchTime}: TeamA={TeamA}, TeamB={TeamB}");

            var res = MatchSim(MatchTime);
            RegularResultA = res.Item1;
            RegularResultB = res.Item2;

            SimpleLog.Info($"Match Result: ResultA={RegularResultA}, ResultB={RegularResultB}");
        }

        /// <summary>
        /// Simulates the extra match time
        /// </summary>
        public void SimulateExtra()
        {
            SimpleLog.Info($"Simulate Match with extra time {ExtraMatchTime}: TeamA={TeamA}, TeamB={TeamB}");

            IsExtraTime = true;
            var res = MatchSim(ExtraMatchTime);
            ExtraResultA = res.Item1;
            ExtraResultB = res.Item2;

            SimpleLog.Info($"Match Result: ResultA={ExtraResultA}, ResultB={ExtraResultB}");
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
        public void SimulatePenaltyShootout()
        {
            SimpleLog.Info($"Simulate penalty shootout: TeamA={TeamA}, TeamB={TeamB}");

            IsPenalty = true;
            var totalStrength = TeamA.AvgStrength + TeamB.AvgStrength;

            var firVal = Globals.Random.Next(0, 1);
            var firTeam = firVal == 0 ? TeamA : TeamB;
            var secTeam = firVal == 1 ? TeamA : TeamB;

            // first 5 penalties
            var firstPenalties = 5;
            int remainA = 0, remainB = 0;
            int neededA, neededB;
            int penaltyA = 0, penaltyB = 0;
            for (int i = 0; i < firstPenalties; i++)
            {
                var valueA = Globals.Random.Next(0, totalStrength);
                var valueB = Globals.Random.Next(0, totalStrength);

                if (valueA < firTeam.AvgStrength)
                    penaltyA++;

                // break
                remainA = firstPenalties - i;
                neededB = firstPenalties - penaltyA;
                if (neededB > remainB) break;

                if (valueB < secTeam.AvgStrength)
                    penaltyB++;

                // break
                remainB = firstPenalties - i;
                neededA = firstPenalties - penaltyB;
                if (neededA > remainA) break;
            }

            // additional penalties
            while (penaltyA == penaltyB)
            {
                var valueA = Globals.Random.Next(0, totalStrength);
                var valueB = Globals.Random.Next(0, totalStrength);

                if (valueA < firTeam.AvgStrength)
                    penaltyA++;
                if (valueB < secTeam.AvgStrength)
                    penaltyB++;
            }

            PenaltyA = firVal == 0 ? penaltyA : penaltyB;
            PenaltyB = firVal == 1 ? penaltyA : penaltyB;

            SimpleLog.Info($"Penalty Result: ResultA={PenaltyA}, ResultB={PenaltyB}");
        }

        #endregion

    }
}
