using UltimateFootballSystem.Core.TacticsEngine.Instructions.Team.OutOfPossession;

namespace UltimateFootballSystem.Core.TacticsEngine.Instructions.Team
{
    public class OutOfPossessionInstructions
    {
        public OutOfPossessionInstructions()
        {
            DefensiveShape = new DefensiveShape();
            TighterMarking = false;
            Tackling = null;
            MarkingType = MarinkngTypeOption.Zonal;
            PressingIntensity = PressingIntensityOption.Balanced;
            PressingArea = PressingAreaOption.OwnHalf;
        }

        public DefensiveShape DefensiveShape { get; set; }
        public bool TighterMarking { get; set; }
        public TacklingOption? Tackling { get; set; }
        public MarinkngTypeOption MarkingType { get; set; }
        public PressingIntensityOption PressingIntensity { get; set; }
        public PressingAreaOption PressingArea { get; set; }
    }
}