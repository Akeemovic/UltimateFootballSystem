namespace UltimateFootballSystem.Core.TacticsEngine.Instructions.Team
{
    public class TeamInstructions
    {
        public InPossessionInstructions InPossession;
        public InTransitionInstructions InTransition;
        public OutOfPossessionInstructions OutOfPossession;

        public TeamInstructions()
        {
            InPossession = new InPossessionInstructions();
            InTransition = new InTransitionInstructions();
            OutOfPossession = new OutOfPossessionInstructions();
        }
    }
}