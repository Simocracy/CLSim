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
using Simocracy.CLSim.IO;
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

            var yearRegexMatch = YearRegex.Match(cl.Season);
            var startYear = yearRegexMatch.Groups[2].Value;
            var finalYear = yearRegexMatch.Groups[3].Value.Substring(yearRegexMatch.Groups[3].Value.Length - 2);
            PageTitle = $"{ClPageTitlePrefix} {startYear}/{finalYear}";
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

        /// <summary>
        /// Year regex. Group 1: Whole match, Group 2: start year (4 digits), Group 3: final year (2 oder 4 digits)
        /// </summary>
        private static Regex YearRegex => new Regex(@"((\d{4})\/(\d{2,4}))");

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
                var seasonRegexMatch = YearRegex.Match(Cl.Season);
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
        /// Creates the page content for the given <see cref="ChampionsLeague"/>
        /// </summary>
        /// <returns>True if successfully</returns>
        /// <remarks>
        /// The Variables needed in Raw Page:
        ///  0: season number
        ///  1: start year (4 digits)
        ///  2: final year (2 digits)
        ///  3: full final date string
        ///  4: final city
        ///  5: winner team
        ///  6: group stage out color
        ///  7: round of 16 out color
        ///  8: quarter finals out color
        ///  9: semi finals out color
        /// 10: final color
        /// 11: winner color
        /// 12: last winner
        /// 13: participant table
        /// 14: group team list table
        /// 15-22: Group A-H Codes
        /// 23-30: Round of 16 matches
        /// 31-34: Quarter finals matches
        /// 35: Semi final 1 first leg
        /// 36: Semi final 2 first leg
        /// 37: Semi final 2 second leg
        /// 38: Semi final 1 second leg
        /// 39: Final match
        /// </remarks>
        public bool CreatePageContent()
        {
            SimpleLog.Info($"Create content for page {PageTitle} with Champions League {Cl.Season}.");

            if(Cl.Final == null || !Cl.Final.IsSimulated)
            {
                SimpleLog.Warning($"The given CL {Cl.Season} is not completly simulated.");
                return false;
            }

            try
            {
                var sb = new StringBuilder();

                // base infos
                var participants = GetParticipantsTable();
                var yearRegexMatch = YearRegex.Match(Cl.Season);
                var startYear = yearRegexMatch.Groups[2].Value;
                var finalYear = yearRegexMatch.Groups[3].Value.Substring(yearRegexMatch.Groups[3].Value.Length - 2);
                var groupTeamList = GetGroupTeamList();
                var groupCodes = GetGroupCodes();
                var roundOf16Codes = GetDoubleMatchCodes(Cl.RoundOf16);
                var quarterFinalsCodes = GetDoubleMatchCodes(Cl.QuarterFinals);

                // building
                sb.AppendFormat(RawPageCode,
                    CurrentSeasonNumber, startYear, finalYear, // season no/years
                    Cl.Final.Date.ToLongDateString(), Cl.Final.City, Cl.Final.Winner, // final infos
                    ColorGroupStage.ToString().Substring(3), ColorRoundOf16.ToString().Substring(3), // base colors
                    ColorQuarterFinals.ToString().Substring(3), ColorSemiFinals.ToString().Substring(3), // base colors
                    ColorFinal.ToString().Substring(3), ColorWinner.ToString().Substring(3), // base colors
                    participants.Item1, participants.Item2, groupTeamList, // participants
                    groupCodes[0], groupCodes[1], groupCodes[2], groupCodes[3], // groups
                    groupCodes[4], groupCodes[5], groupCodes[6], groupCodes[7], // groups
                    roundOf16Codes[0], roundOf16Codes[1], roundOf16Codes[2], roundOf16Codes[3], // Round of 16
                    roundOf16Codes[4], roundOf16Codes[5], roundOf16Codes[6], roundOf16Codes[7], // Round of 16
                    quarterFinalsCodes[0], quarterFinalsCodes[1], // Quarter Finals
                    quarterFinalsCodes[2], quarterFinalsCodes[3], // Quarter Finals
                    WikiCodeConverter.ToWikiCode(Cl.SemiFinals[0].FirstLeg), // semi final 1 first leg
                    WikiCodeConverter.ToWikiCode(Cl.SemiFinals[1].FirstLeg), // semi final 2 first leg
                    WikiCodeConverter.ToWikiCode(Cl.SemiFinals[1].SecondLeg), // semi final 2 second leg
                    WikiCodeConverter.ToWikiCode(Cl.SemiFinals[0].SecondLeg), // semi final 1 second leg
                    WikiCodeConverter.ToWikiCode(Cl.Final) // final
                    );

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
            catch (Exception e)
            {
                SimpleLog.Error(e.ToString());
            }

            return false;
        }

        /// <summary>
        /// Creates the wiki page for the given <see cref="ChampionsLeague"/>
        /// </summary>
        /// <param name="cl">The champions league</param>
        /// <returns>True if successfully</returns>
        public static bool CreateWikiPage(ChampionsLeague cl)
        {
            var handler = new ClWikiHandler(cl);
            var succ = handler.GetRawPageCode();
            if(succ) succ = handler.GetClSeasonNumber();
            if(succ) succ = handler.CreatePageContent();
            if(succ) succ = handler.WritePageToWiki();
            return succ;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the participants table values for the champions league with last winner first
        /// </summary>
        /// <returns>Tuple with the last winner and the table</returns>
        /// <remarks>
        /// The team sorting inside a state is based on <see cref="ChampionsLeague.AllTeamsRaw"/>.
        /// The last winner is always the first team, (first) MAC-PV always the last.
        /// </remarks>
        public Tuple<string, string> GetParticipantsTable()
        {
            SimpleLog.Info($"Building participants table for CL season {Cl.Season}.");
            if(Cl.Coefficients.Count <= 0)
                Cl.CalculateCoefficient();

            // get colors
            var rawList = new SortedDictionary<string, Dictionary<FootballTeam, Color>>(StringComparer.CurrentCultureIgnoreCase);
            for(int i = 0; i < Cl.AllTeamsRaw.Count; i++)
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

            // convert for getting wiki code
            var table = WikiCodeConverter.GetUafaClParticipantTable(rawList);
            var tuple = Tuple.Create(rawList.First().Value.First().Key.FullName, table);

            return tuple;
        }

        /// <summary>
        /// Returns the group team list as array
        /// </summary>
        /// <returns>The list</returns>
        private string GetGroupTeamList()
        {
            SimpleLog.Info($"Get group list for CL season {Cl.Season}.");

            var list = new List<string>(Cl.AllTeamsRaw.Count);
            foreach(var g in Cl.Groups)
                foreach(var t in g.Teams)
                    list.Add(t.FullName);

            var table = WikiCodeConverter.GetGroupTeamsTable(list);

            return table;
        }

        /// <summary>
        /// Returns the group wiki codes as list
        /// </summary>
        /// <returns>The list</returns>
        private string[] GetGroupCodes()
        {
            SimpleLog.Info($"Get group codes for CL season {Cl.Season}.");

            var list = new List<string>(8);
            foreach (var g in Cl.Groups)
                list.Add(WikiCodeConverter.ToWikiCode(g, WikiCodeConverter.ELeagueTemplate.AlGruppe));

            return list.ToArray();
        }

        /// <summary>
        /// Returns the group wiki codes as list
        /// </summary>
        /// <returns>The list</returns>
        private string[] GetDoubleMatchCodes(IEnumerable<DoubleMatch> doubleMatches)
        {
            SimpleLog.Info($"Get double matches codes for {Cl.Season}.");

            var dMatches = doubleMatches as DoubleMatch[] ?? doubleMatches.ToArray();
            var list = new List<string>(dMatches.Length);
            int teamIndex = 0;
            foreach (var g in dMatches)
                list.Add(WikiCodeConverter.ToWikiCode(g, ++teamIndex, ++teamIndex));

            return list.ToArray();
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
