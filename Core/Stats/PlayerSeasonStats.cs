using System.Collections.Generic;

namespace UltimateFootballSystem.Core.Stats
{
    public class PlayerSeasonStats
    {
        public int SeasonId;
    
        private Dictionary<int, PlayerCompetitionStats> PlayerCompetitionStats;
    }
}