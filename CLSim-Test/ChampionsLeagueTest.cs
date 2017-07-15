using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simocracy.CLSim.Football;
using Simocracy.CLSim.Football.Base;
using Simocracy.CLSim.Football.UAFA;

namespace Simocracy.CLSim.Test
{
    [TestClass]
    public class ChampionsLeagueTest
    {
        private ChampionsLeague Cl;

        [TestInitialize]
        public void InitTests()
        {
            Cl = null;

            Cl = new ChampionsLeague("2053/54");
            var groupA = new FootballLeague("A", FootballLeague.EMatchMode.Default,
                new FootballTeam("{{FRC}} BAM", 12),
                new FootballTeam("{{UNS}} SRFC", 12),
                new FootballTeam("{{GRA}} SV", 12));
            var groupB = new FootballLeague("B", FootballLeague.EMatchMode.Default,
                new FootballTeam("{{UNS}} Seattle", 12),
                new FootballTeam("{{MAC}} Tesoro", 12),
                new FootballTeam("{{GRA}} GS", 12));
            Cl.Groups = new ObservableCollection<FootballLeague> { groupA, groupB };
        }

        [TestMethod]
        public void TestGroupValidation()
        {
            Cl.ValidateGroups();

            Assert.AreEqual("BAM", Cl.Groups[0].Teams[0].Name);
            Assert.AreEqual("Seattle", Cl.Groups[1].Teams[0].Name);
        }

        /// <summary>
        /// Tests the Round of 16 validation. For only 2 Matches test is not reliable.
        /// </summary>
        //[TestMethod]
        public void TestRoundOf16Validation()
        {
            foreach(var g in Cl.Groups)
                g.CalculateTable();

            Cl.DrawRoundOf16();

            for(int i = 0; i < Cl.Groups.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Match {i}: {Cl.RoundOf16[i]}");
                Assert.IsTrue((Cl.RoundOf16[i].TeamA.Name == "BAM" && Cl.RoundOf16[i].TeamB.Name != "SRFC" &&
                               Cl.RoundOf16[i].TeamB.Name != "SV")
                              || (Cl.RoundOf16[i].TeamA.Name == "SRFC" && Cl.RoundOf16[i].TeamB.Name != "Seattle" &&
                                  Cl.RoundOf16[i].TeamB.Name != "BAM" && Cl.RoundOf16[i].TeamB.Name != "SV")
                              || (Cl.RoundOf16[i].TeamA.Name == "SV" && Cl.RoundOf16[i].TeamB.Name != "SRFC" &&
                                  Cl.RoundOf16[i].TeamB.Name != "BAM")
                              || (Cl.RoundOf16[i].TeamA.Name == "Seattle" && Cl.RoundOf16[i].TeamB.Name != "SRFC" &&
                                  Cl.RoundOf16[i].TeamB.Name != "Tesoro" && Cl.RoundOf16[i].TeamB.Name != "GS")
                              || (Cl.RoundOf16[i].TeamA.Name == "Tesoro" && Cl.RoundOf16[i].TeamB.Name != "Seattle" &&
                                  Cl.RoundOf16[i].TeamB.Name != "GS")
                              || (Cl.RoundOf16[i].TeamA.Name == "GS" && Cl.RoundOf16[i].TeamB.Name != "Seattle" &&
                                  Cl.RoundOf16[i].TeamB.Name != "Tesoro"));
            }
        }
    }
}
