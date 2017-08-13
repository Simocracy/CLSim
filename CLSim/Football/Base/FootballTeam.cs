using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SimpleLogger;

namespace Simocracy.CLSim.Football.Base
{

    [DebuggerDisplay("Team={" + nameof(FullName) + "}, Strength={" + nameof(AvgStrength) + "}")]
    public class FootballTeam : INotifyPropertyChanged
    {
        private string _Name;
        private string _State;
        private int _GoalkeeperStrength;
        private int _DefenseStrength;
        private int _MidfieldStrength;
        private int _ForwardStrength;
        private string _Remark;
        private string _Logo;

        #region Properties

        /// <summary>
        /// Team name
        /// </summary>
        public string Name
        {
            get => _Name;
            set { _Name = value; Notify(); }
        }

        /// <summary>
        /// Association/Nationality/State of Team (without {{}})
        /// </summary>
        public string State
        {
            get => _State;
            set { _State = value; Notify(); }
        }

        /// <summary>
        /// Average team strength.
        /// Calculation via 4-4-2 system.
        /// </summary>
        public int AvgStrength => (GoalkeeperStrength + 4 * DefenseStrength + 4 * MidfieldStrength +
                                   2 * ForwardStrength) / 11;

        /// <summary>
        /// Goalkeeper strength
        /// </summary>
        public int GoalkeeperStrength
        {
            get => _GoalkeeperStrength;
            set { _GoalkeeperStrength = value; Notify(); }
        }

        /// <summary>
        /// Defense strength
        /// </summary>
        public int DefenseStrength
        {
            get => _DefenseStrength;
            set { _DefenseStrength = value; Notify(); }
        }

        /// <summary>
        /// Midfield strength
        /// </summary>
        public int MidfieldStrength
        {
            get => _MidfieldStrength;
            set { _MidfieldStrength = value; Notify(); }
        }

        /// <summary>
        /// Forward strength
        /// </summary>
        public int ForwardStrength
        {
            get => _ForwardStrength;
            set { _ForwardStrength = value; Notify(); }
        }

        /// <summary>
        /// Team remarks
        /// </summary>
        public string Remark
        {
            get => _Remark;
            set { _Remark = value; Notify(); }
        }

        /// <summary>
        /// Team logo
        /// </summary>
        public string Logo
        {
            get => String.IsNullOrWhiteSpace(_Logo) ? $"{{{{State|150}}}}" : _Logo;
            set { _Logo = value; Notify(); }
        }

        /// <summary>
        /// Full name of the team with state
        /// </summary>
        public string FullName => $"{{{{{State}}}}} {Name}";

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a football team.
        /// Scheme of Input String: {{Flag}} Teamname, avgStrength (, Remarks)
        /// </summary>
        /// <param name="inputStr">Input string</param>
        public FootballTeam(string inputStr)
        {
            SetupTeam(inputStr);
        }

        /// <summary>
        /// Creates a football team
        /// </summary>
        /// <param name="inputStr">Input string with name and state</param>
        /// <param name="avgStrength">Team strength</param>
        public FootballTeam(string inputStr, string avgStrength)
        {
            SetupTeam(inputStr, Int32.Parse(avgStrength));
        }

        /// <summary>
        /// Creates a football team
        /// </summary>
        /// <param name="inputStr">Input string with name and state</param>
        /// <param name="avgStrength">Team strength</param>
        public FootballTeam(string inputStr, int avgStrength)
        {
            SetupTeam(inputStr, avgStrength);
        }

        /// <summary>
        /// Creates a football team
        /// </summary>
        /// <param name="name">Team name</param>
        /// <param name="state">Association/Nationality/State of Team</param>
        /// <param name="avgStrength">Team strength</param>
        public FootballTeam(string name, string state, int avgStrength)
        {
            SetupTeam(name, state, avgStrength);
        }

        /// <summary>
        /// Set ups a football team. Scheme: {{Flag}} Team Name, Strenght (, Remarks)
        /// </summary>
        /// <param name="inputStr">Input string</param>
        private void SetupTeam(string inputStr)
        {
            var regexMatch = Regex.Match(inputStr, @"\{\{([^\}]+)\}\}\s*([^\,]+)\s*,\s*(\d+)(\s*,\s*(.*))?");
            SetupTeam(regexMatch.Groups[2].Value, regexMatch.Groups[1].Value, Int32.Parse(regexMatch.Groups[3].Value), regexMatch.Groups[5].Value);
        }

        /// <summary>
        /// Set ups a football team
        /// </summary>
        /// <param name="inputStr">Input string with name and state</param>
        /// <param name="avgStrength">Team strength</param>
        private void SetupTeam(string inputStr, int avgStrength)
        {
            var regexMatch = Regex.Match(inputStr, @"\{\{([^\}]+)\}\} (.+)");
            SetupTeam(regexMatch.Groups[2].Value, regexMatch.Groups[1].Value, avgStrength);
        }

        /// <summary>
        /// Set ups a football team
        /// </summary>
        /// <param name="name">Team name</param>
        /// <param name="state">Association/Nationality/State of Team</param>
        /// <param name="avgStrength">Team strength</param>
        /// <param name="remarks">Remarks</param>
        /// <remarks>Final Setup Method</remarks>
        private void SetupTeam(string name, string state, int avgStrength, string remarks = null)
        {
            Name = name;
            State = state;
            GoalkeeperStrength = DefenseStrength = MidfieldStrength = ForwardStrength = avgStrength;
            Remark = (!String.IsNullOrEmpty(remarks)) ? remarks : String.Empty;

            PropertyChanged += PropertyChangedPropagator.Create(nameof(Name), nameof(FullName), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(Name), nameof(FullName), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(State), nameof(FullName), Notify);
            PropertyChanged += PropertyChangedPropagator.Create(nameof(State), nameof(FullName), Notify);

            SimpleLog.Info($"{this} created.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the <see cref="FullName"/> for this Team
        /// </summary>
        /// <returns>The team name</returns>
        public override string ToString()
        {
            return FullName;
        }

        /// <summary>
        /// Gets the wiki code for this team with remarks
        /// </summary>
        /// <returns>wiki code</returns>
        public string GetWikiCodeWithRemarks()
        {
            if (String.IsNullOrWhiteSpace(Remark))
                return $"{FullName}<ref group=\"A\">{Remark}</ref>";
            else
                return FullName;
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
