using UltimateFootballSystem.Core.TacticsEngine.Instructions.Team.InTransition;

namespace UltimateFootballSystem.Core.TacticsEngine.Instructions.Team
{
    public class InTransitionInstructions
    {
        public OnGkInPossession OnGkInPossessionInstructions { get; protected set; }
        public OnPossessionLostOption OnPossessionLostInstruction { get; set; }
        public OnPossessionWonOption OnPossessionWonInstruction { get; set; }
        public InTransitionInstructions()
        {
            OnPossessionWonInstruction = OnPossessionWonOption.Counter;
            OnPossessionLostInstruction = OnPossessionLostOption.CounterPress;
            OnGkInPossessionInstructions = new OnGkInPossession();
        }
    }
}