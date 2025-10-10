namespace UltimateFootballSystem.Core.Tactics.Instructions.Team.OutOfPossession
{
    public class DefensiveShape
    {
        public DefensiveShape()
        {
            OffSideTrap = false;
            DefensiveLine = DefensiveLineOption.Standard;
            LineOfEngagement = LineOfEngagementOption.Standard;
        }

        public bool OffSideTrap { get; set; }
        public DefensiveLineOption DefensiveLine { get; set; }
        public LineOfEngagementOption LineOfEngagement { get; set; }
    }
}