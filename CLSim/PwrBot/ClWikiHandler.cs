using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using DotNetWikiBot;
using Simocracy.CLSim.Football.Base;
using Simocracy.CLSim.Football.UAFA;
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

        public static readonly Color ColorGroupStage = Color.FromRgb(0xe5, 0xb1, 0xb1);
        public static readonly Color ColorRoundOf32 = Color.FromRgb(0xff, 0xda, 0xda);
        public static readonly Color ColorRoundOf16 = Color.FromRgb(0xff, 0xdf, 0xb1);
        public static readonly Color ColorQuarterFinals = Color.FromRgb(0xff, 0xff, 0xb1);
        public static readonly Color ColorSemiFinals = Color.FromRgb(0xb1, 0xee, 0xb1);
        public static readonly Color ColorFinal = Color.FromRgb(0x9f, 0xd3, 0x9f);
        public static readonly Color ColorWinner = Color.FromRgb(0xb1, 0xee, 0xff);

        #endregion

        #region Members

        private static Site _Site;
        private string _RawPageCode;
        private int _CurrentSeasonNumber;
        private ChampionsLeague _Cl;

        private string _PageTitle;
        private string _PageContent;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new cl wiki handler instance
        /// </summary>
        public ClWikiHandler()
        {
        }

        /// <summary>
        /// Creates a new cl wiki handler instance
        /// </summary>
        /// <param name="cl">The <see cref="ChampionsLeague"/> to use</param>
        public ClWikiHandler(ChampionsLeague cl)
        {
            Cl = cl;
            PageTitle = $"{ClPageTitlePrefix} {cl.Season}";
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Site for the Bot
        /// </summary>
        public static Site Site => _Site ?? (_Site = new Site("https://simocracy.de", PwrBotLoginData.Username, PwrBotLoginData.Password));

        /// <summary>
        /// The <see cref="ChampionsLeague"/> to export
        /// </summary>
        public ChampionsLeague Cl
        {
            get => _Cl;
            set { _Cl = value; Notify(); }
        }

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

        #region Page Methods

        /// <summary>
        /// Gets the raw page code for the CL site
        /// </summary>
        /// <returns>True if successfully</returns>
        public bool GetRawPageCode()
        {
            SimpleLog.Info($"Get raw page code from {CurrentClRawPageTitle}.");
            var haveCode = false;

            try
            {
                RawPageCode = String.Empty;
                Page p = new Page(Site, CurrentClRawPageTitle);
                p.Load();
                RawPageCode = p.text;

                haveCode = !String.IsNullOrWhiteSpace(RawPageCode);
            }
            catch (Exception e)
            {
                SimpleLog.Error(e.ToString());
            }
            SimpleLog.Info($"Have raw page code from {CurrentClRawPageTitle}: {haveCode}");
            return haveCode;
        }

        /// <summary>
        /// Searches for the current cl season number
        /// </summary>
        /// <returns>True if succesfully</returns>
        public bool GetClSeasonNumber()
        {
            SimpleLog.Info($"Get current cl season number for season {Cl.Season}.");

            try
            {
                var seasonRegexMatch = Regex.Match(Cl.Season, @"((\d{4})\/(\d{2}))");
                if (!seasonRegexMatch.Success)
                {
                    SimpleLog.Warning($"Wrong season format: {Cl.Season}");
                    return false;
                }

                var lastSeason = $"{Int32.Parse(seasonRegexMatch.Groups[2].Value) - 1}/{seasonRegexMatch.Groups[2].Value.Substring(2)}";

                Page p = new Page(Site, $"{ClPageTitlePrefix} {lastSeason}");
                p.Load();
                var seasonNumberMatch = Regex.Match(p.text, @"Die (\d{2})\. Saison der").Groups[1];
                if (!seasonNumberMatch.Success)
                {
                    SimpleLog.Warning($"Season cannot be readed from: {p.title}");
                    return false;
                }

                CurrentSeasonNumber = Int32.Parse(seasonNumberMatch.Value) + 1;
                SimpleLog.Info("Current season number readed.");
                return true;
            }
            catch (Exception e)
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

        /// <summary>
        /// Creates the page content for the given <see cref="ChampionsLeague"/>
        /// </summary>
        /// <param name="cl">The CL complete simulated instance</param>
        /// <returns>True if successfully</returns>
        /// <remarks>
        /// The Variables needed in Raw Page:
        ///   0: season number
        ///   1: start year (4 digits)
        ///   2: final year (2 digits)
        ///   3: full final date string
        ///   4: final city
        ///   5: winner team
        ///   6: last winner
        ///   7: group stage out color
        ///   8: round of 16 out color
        ///   9: quarter finals out color
        ///  10: semi finals out color
        ///  11: final color
        ///  12: winner color
        ///  13: last winner out color
        ///  14- 89: team out color / team with remarks
        ///  90- 91: MAC-PV team out color /team with remarks
        ///  92-131: Group A-H teams
        /// 132-139: Group A-H Codes
        /// 140-147: Round of 16 matches
        /// 148-151: Quarter finals matches
        /// 152: Semi final 1 first leg
        /// 153: Semi final 2 first leg
        /// 154: Semi final 2 second leg
        /// 155: Semi final 1 second leg
        /// 156: Final match
        /// </remarks>
        public bool CreatePageContent(ChampionsLeague cl)
        {
            SimpleLog.Info($"Create content for page {PageTitle} with Champions League {cl.Season}.");

            if(cl.Final == null || !cl.Final.IsSimulated)
            {
                SimpleLog.Warning($"The given CL {cl.Season} is not completly simulated.");
                return false;
            }

            try
            {
                var sb = new StringBuilder();


                PageContent = sb.ToString();

                SimpleLog.Info($"Page content for {PageTitle} created.");
                return true;
            }
            catch(Exception e)
            {
                SimpleLog.Error($"Error during creating page content for {PageTitle}.");
                SimpleLog.Error(e.ToString());
            }

            return false;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the participants table values for the champions league
        /// </summary>
        /// <returns>the table values</returns>
        /// <remarks>
        /// The team sorting inside a state is based on <see cref="ChampionsLeague.AllTeamsRaw"/>.
        /// The list is sorted by state.
        /// The last winner is always the first team, MAC-PV always the last.
        /// </remarks>
        public Dictionary<FootballTeam, Color> GetParticipantsTable()
        {
            SimpleLog.Info($"Building participants table for CL season {Cl.Season}");
            if(Cl.Coefficients.Count <= 0)
                Cl.CalculateCoefficient();

            // get colors
            var rawList = new SortedDictionary<string, Dictionary<FootballTeam, Color>>(StringComparer.CurrentCultureIgnoreCase);
            for(int i = Cl.AllTeamsRaw.Count - 1; i >= 0; i--)
            {
                var team = Cl.AllTeamsRaw[i];

                var reached = Cl.Coefficients[team].GetReachedClRound();
                var color = ColorGroupStage;
                switch(reached)
                {
                    case ETournamentRound.CLGroupStage:
                        color = ColorGroupStage;
                        break;
                    case ETournamentRound.CLRoundOf16:
                        color = ColorRoundOf16;
                        break;
                    case ETournamentRound.CLQuarterFinals:
                        color = ColorQuarterFinals;
                        break;
                    case ETournamentRound.CLSemiFinals:
                        color = ColorSemiFinals;
                        break;
                    case ETournamentRound.CLFinal:
                        color = ColorFinal;
                        break;
                }
                if(team == Cl.Final.Winner)
                    color = ColorWinner;

                var stateIndex = team.State;
                if(i == 0) // tv
                    stateIndex = "aaatv";
                else if(team.State.ToLower() == "mac-pv" && !rawList.ContainsKey("zzzmacpv")) // MAC-PV team
                    stateIndex = "zzzmacpv";

                if(!rawList.ContainsKey(stateIndex))
                    rawList[stateIndex] = new Dictionary<FootballTeam, Color>();
                rawList[stateIndex][team] = color;
            }

            var finalDic = new Dictionary<FootballTeam, Color>();
            foreach(var r in rawList.Values)
                foreach(var d in r)
                    finalDic.Add(d.Key, d.Value);

            return finalDic;
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
