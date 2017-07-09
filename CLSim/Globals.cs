using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Simocracy.CLSim
{
    public static partial class Globals
    {

        #region Global Variables

        private static readonly ThreadLocal<Random> _Random = new ThreadLocal<Random>(() => new Random());

        /// <summary>
        /// Returns a <see cref="System.Random"/> instance
        /// </summary>
        public static Random Random => _Random.Value;

        #endregion

        #region Program Infos

        /// <summary>
        /// Returns the current program version
        /// </summary>
        public static string ProgramVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                var versionString = $"{version.Major}.{version.Minor}.{version.Revision}";
#if DEBUG
                versionString = $"{versionString} Debug Build";
#endif
                return versionString;
            }
        }

        /// <summary>
        /// Current program name
        /// </summary>
        public static string ProgramName => ((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title;

        #endregion

        #region Credits Infos

        /// <summary>
        /// Main Developer
        /// </summary>
        public static string Developer => "Gobo77";

        /// <summary>
        /// Link to the current program
        /// </summary>
        public static string ProgramLink => "https://github.com/Simocracy/CLSim";

        /// <summary>
        /// Returns the information about the program and all external libs.
        /// Schema: LibName, Lib URL, License, License URL
        /// </summary>
        public static LibInfo[] ProgramInfos => new[]
        {
            new LibInfo(ProgramName, ProgramLink, "MIT-Lizenz","https://github.com/Simocracy/CLSim/blob/master/LICENSE"),
            new LibInfo("SimpleLog", "http://www.codeproject.com/Tips/585796/Simple-Log", "MIT-Lizenz","https://opensource.org/licenses/mit-license.php"),
            new LibInfo("Simocracy Datumsrechner", "https://github.com/Simocracy/Datumsrechner", "MIT-Lizenz","https://github.com/Simocracy/Datumsrechner/blob/master/LICENSE"),
        };

        /// <summary>
        /// The format string for the credits
        /// </summary>
        internal static string CreditsFormatStr => "$1";

        /// <summary>
        /// Github name
        /// </summary>
        public static string GithubName => "Github";

        /// <summary>
        /// IRC channel
        /// </summary>
        public static string IrcChannel => "#Simocracy";

        /// <summary>
        /// Link to the IRC channel
        /// </summary>
        public static string IrcChannelLink => "ircs://irc.newerairc.net:6697/Simocracy";

        /// <summary>
        /// Name of the IRC network
        /// </summary>
        public static string IrcNetworkName => "NewEraIRC";

        /// <summary>
        /// Link to the IRC network
        /// </summary>
        public static string IrcNetworkLink => "https://newerairc.net/";

        #endregion

        #region Global Methods

        /// <summary>
        /// Checks if the given string is a valid HTTP(S) url
        /// Prüft, ob der String eine gültige HTTP(S)-URL ist
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>True if URL is valid</returns>
        public static bool CheckValidHttpUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        #endregion


    }
}
