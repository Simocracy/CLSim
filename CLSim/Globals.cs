using System;
using System.Reflection;
using System.Threading;

namespace Simocracy.CLSim
{
    public static class Globals
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
        /// Returns the current program name
        /// </summary>
        public static string ProgramName => ((AssemblyTitleAttribute)Assembly.GetExecutingAssembly().GetCustomAttribute(typeof(AssemblyTitleAttribute))).Title;

        /// <summary>
        /// Name of <see cref="SimpleLogger.SimpleLog"/>
        /// </summary>
        public static string SimpleLogName => "SimpleLog";

        /// <summary>
        /// Link to <see cref="SimpleLogger.SimpleLog"/>
        /// </summary>
        public static string SimpleLogLink => "http://www.codeproject.com/Tips/585796/Simple-Log";

        /// <summary>
        /// Licence of <see cref="SimpleLogger.SimpleLog"/>
        /// </summary>
        public static string SimpleLogLicense => "MIT-Lizenz";

        /// <summary>
        /// Link to the full license text of <see cref="SimpleLogger.SimpleLog"/>
        /// </summary>
        public static string SimpleLogLicenseLink => "https://opensource.org/licenses/mit-license.php";

        /// <summary>
        /// Name of <see cref="Datumsrechner"/>
        /// </summary>
        public static string DRechnerName => "Simocracy Datumsrechner";

        /// <summary>
        /// Link to <see cref="Datumsrechner"/>
        /// </summary>
        public static string DRechnerLink => "https://github.com/Simocracy/Datumsrechner";

        /// <summary>
        /// Licence of <see cref="Datumsrechner"/>
        /// </summary>
        public static string DRechnerLicense => "MIT-Lizenz";

        /// <summary>
        /// Link to the full license text of <see cref="Datumsrechner"/>
        /// </summary>
        public static string DRechnerLicenseLink => "https://github.com/Simocracy/Datumsrechner/blob/master/LICENSE";

        ///// <summary>
        ///// Full credits text
        ///// </summary>
        //public static string FullCreditsText =>
        //    $"<Hyperlink NavigateUri=\"{SimpleLogLink}\" RequestNavigate=\"Hyperlink_RequestNavigate\">{SimpleLogName}</Hyperlink> ist unter der <Hyperlink NavigateUri=\"{SimpleLogLicenseLink}\" RequestNavigate=\"Hyperlink_RequestNavigate\">{SimpleLogLicense}</Hyperlink> lizenziert.{Environment.NewLine}" +
        //    $"<Hyperlink NavigateUri=\"{DRechnerLink}\" RequestNavigate=\"Hyperlink_RequestNavigate\">{DRechnerName}</Hyperlink> ist unter der <Hyperlink NavigateUri=\"{DRechnerLicenseLink}\" RequestNavigate=\"Hyperlink_RequestNavigate\">{DRechnerLicense}</Hyperlink> lizenziert.";

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
