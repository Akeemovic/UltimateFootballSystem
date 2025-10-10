using UltimateFootballSystem.Core.Tactics.Instructions.Team.InTransition.GkDistributionInstructions;

namespace UltimateFootballSystem.Core.Tactics.Instructions.Team.InTransition
{
    public class OnGkInPossession
    {
        public OnGkInPossession()
        {
            Behavior = GkBehaviorOption.DistributeQuickly;
            GkDistributionInstructions = new GkDistribution();
        }

        public GkDistribution GkDistributionInstructions { get; private set; }
        public GkBehaviorOption Behavior { get; set; }
    }
}