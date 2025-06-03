namespace UltimateFootballSystem.Core.TacticsEngine.Instructions.Individual.OnOppositionHasBall
{
    public class OnOppositionHasBall
    {
        public PressingFrequencyOption PressingFrequency { get; set; }
        public PressingStyleOption PressingStyle { get; set; }
        public bool TighterMarking { get; set; }
        public TacklingStyleOption TacklingStyle { get; set; }
        public object OpponentToMark { get; set; }
        public object PositionToMark { get; set; }
    }
}