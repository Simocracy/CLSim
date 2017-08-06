using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using DotNetWikiBot;
using Simocracy.PwrBot;
using SimpleLogger;

namespace Simocracy.CLSim.PwrBot
{
    /// <summary>
    /// Wiki handler class for UAFA Champions League
    /// </summary>
    public class ClWikiHandler : INotifyPropertyChanged
    {

        #region Constants

        public const string CurrentClRawPageTitle = "CLSim/UAFA CL Vorlage 2054";
        public const string ClPageTitlePrefix = "UAFA Champions League";

        #endregion

        #region Members

        private static Site _Site;
        private string _RawPageCode;
        private int _CurrentSeasonNumber;

        private string _PageTitle;
        private string _PageContent;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new cl wiki handler instance
        /// </summary>
        /// <param name="pageTitle">The page title for the page which will be created</param>
        public ClWikiHandler(string pageTitle)
        {
            PageTitle = pageTitle;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Site for the Bot
        /// </summary>
        public static Site Site => _Site ?? (_Site = new Site("https://simocracy.de", PwrBotLoginData.Username, PwrBotLoginData.Password));

        /// <summary>
        /// Raw wiki page code
        /// </summary>
        public string RawPageCode
        {
            get => _RawPageCode;
            set { _RawPageCode = value; Notify(); }
        }

        /// <summary>
        /// Current season number
        /// </summary>
        public int CurrentSeasonNumber
        {
            get => _CurrentSeasonNumber;
            set { _CurrentSeasonNumber = value; Notify(); }
        }

        /// <summary>
        /// Page Content
        /// </summary>
        public string PageContent
        {
            get => _PageContent;
            set { _PageContent = value; Notify(); }
        }

        /// <summary>
        /// Page Title
        /// </summary>
        public string PageTitle
        {
            get => _PageTitle;
            private set { _PageTitle = value; Notify(); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the raw page code for the CL site
        /// </summary>
        /// <param name="rawPageName">Page Name for raw code</param>
        /// <returns>True if successfully</returns>
        public bool GetRawPageCode(string rawPageName)
        {
            SimpleLog.Info($"Get raw page code from {rawPageName}.");
            var haveCode = false;

            try
            {
                RawPageCode = String.Empty;
                Page p = new Page(Site, rawPageName);
                p.Load();
                RawPageCode = p.text;

                haveCode = !String.IsNullOrWhiteSpace(RawPageCode);
            }
            catch(Exception e)
            {
                SimpleLog.Error(e.ToString());
            }
            SimpleLog.Info($"Have raw page code from {rawPageName}: {haveCode}");
            return haveCode;
        }

        /// <summary>
        /// Searches for the current cl season number
        /// </summary>
        /// <param name="currentSeason">current season years</param>
        /// <returns>True if succesfully</returns>
        public bool GetClSeasonNumber(string currentSeason)
        {
            SimpleLog.Info($"Get current cl season number for season {currentSeason}.");

            try
            {
                var seasonRegexMatch = Regex.Match(currentSeason, @"((\d{4})\/(\d{2}))");
                if(!seasonRegexMatch.Success)
                {
                    SimpleLog.Warning($"Wrong season format: {currentSeason}");
                    return false;
                }

                var lastSeason = $"{Int32.Parse(seasonRegexMatch.Groups[2].Value) - 1}/{seasonRegexMatch.Groups[2].Value.Substring(2)}";

                Page p = new Page(Site, $"{ClPageTitlePrefix} {lastSeason}");
                p.Load();
                var seasonNumberMatch = Regex.Match(p.text, @"Die (\d{2})\. Saison der").Groups[1];
                if(!seasonNumberMatch.Success)
                {
                    SimpleLog.Warning($"Season cannot be readed from: {p.title}");
                    return false;
                }

                CurrentSeasonNumber = Int32.Parse(seasonNumberMatch.Value);
                SimpleLog.Info("Current season number readed.");
                return true;
            }
            catch(Exception e)
            {
                SimpleLog.Error(e.ToString());
            }

            return false;
        }

        /// <summary>
        /// Saves the page to the wiki and overrides existing pages
        /// </summary>
        /// <returns>True if successfully</returns>
        public bool WritePageToWiki()
        {
            SimpleLog.Info($"Write article {PageTitle} to wiki.");

            try
            {
                Page p = new Page(Site, PageTitle);
                p.text = PageContent;
                p.Save("CLSim simulation", false);

                SimpleLog.Info($"Article {PageTitle} writed to wiki.");
                return true;
            }
            catch(Exception e)
            {
                SimpleLog.Error(e.ToString());
            }

            return false;
        }

        public bool CreatePageContent()
        {
            SimpleLog.Info($"Create content for page {PageTitle}.");

            try
            {

                SimpleLog.Info($"Page content for {PageTitle} created.");
                return true;
            }
            catch(Exception e)
            {
                SimpleLog.Error($"Error during creating page content for {PageTitle}");
                SimpleLog.Error(e.ToString());
            }

            return false;
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
