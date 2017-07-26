using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simocracy.CLSim.IO
{
    /// <summary>
    /// Handles import of team list in file
    /// </summary>
    public class TeamListFileHandler
    {
        /// <summary>
        /// Reads the given file and returns the team list
        /// </summary>
        /// <param name="fileName">the file name with path</param>
        /// <param name="expectedTeamCount">expected team count in file</param>
        /// <returns>the team list string</returns>
        /// <exception cref="FileNotFoundException">Team file not found</exception>
        /// <exception cref="FileFormatException">Team count is not the expected count</exception>
        public static string ReadTeamList(string fileName, int expectedTeamCount)
        {
            string teamList;
            if(!File.Exists(fileName))
            {
                throw new FileNotFoundException("Team list file not found.", fileName);
            }

            using(var sr = new StreamReader(fileName))
            {
                teamList = sr.ReadToEnd();
            }

            var teamCount = teamList.Split(new[] {Environment.NewLine, "\n", "\r"},
                StringSplitOptions.RemoveEmptyEntries).Length;
            if(expectedTeamCount != teamCount)
            {
                throw new FileFormatException($"Wrong team count in file. Should be {expectedTeamCount}, is {teamCount}.");
            }

            return teamList;
        }
    }
}
