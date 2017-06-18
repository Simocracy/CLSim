using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simocracy.CLSim.Simulation;

namespace Simocracy.CLSim.Test
{
    [TestClass]
    public class TableSortingTest
    {
        private FootballLeague League;
        private FootballTeam A1;
        private FootballTeam A2;
        private FootballTeam A3;
        private FootballTeam A4;
        private FootballTeam A5;


        [TestInitialize]
        public void InitTest()
        {
            // Original UAFA CL 2053-54 Group G
            // Source: https://simocracy.de/UAFA_Champions_League_2053/54#Gruppe_G
            A1 = new FootballTeam("{{OB}} Royal George Dutchtown", 0);
            A2 = new FootballTeam("{{NUS}} Lokomotive Simskau", 0);
            A3 = new FootballTeam("{{EMA}} FC Juneborg", 0);
            A4 = new FootballTeam("{{RLQ}} FC Montrêve", 0);
            A5 = new FootballTeam("{{GRA}} TSG 1930 Neuenburg", 0);

            League = new FootballLeague("test", A1, A2, A3, A4, A5)
            {
                Matches = new ObservableCollection<FootballMatch>
                {
                    new FootballMatch(A1, A2) {ResultA = 2, ResultB = 1},
                    new FootballMatch(A3, A4) {ResultA = 2, ResultB = 0},
                    new FootballMatch(A2, A3) {ResultA = 2, ResultB = 1},
                    new FootballMatch(A4, A5) {ResultA = 1, ResultB = 3},
                    new FootballMatch(A5, A1) {ResultA = 2, ResultB = 1},
                    new FootballMatch(A4, A2) {ResultA = 1, ResultB = 1},
                    new FootballMatch(A3, A1) {ResultA = 2, ResultB = 0},
                    new FootballMatch(A2, A5) {ResultA = 1, ResultB = 0},
                    new FootballMatch(A1, A4) {ResultA = 3, ResultB = 0},
                    new FootballMatch(A5, A3) {ResultA = 1, ResultB = 1},
                    new FootballMatch(A2, A1) {ResultA = 2, ResultB = 0},
                    new FootballMatch(A4, A3) {ResultA = 0, ResultB = 1},
                    new FootballMatch(A3, A2) {ResultA = 1, ResultB = 2},
                    new FootballMatch(A5, A4) {ResultA = 3, ResultB = 1},
                    new FootballMatch(A1, A5) {ResultA = 1, ResultB = 2},
                    new FootballMatch(A2, A4) {ResultA = 2, ResultB = 0},
                    new FootballMatch(A1, A3) {ResultA = 3, ResultB = 3},
                    new FootballMatch(A5, A2) {ResultA = 1, ResultB = 1},
                    new FootballMatch(A4, A1) {ResultA = 1, ResultB = 0},
                    new FootballMatch(A3, A5) {ResultA = 2, ResultB = 1}
                }
            };
        }

        [TestMethod]
        public void TestTableSorting()
        {
            League.CalculateTable();

            Assert.AreEqual(A2, League.Table.Pos1[LeagueTable.TeamRow]);
            Assert.AreEqual(A3, League.Table.Pos2[LeagueTable.TeamRow]);
            Assert.AreEqual(A5, League.Table.Pos3[LeagueTable.TeamRow]);
            Assert.AreEqual(A1, League.Table.Pos4[LeagueTable.TeamRow]);
            Assert.AreEqual(A4, League.Table.Pos5[LeagueTable.TeamRow]);
        }
    }
}
