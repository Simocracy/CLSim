using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Simocracy.CLSim.Football.Base;
using SimpleLogger;

namespace Simocracy.CLSim.Football.UAFA
{
    /// <summary>
    /// Simulates the UAFA Champions League in the mode since 2051/52
    /// </summary>
    public class ChampionsLeague : INotifyPropertyChanged
    {

        #region Members

        private ObservableCollection<FootballTeam> _AllTeamsRaw;

        private ObservableCollection<FootballLeague> _Groups;
        private bool _IsGroupsSimulatable;

        private ObservableCollection<DoubleMatch> _RoundOf16;
        private bool _IsRoundOf16Simulatable;
        private ObservableCollection<DoubleMatch> _QuarterFinal;
        private ObservableCollection<DoubleMatch> _SemiFinal;
        private FootballMatch _Final;

        private const int TeamsPerGroup = 5;
        private const int GroupCount = 8;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new CL instance
        /// </summary>
        public ChampionsLeague()
        {
            IsGroupsSimulatable = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// All Teams in raw order
        /// </summary>
        public ObservableCollection<FootballTeam> AllTeamsRaw
        {
            get => _AllTeamsRaw;
            set
            {
                _AllTeamsRaw = value;
                Notify();
            }
        }

        /// <summary>
        /// CL Groups from A to H, all groups should have 5 members!
        /// </summary>
        public ObservableCollection<FootballLeague> Groups
        {
            get => _Groups;
            set
            {
                _Groups = value;
                Notify();
            }
        }

        /// <summary>
        /// True if group drawing was successfully
        /// </summary>
        public bool IsGroupsSimulatable
        {
            get => _IsGroupsSimulatable;
            private set
            {
                _IsGroupsSimulatable = value;
                Notify();
            }
        }

        /// <summary>
        /// Round of 16
        /// </summary>
        public ObservableCollection<DoubleMatch> RoundOf16
        {
            get => _RoundOf16;
            set
            {
                _RoundOf16 = value;
                Notify();
            }
        }        /// <summary>
        /// True if group drawing was successfully
        /// </summary>
        public bool IsRoundOf16Simulatable
        {
            get => _IsRoundOf16Simulatable;
            private set
            {
                _IsRoundOf16Simulatable = value;
                Notify();
            }
        }

        /// <summary>
        /// Quarter Final
        /// </summary>
        public ObservableCollection<DoubleMatch> QuarterFinal
        {
            get => _QuarterFinal;
            set
            {
                _QuarterFinal = value;
                Notify();
            }
        }

        /// <summary>
        /// Semi Final
        /// </summary>
        public ObservableCollection<DoubleMatch> SemiFinal
        {
            get => _SemiFinal;
            set
            {
                _SemiFinal = value;
                Notify();
            }
        }

        /// <summary>
        /// Final
        /// </summary>
        public FootballMatch Final
        {
            get => _Final;
            set
            {
                Final = value;
                Notify();
            }
        }

        #endregion

        #region Group Drawing

        /// <summary>
        /// Draw groups. If validation not succesfull, <paramref name="tryCount"/> times will be tried.
        /// The last executed try will be used for match recreation.
        /// </summary>
        /// <param name="tryCount">Draw tries</param>
        public void DrawGroups(int tryCount = 5)
        {
            for(int trie = 0; trie < tryCount; trie++)
            {
                var ordered = AllTeamsRaw.OrderBy(x => Globals.Random.Next()).ToArray();

                Groups = new ObservableCollection<FootballLeague>();
                char groupID = 'A';
                for(int i = 0; i < ordered.Length; i += 5)
                {
                    Groups.Add(new FootballLeague(groupID.ToString(), ordered[i], ordered[i + 1],
                        ordered[i + 2], ordered[i + 3], ordered[i + 4]));
                    groupID = (char) (groupID + 1);
                }

                bool[] isNationValid = ValidateGroups();

                IsGroupsSimulatable = isNationValid.Contains(false);

                if(IsGroupsSimulatable)
                    break;
            }

            ResetGroupMatches();
        }

        /// <summary>
        /// Validate the groups (no multiple teams from one state) and tries to switch teams between groups. Returns true for each group if success.
        /// </summary>
        /// <returns>True for each group if success</returns>
        public bool[] ValidateGroups()
        {
            bool[] isNationValid = new bool[Groups.Count];
            bool reValidNeeded = false;
            for(int i = 0; i < Groups.Count; i++)
            {
                var group = Groups[i];
                isNationValid[i] = !AreSameStatesInGroup(group);
                if(isNationValid[i])
                    continue;

                // Switch teams
                for(int teamA = 0; teamA < group.TeamCount - 1; teamA++)
                {
                    for(int teamB = teamA + 1; teamB < group.TeamCount; teamB++)
                    {
                        var res = SwitchTeamGroups(i, teamA, teamB);
                        if(!reValidNeeded)
                            reValidNeeded = res;
                    }
                }

                if(reValidNeeded)
                    isNationValid[i] = !AreSameStatesInGroup(group);
            }

            return isNationValid;
        }

        /// <summary>
        /// Switches teams between groups. If <paramref name="teamA"/> and <paramref name="teamB"/> are from same state, then switch <paramref name="teamA"/> to another group.
        /// Returns true if teams switched.
        /// </summary>
        /// <param name="groupNo">Base group number (group A = 0)</param>
        /// <param name="teamA">Position of Team A</param>
        /// <param name="teamB">Validation position of Team B</param>
        private bool SwitchTeamGroups(int groupNo, int teamA = 0, int teamB = 1)
        {
            var group = Groups[groupNo];

            // Check
            if(group.Teams[teamA].State != group.Teams[teamB].State)
                return false;

            // Previous group
            if(groupNo > 0 && Groups[groupNo - 1].Teams[teamA].State != group.Teams[teamA].State)
            {
                var newTeam = Groups[groupNo - 1].Teams[teamA];
                Groups[groupNo - 1].Teams[teamA] = group.Teams[teamA];
                group.Teams[teamA] = newTeam;
                SimpleLog.Info($"Switched teams in groups {Groups[groupNo - 1].ID} and {group.ID}");
            }
            // Next group
            else if(groupNo < Groups.Count-1 && Groups[groupNo + 1].Teams[teamA].State != group.Teams[teamA].State)
            {
                var newTeam = Groups[groupNo + 1].Teams[teamA];
                Groups[groupNo + 1].Teams[teamA] = group.Teams[teamA];
                group.Teams[teamA] = newTeam;
                SimpleLog.Info($"Switched teams in groups {group.ID} and {Groups[groupNo + 1].ID}");
            }
            else return false;

            return true;
        }

        /// <summary>
        /// Checks if 2 teams from the same state in the same group and returns false if not
        /// </summary>
        /// <param name="group">Group</param>
        private bool AreSameStatesInGroup(FootballLeague group)
        {
            for(int i = 0; i < group.TeamCount - 1; i++)
            for(int j = i + 1; j < group.TeamCount; j++)
                if(group.Teams[i].State == group.Teams[j].State)
                    return true;
            return false;
        }

        /// <summary>
        /// Resets the group matches und creates the new match plan
        /// </summary>
        public void ResetGroupMatches()
        {
            foreach(var g in Groups)
                g.CreateMatches();
        }

        #endregion

        #region Group Simulation

        /// <summary>
        /// Simulates all Groups
        /// </summary>
        public void SimulateGroups()
        {
            if(IsGroupsSimulatable && Groups != null)
            {
                SimpleLog.Info("Simulate all CL groups.");
                foreach(var group in Groups)
                {
                    group.Simulate();
                    group.CalculateTable();
                }
            }
        }

        /// <summary>
        /// Simulates all groups async
        /// </summary>
        public async void SimulateGroupsAsync()
        {
            await Task.Run(() => SimulateGroups());
        }

        #endregion

        #region Draw KO

        /// <summary>
        /// Draw round of 16. If validation not succesfull, <paramref name="tryCount"/> times will be tried.
        /// The last executed try will be used for match recreation.
        /// </summary>
        /// <param name="tryCount">Draw tries</param>
        public void DrawRoundOf16(int tryCount = 5)
        {
            // get group results
            var firsts = Groups.Select(g => g.Table.Pos1).ToArray();
            var secs = Groups.Select(g => g.Table.Pos2).ToArray();

            for(int trie = 0; trie < tryCount; trie++)
            {
                firsts = firsts.OrderBy(x => Globals.Random.Next()).ToArray();
                secs = secs.OrderBy(x => Globals.Random.Next()).ToArray();

                RoundOf16 = new ObservableCollection<DoubleMatch>();
                for(int i = 0; i < firsts.Length; i++)
                    RoundOf16.Add(new DoubleMatch(secs[i], firsts[i]));

                bool[] isNationValid = ValidateGroupOf16();

                IsRoundOf16Simulatable = isNationValid.All(x => x);

                if(IsRoundOf16Simulatable)
                    break;
            }
        }

        /// <summary>
        /// Validate the Round of 16 (no multiple teams from one state or group) and tries to switch teams between matches.
        /// Returns true for each match if success.
        /// </summary>
        /// <returns>True for each match if success</returns>
        public bool[] ValidateGroupOf16()
        {
            bool[] isMatchValid = new bool[RoundOf16.Count];
            bool reValidNeeded = false;
            for(int i = 0; i < RoundOf16.Count; i++)
            {
                var match = RoundOf16[i];
                isMatchValid[i] = IsMatchValid(match);
                if(isMatchValid[i])
                    continue;

                // Switch teams
                var res = SwitchRoundOf16TeamMatches(i);
                if(!reValidNeeded)
                    reValidNeeded = res;

                if(reValidNeeded)
                    isMatchValid[i] = IsMatchValid(match);
            }

            return isMatchValid;
        }

        /// <summary>
        /// Switches teams between matches. If TeamA and TeamB are from same state or group,
        /// then switch TeamA to another match.
        /// Returns true if teams switched.
        /// </summary>
        /// <param name="matchNo">Match number</param>
        private bool SwitchRoundOf16TeamMatches(int matchNo)
        {
            var match = RoundOf16[matchNo];

            // Check
            if(IsMatchValid(match))
                return false;

            // Previous match
            if(matchNo > 0 && RoundOf16[matchNo - 1].TeamA.State != match.TeamA.State)
            {
                var newTeam = RoundOf16[matchNo - 1].TeamA;
                RoundOf16[matchNo - 1] = new DoubleMatch(match.TeamA, RoundOf16[matchNo - 1].TeamB);
                RoundOf16[matchNo] = new DoubleMatch(newTeam, match.TeamB);
                SimpleLog.Info($"Switched teams in Round of 16 matches {matchNo-1} and {matchNo}");
            }
            // Next group
            else if(matchNo < RoundOf16.Count - 1 && RoundOf16[matchNo + 1].TeamA.State != match.TeamA.State)
            {
                var newTeam = RoundOf16[matchNo + 1].TeamA;
                RoundOf16[matchNo + 1] = new DoubleMatch(match.TeamA, RoundOf16[matchNo + 1].TeamB);
                RoundOf16[matchNo] = new DoubleMatch(newTeam, match.TeamB);
                SimpleLog.Info($"Switched teams in Round of 16 matches {matchNo} and {matchNo+1}");
            }
            else return false;

            return true;
        }

        /// <summary>
        /// Checks if the teams are from the same state or the same group and returns true if not
        /// </summary>
        /// <param name="match">Match</param>
        private bool IsMatchValid(DoubleMatch match)
        {
            if(match.TeamA.State == match.TeamB.State) return false;
            foreach(var g in Groups)
            {
                var valid = g.Teams.Contains(match.TeamA) && g.Teams.Contains(match.TeamB);
                if(valid) return false;
            }
            return true;
        }

        /// <summary>
        /// Resets the matches
        /// </summary>
        public void ResetRoundOf16Matches()
        {
            foreach(var m in RoundOf16)
                m.ResetMatch();
        }

        /// <summary>
        /// Draws the Quarter final
        /// </summary>
        public void DrawQuarterFinal()
        {
            var teams = RoundOf16.Select(m => m.Winner).ToArray();
            teams = teams.OrderBy(t => Globals.Random.Next()).ToArray();

            QuarterFinal = new ObservableCollection<DoubleMatch>();
            for(int i = 0; i < teams.Length; i += 2)
                QuarterFinal.Add(new DoubleMatch(teams[i], teams[i + 1]));
        }

        /// <summary>
        /// Draws the Semi Final
        /// </summary>
        public void DrawSemiFinal()
        {
            var teams = QuarterFinal.Select(m => m.Winner).ToArray();
            teams = teams.OrderBy(t => Globals.Random.Next()).ToArray();

            SemiFinal = new ObservableCollection<DoubleMatch>();
            for(int i = 0; i < teams.Length; i += 2)
                SemiFinal.Add(new DoubleMatch(teams[i], teams[i + 1]));
        }

        #endregion

        #region Simulate KO

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
