using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simocracy.CLSim.Simulation;

namespace Simocracy.CLSim.Extensions
{
    /// <summary>
    /// Custom comparer for Football League Tables
    /// </summary>
    class FootballLeagueBaseComparer : Comparer<DataRow>
    {

        public override int Compare(DataRow x, DataRow y)
        {
            // Rule 1: Points
            if(x.Field<int>(FootballLeague.PointsRow) != y.Field<int>(FootballLeague.PointsRow))
                return x.Field<int>(FootballLeague.PointsRow).CompareTo(y.Field<int>(FootballLeague.PointsRow));

            // Rule 2: GoalDiff
            if(x.Field<int>(FootballLeague.GoalDiffRow) != y.Field<int>(FootballLeague.GoalDiffRow))
                return x.Field<int>(FootballLeague.GoalDiffRow).CompareTo(y.Field<int>(FootballLeague.GoalDiffRow));

            // Rule 3: GoalsFor
            if(x.Field<int>(FootballLeague.GoalsForCountRow) != y.Field<int>(FootballLeague.GoalsForCountRow))
                return x.Field<int>(FootballLeague.GoalsForCountRow).CompareTo(y.Field<int>(FootballLeague.GoalsForCountRow));

            // Direct Matches
            x[FootballLeague.DirectMatchPos] = 0;
            y[FootballLeague.DirectMatchPos] = 0;
            return 0;

            // Rule 4: Direct Points

            // Rule 5: Direct GoalDIff

            // Rule 6: Direct Goals

            // Rule 7: Drawing
            //return Globals.Random.Next(0, 2) == 0 ? -1 : 1;
        }
    }
}
