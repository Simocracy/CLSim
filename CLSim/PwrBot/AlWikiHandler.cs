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
    /// Wiki handler class for UAFA America League
    /// </summary>
    public class AlWikiHandler : INotifyPropertyChanged
    {

        #region Constants

        public const string CurrentAlRawPageTitle = "CLSim/UAFA AL Vorlage 2055";
        public const string AlPageTitlePrefix = "UAFA America League";

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
        private AmericaLeague _Al;

        private string _PageTitle;
        private string _PageContent;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new al wiki handler instance
        /// </summary>
        public AlWikiHandler()
        {
        }

        /// <summary>
        /// Creates a new al wiki handler instance
        /// </summary>
        /// <param name="al">The <see cref="AmericaLeague"/> to use</param>
        public AlWikiHandler(AmericaLeague al)
        {
            Al = al;

            var yearRegexMatch = YearRegex.Match(al.Season);
            var startYear = yearRegexMatch.Groups[2].Value;
            var finalYear = yearRegexMatch.Groups[3].Value.Substring(yearRegexMatch.Groups[3].Value.Length - 2);
            PageTitle = $"{AlPageTitlePrefix} {startYear}/{finalYear}";
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Site for the Bot
        /// </summary>
        public static Site Site => _Site ?? (_Site = new Site("https://simocracy.de", PwrBotLoginData.Username, PwrBotLoginData.Password));

        /// <summary>
        /// The <see cref="AmericaLeague"/> to export
        /// </summary>
        public AmericaLeague Al
        {
            get => _Al;
            set { _Al = value; Notify(); }
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
        /// Gets the raw page code for the AL site
        /// </summary>
        /// <returns>True if successfully</returns>
        public bool GetRawPageCode()
        {
            SimpleLog.Info($"Get raw page code from {CurrentAlRawPageTitle}.");
            var haveCode = false;

            try
            {
                RawPageCode = String.Empty;
                Page p = new Page(Site, CurrentAlRawPageTitle);
                p.Load();
                RawPageCode = p.text;

                haveCode = !String.IsNullOrWhiteSpace(RawPageCode);
            }
            catch (Exception e)
            {
                SimpleLog.Error(e.ToString());
            }
            SimpleLog.Info($"Have raw page code from {CurrentAlRawPageTitle}: {haveCode}");
            return haveCode;
        }

        /// <summary>
        /// Searches for the current al season number
        /// </summary>
        /// <returns>True if succesfully</returns>
        public bool GetClSeasonNumber()
        {
            SimpleLog.Info($"Get current al season number for season {Al.Season}.");

            try
            {
                var seasonRegexMatch = YearRegex.Match(Al.Season);
                if (!seasonRegexMatch.Success)
                {
                    SimpleLog.Warning($"Wrong season format: {Al.Season}");
                    return false;
                }

                var lastSeason = $"{Int32.Parse(seasonRegexMatch.Groups[2].Value) - 1}/{seasonRegexMatch.Groups[2].Value.Substring(2)}";

                Page p = new Page(Site, $"{AlPageTitlePrefix} {lastSeason}");
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
        /// Creates the page content for the given <see cref="AmericaLeague"/>
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
        ///  7: round of 32 out color
        ///  8: round of 16 out color
        ///  9: quarter finals out color
        ///  10: semi finals out color
        /// 11: final color
        /// 12: winner color
        /// 13: last winner
        /// 14: participant table
        /// 15: group team list table
        /// 16-23: Group A-H Codes
        /// 24-39: Round of 32 matches
        /// 40-47: Round of 16 matches
        /// 48-51: Quarter finals matches
        /// 52: Semi final 1 first leg
        /// 53: Semi final 2 first leg
        /// 54: Semi final 2 second leg
        /// 55: Semi final 1 second leg
        /// 56: Final match
        /// </remarks>
        public bool CreatePageContent()
        {
            SimpleLog.Info($"Create content for page {PageTitle} with America League {Al.Season}.");

            if(Al.Final == null || !Al.Final.IsSimulated)
            {
                SimpleLog.Warning($"The given AL {Al.Season} is not completly simulated.");
                return false;
            }

            try
            {
                var sb = new StringBuilder();

                // base infos
                var participants = GetParticipantsTable();
                var yearRegexMatch = YearRegex.Match(Al.Season);
                var startYear = yearRegexMatch.Groups[2].Value;
                var finalYear = yearRegexMatch.Groups[3].Value.Substring(yearRegexMatch.Groups[3].Value.Length - 2);
                var groupTeamList = GetGroupTeamList();
                var groupCodes = GetGroupCodes();
                var roundOf32Codes = GetDoubleMatchCodes(Al.RoundOf32);
                var roundOf16Codes = GetDoubleMatchCodes(Al.RoundOf16);
                var quarterFinalsCodes = GetDoubleMatchCodes(Al.QuarterFinals);

                // building
                sb.AppendFormat(RawPageCode,
                    CurrentSeasonNumber, startYear, finalYear, // season no/years
                    Al.Final.Date.ToLongDateString(), Al.Final.City, Al.Final.Winner, // final infos
                    ColorGroupStage.ToString().Substring(3), ColorRoundOf32.ToString().Substring(3), // base colors
                    ColorRoundOf16.ToString().Substring(3), ColorQuarterFinals.ToString().Substring(3), // base colors
                    ColorSemiFinals.ToString().Substring(3), ColorFinal.ToString().Substring(3), // base colors
                    ColorWinner.ToString().Substring(3), // base colors
                    participants.Item1, participants.Item2, groupTeamList, // participants
                    groupCodes[0], groupCodes[1], groupCodes[2], groupCodes[3], // groups
                    groupCodes[4], groupCodes[5], groupCodes[6], groupCodes[7], // groups
                    roundOf32Codes[0], roundOf32Codes[1], roundOf32Codes[2], roundOf32Codes[3], // Round of 32
                    roundOf32Codes[4], roundOf32Codes[5], roundOf32Codes[6], roundOf32Codes[7], // Round of 32
                    roundOf32Codes[8], roundOf32Codes[9], roundOf32Codes[10], roundOf32Codes[11], // Round of 32
                    roundOf32Codes[12], roundOf32Codes[13], roundOf32Codes[14], roundOf32Codes[15], // Round of 32
                    roundOf16Codes[0], roundOf16Codes[1], roundOf16Codes[2], roundOf16Codes[3], // Round of 16
                    roundOf16Codes[4], roundOf16Codes[5], roundOf16Codes[6], roundOf16Codes[7], // Round of 16
                    quarterFinalsCodes[0], quarterFinalsCodes[1], // Quarter Finals
                    quarterFinalsCodes[2], quarterFinalsCodes[3], // Quarter Finals
                    WikiCodeConverter.ToWikiCode(Al.SemiFinals[0].FirstLeg), // semi final 1 first leg
                    WikiCodeConverter.ToWikiCode(Al.SemiFinals[1].FirstLeg), // semi final 2 first leg
                    WikiCodeConverter.ToWikiCode(Al.SemiFinals[1].SecondLeg), // semi final 2 second leg
                    WikiCodeConverter.ToWikiCode(Al.SemiFinals[0].SecondLeg), // semi final 1 second leg
                    WikiCodeConverter.ToWikiCode(Al.Final) // final
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
        /// Creates the wiki page for the given <see cref="AmericaLeague"/>
        /// </summary>
        /// <param name="al">The champions league</param>
        /// <returns>True if successfully</returns>
        public static bool CreateWikiPage(AmericaLeague al)
        {
            var handler = new AlWikiHandler(al);
            var succ = handler.GetRawPageCode();
            if(succ) succ = handler.GetClSeasonNumber();
            if(succ) succ = handler.CreatePageContent();
            if(succ) succ = handler.WritePageToWiki();
            return succ;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the participants table values for the america league with last winner first
        /// </summary>
        /// <returns>Tuple with the last winner and the table</returns>
        /// <param name="startYear">Start year(4 digits)</param>
        /// <param name="finalYear">Final year(2 digits)</param>
        /// <remarks>
        /// The team sorting inside a state is based on <see cref="AmericaLeague.AllTeamsRaw"/>.
        /// The last winner is always the first team, (first) MAC-PV always the last, and after this the 8 CL relegation teams.
        /// </remarks>
        public Tuple<string, string> GetParticipantsTable(int startYear = 0, int finalYear = 0)
        {
            SimpleLog.Info($"Building participants table for AL season {Al.Season}.");
            if(Al.Coefficients.Count <= 0)
                Al.CalculateCoefficient();

            // get colors
            var rawList = new SortedDictionary<string, Dictionary<FootballTeam, Color>>(StringComparer.CurrentCultureIgnoreCase);
            for(int i = 0; i < Al.AllTeamsRaw.Count; i++)
            {
                var team = Al.AllTeamsRaw[i];

                var reached = Al.Coefficients[team].GetReachedAlRound();
                var color = ColorGroupStage;
                switch(reached)
                {
                    case ETournamentRound.ALGroupStage:
                        color = ColorGroupStage;
                        break;
                    case ETournamentRound.ALRoundOf32:
                        color = ColorRoundOf32;
                        break;
                    case ETournamentRound.ALRoundOf16:
                        color = ColorRoundOf16;
                        break;
                    case ETournamentRound.ALQuarterFinals:
                        color = ColorQuarterFinals;
                        break;
                    case ETournamentRound.ALSemiFinals:
                        color = ColorSemiFinals;
                        break;
                    case ETournamentRound.ALFinal:
                        color = ColorFinal;
                        break;
                }
                if(team == Al.Final.Winner)
                    color = ColorWinner;

                var stateIndex = team.State;
                if(i == 0) // tv
                    stateIndex = WikiCodeConverter.TeamStateTvKey;
                else if(i >= 40) // cl relegations
                    stateIndex = WikiCodeConverter.TeamStateClRelKeyPrefix + team.State;
                else if(team.State.ToLower() == "mac-pv" && !rawList.ContainsKey(WikiCodeConverter.TeamStateMacPvKey)) // MAC-PV team
                    stateIndex = WikiCodeConverter.TeamStateMacPvKey;

                if(!rawList.ContainsKey(stateIndex))
                    rawList[stateIndex] = new Dictionary<FootballTeam, Color>();
                rawList[stateIndex][team] = color;
            }

            // convert for getting wiki code
            var table = WikiCodeConverter.GetUafaAlParticipantTable(rawList, startYear, finalYear);
            var tuple = Tuple.Create(rawList.First().Value.First().Key.FullName, table);

            return tuple;
        }

        /// <summary>
        /// Returns the group team list as array
        /// </summary>
        /// <returns>The list</returns>
        private string GetGroupTeamList()
        {
            SimpleLog.Info($"Get group list for AL season {Al.Season}.");

            var list = new List<string>(Al.AllTeamsRaw.Count);
            foreach(var g in Al.Groups)
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
            SimpleLog.Info($"Get group codes for AL season {Al.Season}.");

            var list = new List<string>(8);
            foreach (var g in Al.Groups)
                list.Add(WikiCodeConverter.ToWikiCode(g, WikiCodeConverter.ELeagueTemplate.AlGruppe));

            return list.ToArray();
        }

        /// <summary>
        /// Returns the group wiki codes as list
        /// </summary>
        /// <returns>The list</returns>
        private string[] GetDoubleMatchCodes(IEnumerable<DoubleMatch> doubleMatches)
        {
            SimpleLog.Info($"Get double matches codes for {Al.Season}.");

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
