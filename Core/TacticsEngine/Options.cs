namespace UltimateFootballSystem.Core.TacticsEngine
{
    public enum TeamStateOption
    {
        InPosession,
        InTransitionPosessionWon,
        InTransitionPosessionLost,
        InTransitionGkPosession,
        OutOfPosessesion
    }

    public enum TacticalPositionGenericOption
    {
        GK = 1,
        DL, DC, DR,
        WBL, DMC, WBR,
        ML, MC, MR,
        AML, AMC, AMR,
        F
    }

    public enum TacticalPositionGroupOption
    {
        None = 0,
        GK = 1,
        D_Center, D_Flank,
        DM_Center, DM_Flank,
        M_Center, M_Flank,
        AM_Center, AM_Flank,
        ST_Center
    }

    public enum TacticalPositionOption
    {
        None = 0,
        GK = 1,
        DL, DCL, DC, DCR, DR,
        DML, DMCL, DMC, DMCR, DMR,
        ML, MCL, MC, MCR, MR,
        AML, AMCL, AMC, AMCR, AMR,
        STCL, STC, STCR
    }

    public enum TacticalZoneOption
    {
        Z_1 = 1, Z_2, Z_3, Z_4, Z_5, Z_6, Z_7, Z_8, Z_9, Z_10,
        Z_11, Z_12, Z_13, Z_14, Z_15, Z_16, Z_17, Z_18, Z_19, Z_20,
        Z_21, Z_22, Z_23, Z_24, Z_25, Z_26, Z_27, Z_28, Z_29, Z_30,
        Z_31, Z_32, Z_33, Z_34, Z_35
    }

    public enum TacticalZoneAvailabilityOption
    {
        None = 0,
        Low,
        Medium,
        High,
    }
    
    public enum TacticalDutyOption
    {
        Automatic = 1,
        Attack,
        Support,
        Defend,
        Cover,
        Stopper
    }

    public enum TacticalRoleOption
    {
        // Goalkeeper roles
        Goalkeeper = 1,
        SweeperKeeper,

        // Defender roles
        Defender,
        BallPlayingDefender,
        NoNonsenseDefender,
        Libero,

        // Fullback roles
        FullBack,
        WingBack,
        InvertedWingBack,

        // Defensive midfielder roles
        DefensiveMidfielder,
        HalfBack,
        Anchor,
        BallWinningMidfielder,
        DeepLyingPlaymaker,
        Regista,

        // Central midfielder roles
        CentralMidfielder,
        BoxToBoxMidfielder,
        Mezzala,
        AdvancedPlaymaker,

        // Wide roles
        Winger,
        InsideForward,
        InvertedWinger,
        WideMidfielder,
        DefensiveWinger,

        // Attacking midfielder roles
        AttackingMidfielder,
        ShadowStriker,
        Trequartista,
        Enganche,

        // Striker roles
        Striker,
        AdvancedForward,
        Poacher,
        TargetMan,
        DeepLyingForward,
        FalseNine,
        PressingForward,
        CompleteForward
    }
}
