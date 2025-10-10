using System.Collections.Generic;
using UltimateFootballSystem.Core.Tactics.Instructions.Team.InPossession;

namespace UltimateFootballSystem.Core.Tactics.Instructions.Team
{
    public class InPossessionInstructions
    {
        public AttackingWidthOption AttackingWidth;
        public ChanceCreation ChanceCreation;
        public CreativityFreedomOption? CreativityFreedom;
        public CrossingTypeOption CrossingType;

        public ActionFrequencyOption? Dribbling;
        public PassingDirectnessOption PassingDirectness;
        public bool PassIntoSpace;
        public PlayFocus? PlayFocus;

        public bool PlayForSetPieces;
        public TempoOption Tempo;
        public TimeWastingOption TimeWasting;

        public InPossessionInstructions()
        {
            AttackingWidth = AttackingWidthOption.FailyWide;
            PassIntoSpace = false;
            PlayFocus = new PlayFocus();
            PassingDirectness = PassingDirectnessOption.Standard;
            Tempo = TempoOption.Standard;
            TimeWasting = TimeWastingOption.Never;
            CrossingType = CrossingTypeOption.Mixed;
            ChanceCreation = new ChanceCreation();
            PlayForSetPieces = false;
            Dribbling = null;
            CreativityFreedom = null;
        }

        public IReadOnlyCollection<PlayFocusOption> PlayFocusChoices => PlayFocus.Choices;
        public IReadOnlyCollection<ChanceCreationOption> ChanceCreationChoices => ChanceCreation.Choices;
    }
}