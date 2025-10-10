namespace UltimateFootballSystem.Core.Tactics.Instructions.Team.InTransition.GkDistributionInstructions
{
    public enum GkBehaviorOption
    {
        DistributeQuickly,
        SlowPaceDown
    }

    public enum GkDistributionAreaOption
    {
        ToCentreBacks,
        ToFullBacks,
        ToPlaymakers,
        ToFlanks,
        ToOppositionDefense,
        ToTargetMan
    }

    public enum GkDistributionTypeOption
    {
        RollItOut,
        ThrowItLong,
        TakeShortKick,
        TakeLongKicks
    }

    public enum GkDistributionZoneTypeOption
    {
        AreaPlayer,
        Position
    }
}
