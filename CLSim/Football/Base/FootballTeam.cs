using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SimpleLogger;

namespace Simocracy.CLSim.Football.Base
{

    [DebuggerDisplay("Team={" + nameof(FullName) + "}, Strength={" + nameof(Strength) + "}")]
    public class FootballTeam : INotifyPropertyChanged
    {
        private string _Name;
        private string _State;
        private int _Strength;

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
        /// Association/Nationality/State of Team
        /// </summary>
        public string State
        {
            get => _State;
            set { _State = value; Notify(); }
        }

        /// <summary>
        /// Team strength
        /// </summary>
        public int Strength
        {
            get => _Strength;
            set { _Strength = value; Notify(); }
        }

        /// <summary>
        /// Full name of the team with state
        /// </summary>
        public string FullName => $"{{{{{State}}}}} {Name}";

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a football team
        /// </summary>
        /// <param name="inputStr">Input string with name and state</param>
        /// <param name="strength">Team strength</param>
        public FootballTeam(string inputStr, string strength)
        {
            SetupTeam(inputStr, Int32.Parse(strength));
        }

        /// <summary>
        /// Creates a football team
        /// </summary>
        /// <param name="inputStr">Input string with name and state</param>
        /// <param name="strength">Team strength</param>
        public FootballTeam(string inputStr, int strength)
        {
            SetupTeam(inputStr, strength);
        }

        /// <summary>
        /// Creates a football team
        /// </summary>
        /// <param name="name">Team name</param>
        /// <param name="state">Association/Nationality/State of Team</param>
        /// <param name="strength">Team strength</param>
        public FootballTeam(string name, string state, int strength)
        {
            SetupTeam(name, state, strength);
        }

        /// <summary>
        /// Set ups a football team
        /// </summary>
        /// <param name="inputStr">Input string with name and state</param>
        /// <param name="strength">Team strength</param>
        private void SetupTeam(string inputStr, int strength)
        {
            var regexMatch = Regex.Match(inputStr, @"(\{\{[^\}]+\}\}) (.+)");
            SetupTeam(regexMatch.Groups[2].Value, regexMatch.Groups[1].Value, strength);
        }

        /// <summary>
        /// Set ups a football team
        /// </summary>
        /// <param name="name">Team name</param>
        /// <param name="state">Association/Nationality/State of Team</param>
        /// <param name="strength">Team strength</param>
        /// <remarks>Final Setup Method</remarks>
        private void SetupTeam(string name, string state, int strength)
        {
            Name = name;
            State = state;
            Strength = strength;

            SimpleLog.Info($"{this} created.");
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"Football Team={FullName}, Strength={Strength}";
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
