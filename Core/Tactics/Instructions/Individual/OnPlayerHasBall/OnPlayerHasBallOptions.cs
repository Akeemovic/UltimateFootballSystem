namespace UltimateFootballSystem.Core.Tactics.Instructions.Individual
{
    public enum HoldUpBallOption
    {
        None = 0,
        Yes,
        No
    }

    public enum WingPlayOption
    {
        None = 0,
        CutInside,
        RunThroughFlank
    }

    public enum CrossingFrequencyOption
    {
        None = 0,
        Less,
        More
    }

    public enum CrossDistanceOption
    {
        None = 0,
        FromDeep,
        FromByline
    }

    public enum CrossTargetOption
    {
        None = 0,
        FarPost,
        NearPost,
        Centre,
        TargetMan
    }

    public enum ShootingFrequencyOption
    {
        None = 0,
        Less,
        More
    }

    public enum DribblingFrequencyOption
    {
        None = 0,
        Less,
        More
    }

    public enum PassingStyleOption
    {
        None = 0,
        Short,
        Direct
    }

    public enum CreativePassingOption
    {
        None = 0,
        Less,
        More
    }
}