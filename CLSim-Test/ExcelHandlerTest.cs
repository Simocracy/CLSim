using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Simocracy.CLSim.Football.Base;
using Simocracy.CLSim.Football.UAFA;
using Simocracy.CLSim.IO;
using System.IO;

namespace Simocracy.CLSim.Test
{
    [TestClass]
    public class ExcelHandlerTest
    {
        private Coefficient Coeff1;
        private Coefficient Coeff2;
        private ExcelHandler Handler;

        [TestInitialize]
        public void Init()
        {
            Coeff1 = new Coefficient(new FootballTeam("{{FRC}} BAM", 12))
            {
                Won = 10,
                Drawn = 2
            };
            Coeff1.RoundsPlayed.Add(ETournamentRound.CLGroupStage);
            Coeff1.RoundsPlayed.Add(ETournamentRound.CLRoundOf16);
            Coeff1.RoundsPlayed.Add(ETournamentRound.CLQuarterFinals);

            Coeff2 = new Coefficient(new FootballTeam("{{UNAS}} Seattle", 12))
            {
                Won = 9,
                Drawn = 1
            };
            Coeff2.RoundsPlayed.Add(ETournamentRound.CLGroupStage);
            Coeff2.RoundsPlayed.Add(ETournamentRound.ALRoundOf32);
            Coeff2.RoundsPlayed.Add(ETournamentRound.ALRoundOf16);
            Coeff2.RoundsPlayed.Add(ETournamentRound.ALQuarterFinals);
            Coeff2.RoundsPlayed.Add(ETournamentRound.ALSemiFinals);
        }

        [TestCleanup]
        public void Close()
        {
            if(Handler.CanUse)
            {
                Handler.ExcelApp.DisplayAlerts = false;
                Handler.ExcelApp.Quit();
            }
        }

        [TestMethod]
        public void TestFileCreation()
        {
            Handler = new ExcelHandler();
            Handler.WriteHeaders("2053/54");
            Handler.WriteTeamCoefficientLine(Coeff2);
            Handler.WriteTeamCoefficientLine(Coeff1);
            var dir = $"{Environment.CurrentDirectory}\\testfile.xlsx";
            Handler.Close(dir);

            Assert.AreEqual(Coeff1.Points, 33);
            Assert.AreEqual(Coeff2.Points, 27);
            Assert.IsTrue(File.Exists(dir));
        }
    }
}
