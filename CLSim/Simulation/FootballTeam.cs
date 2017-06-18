using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Simocracy.CLSim.Simulation
{

    [DebuggerDisplay("Team=" + nameof(FullName) + ", Strength={" + nameof(Strength) + "}")]
    public class FootballTeam
    {

        #region Properties

        /// <summary>
        /// Team name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Association/Nationality/State of Team
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Team strength
        /// </summary>
        public int Strength { get; set; }

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
        private void SetupTeam(string name, string state, int strength)
        {
            Name = name;
            State = state;
            Strength = strength;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"Team={FullName}, Strength={Strength}";
        }

        #endregion

    }
}
