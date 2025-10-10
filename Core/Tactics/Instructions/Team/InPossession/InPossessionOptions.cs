namespace UltimateFootballSystem.Core.Tactics.Instructions.Team.InPossession
{
    public enum AttackingWidthOption
    {
        VeryNarrow,
        FailyNarrow,
        FailyWide,
        Wide,
        VeryWide
    }

    public enum ChanceCreationOption
    {
        WorkIntoBox,
        ShootOnSight,
        HitEarlyCrosses
    }

    public enum CreativityFreedomOption
    {
        Expressive,
        Disciplined
    }

    public enum CrossingTypeOption
    {
        Low,
        Mixed,
        Floating
    }

    public enum PassingDirectnessOption
    {
        MuchShorter,
        Shoter,
        Standard,
        MoreDirect,
        VeryDirect
    }

    public enum PlayFocusOption
    {
        LeftFlank,
        RightFlank,
        OverlapLeft,
        OverlapRight,
        UnderlapLeft,
        UnderlapRight,
        ThroughTheMiddle,
        PlayOutOfDefense
    }

    public enum TempoOption
    {
        MuchLower,
        Lower,
        Standard,
        Higher,
        MuchHigher
    }

    public enum TimeWastingOption
    {
        Never,
        SomeTimes,
        Frequently
    }
}
