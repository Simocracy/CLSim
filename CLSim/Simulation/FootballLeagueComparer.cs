using System.Collections.Generic;
using System.Data;

namespace Simocracy.CLSim.Simulation
{
    /// <summary>
    /// Base comparer for football league tables for rules 1 to 3
    /// </summary>
    internal class FootballLeagueBaseComparer : Comparer<DataRow>
    {

        public override int Compare(DataRow x, DataRow y)
        {
            // Rule 1: Points
            if(x.Field<int>(LeagueTable.PointsRow) != y.Field<int>(LeagueTable.PointsRow))
                return x.Field<int>(LeagueTable.PointsRow).CompareTo(y.Field<int>(LeagueTable.PointsRow));

            // Rule 2: GoalDiff
            if(x.Field<int>(LeagueTable.GoalDiffRow) != y.Field<int>(LeagueTable.GoalDiffRow))
                return x.Field<int>(LeagueTable.GoalDiffRow).CompareTo(y.Field<int>(LeagueTable.GoalDiffRow));

            // Rule 3: GoalsFor
            if(x.Field<int>(LeagueTable.GoalsForCountRow) != y.Field<int>(LeagueTable.GoalsForCountRow))
                return x.Field<int>(LeagueTable.GoalsForCountRow).CompareTo(y.Field<int>(LeagueTable.GoalsForCountRow));

            // Direct Matches
            return 0;
        }
    }

    /// <summary>
    /// Direct match Comparer for football league tables for rules 4 to 7
    /// </summary>
    internal class FootballLeagueDirectComparer : Comparer<DataRow>
    {

        public override int Compare(DataRow x, DataRow y)
        {
            // Rule 4: Direct Points
            if(x.Field<int>(LeagueTable.PointsRow) != y.Field<int>(LeagueTable.PointsRow))
                return x.Field<int>(LeagueTable.PointsRow).CompareTo(y.Field<int>(LeagueTable.PointsRow));

            // Rule 5: Direct GoalDIff
            if(x.Field<int>(LeagueTable.GoalDiffRow) != y.Field<int>(LeagueTable.GoalDiffRow))
                return x.Field<int>(LeagueTable.GoalDiffRow).CompareTo(y.Field<int>(LeagueTable.GoalDiffRow));

            // Rule 6: Direct Goals
            if(x.Field<int>(LeagueTable.GoalsForCountRow) != y.Field<int>(LeagueTable.GoalsForCountRow))
                return x.Field<int>(LeagueTable.GoalsForCountRow).CompareTo(y.Field<int>(LeagueTable.GoalsForCountRow));

            // Rule 7: Drawing
            return Globals.Random.Next(0, 2) == 0 ? -1 : 1;
        }
    }
}
