namespace UltimateFootballSystem.Core.Stats
{
    public class PlayerCompetitionStats
    {
        int CompetitionId;
        string CompetitionName;

        public PlayerCompetitionStats(int competitionId, string competitionName)
        {
            CompetitionId = competitionId;
            CompetitionName = competitionName;
        }
    
        /******************/
        /*** General ***/
        /******************/
        public int Appearances;
        public int AppearancesAsSubstitute;

        public int Goals;
        public int Assists;
        public int Keypasess;
    
    
        /// <summary>
        /// Man of the match awards.
        /// </summary>
        public int MotmAwards;

        public float MinutesPlayed;
        public float AveragePerformanceRating;
        public int YellowCards;
        public int RedCards;

        /******************/
        /*** Attacking ***/
        /******************/
        public int FoulsWon;
        public int ShotsOnTarget;
        public int PassesMade;
        public int PassesReceived;
        public int Crosses;
        public int DistanceDribbled;
        public int Sprints;

        /******************/
        /*** Defending ***/
        /******************/
        public int FoulsConceded;
        public int TacklesMade;
        public int ChallengesWon;
        public int ChallengesLost;
        public int Interceptions;
        public int Smothers;
        public int Clearances;
        public int CleanSheets;
    }
}
