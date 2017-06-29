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
        [TestMethod]
        public void TestFileCreation()
        {
            var coeff1 = new Coefficient(new FootballTeam("{{FRC}} BAM", 12))
            {
                Won = 10,
                Drawn = 2
            };
            coeff1.RoundsPlayed.Add(ETournamentRound.CLGroupStage);
            coeff1.RoundsPlayed.Add(ETournamentRound.CLRoundOf16);
            coeff1.RoundsPlayed.Add(ETournamentRound.CLQuarterFinals);

            var coeff2 = new Coefficient(new FootballTeam("{{UNAS}} Seattle", 12))
            {
                Won = 9,
                Drawn = 1
            };
            coeff2.RoundsPlayed.Add(ETournamentRound.CLGroupStage);
            coeff2.RoundsPlayed.Add(ETournamentRound.CLRoundOf16);
            coeff2.RoundsPlayed.Add(ETournamentRound.CLQuarterFinals);
            coeff2.RoundsPlayed.Add(ETournamentRound.CLSemiFinals);

            var excelHandler = new ExcelHandler();
            excelHandler.WriteHeaders("2053/54");
            excelHandler.WriteTeamCoefficientLine(coeff1);
            excelHandler.WriteTeamCoefficientLine(coeff2);
            excelHandler.CloseFile("testfile.xlsx");

            Assert.IsTrue(File.Exists("testfile.xlsx"));
        }
    }
}
