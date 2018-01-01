using System;
using Simocracy.CLSim.Football.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Simocracy.CLSim.Test
{
    [TestClass]
    public class FootballTeamTest
    {
        [TestMethod]
        public void TestSetupTeamRemark()
        {
            var team1Str = "{{GRSI}} FC Sevi City; 40";
            var team2Remark ="Da Athletic Puerto Rio sowohl Titelverteidiger als " +
                             "auch Meister ist, rückten der Ligazweite und Ligadritte auf.";
            var team2Str = "{{MAC}} Trinidad CF; 40; " + team2Remark;
            var team2WikiRemarkStr = "{{MAC}} Trinidad CF<ref group=\"A\">" + team2Remark + "</ref>";

            var team1 = new FootballTeam(team1Str);
            var team2 = new FootballTeam(team2Str);

            Assert.IsTrue(String.IsNullOrWhiteSpace(team1.Remark));
            Assert.AreEqual(team2Remark, team2.Remark);
            Assert.AreEqual(team2WikiRemarkStr, team2.GetWikiCodeWithRemarks());
        }
    }
}
