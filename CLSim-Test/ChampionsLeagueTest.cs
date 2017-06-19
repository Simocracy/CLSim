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
		private ChampionsLeague _Cl;

		[TestInitialize]
		public void InitTests()
		{
			_Cl = null;

			_Cl = new ChampionsLeague();
			var groupA = new FootballLeague("A",
				new FootballTeam("{{UNS}} Seattle", 12),
				new FootballTeam("{{UNS}} SRFC", 12),
				new FootballTeam("{{GRA}} SV", 12));
			var groupB = new FootballLeague("B",
				new FootballTeam("{{FRC}} BAM", 12),
				new FootballTeam("{{MAC}} Tesoro", 12),
				new FootballTeam("{{GRA}} GS", 12));

			_Cl.Groups = new ObservableCollection<FootballLeague> {groupA, groupB};
		}

		[TestMethod]
		public void TestGroupValidation()
		{
			_Cl.ValidateGroups();
			Assert.AreEqual("BAM", _Cl.Groups[0].Teams[0].Name);
			Assert.AreEqual("Seattle", _Cl.Groups[1].Teams[0].Name);
		}
	}
}
