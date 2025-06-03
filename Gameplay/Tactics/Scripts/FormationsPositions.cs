using UltimateFootballSystem.Core.TacticsEngine;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts
{
    public class FormationsPositions
    {
        public static TacticalPositionOption[] F442 = new TacticalPositionOption[11]
        {
            TacticalPositionOption.GK,
            TacticalPositionOption.DL, TacticalPositionOption.DCL, TacticalPositionOption.DCR, TacticalPositionOption.DR,
            TacticalPositionOption.ML, TacticalPositionOption.MCL, TacticalPositionOption.MCR, TacticalPositionOption.MR,
            TacticalPositionOption.STCL, TacticalPositionOption.STCR
        };

        public static TacticalPositionOption[]  F4141 = new TacticalPositionOption[11]
        {
            TacticalPositionOption.GK,
            TacticalPositionOption.DL, TacticalPositionOption.DCL, TacticalPositionOption.DCR, TacticalPositionOption.DR,
            TacticalPositionOption.DMC,
            TacticalPositionOption.ML, TacticalPositionOption.MCL, TacticalPositionOption.MCR, TacticalPositionOption.MR,
            TacticalPositionOption.STC
        };

        public static TacticalPositionOption[]  F433_DM_Wide = new TacticalPositionOption[11]
        {
            TacticalPositionOption.GK,
            TacticalPositionOption.DL, TacticalPositionOption.DCL, TacticalPositionOption.DCR, TacticalPositionOption.DR,
            TacticalPositionOption.DMC, 
            TacticalPositionOption.MCL, TacticalPositionOption.MCR,
            TacticalPositionOption.AML, TacticalPositionOption.AMR, 
            TacticalPositionOption.STC
        };

        public static TacticalPositionOption[]  F4231_Wide = new TacticalPositionOption[11]
        {
            TacticalPositionOption.GK,
            TacticalPositionOption.DL, TacticalPositionOption.DCL, TacticalPositionOption.DCR, TacticalPositionOption.DR,
            TacticalPositionOption.MCL, TacticalPositionOption.MCR,
            TacticalPositionOption.AML, TacticalPositionOption.AMC, TacticalPositionOption.AMR, 
            TacticalPositionOption.STC
        };

        public static TacticalPositionOption[]  F3232_352 = new TacticalPositionOption[11]
        {
            TacticalPositionOption.GK,
            TacticalPositionOption.DCL, TacticalPositionOption.DC, TacticalPositionOption.DCR,
            TacticalPositionOption.DML, TacticalPositionOption.DMR, 
            TacticalPositionOption.MCL, TacticalPositionOption.MC, TacticalPositionOption.MCR,
            TacticalPositionOption.STCL, TacticalPositionOption.STCR
        };
    
        public static TacticalPositionOption[]  F343 = new TacticalPositionOption[11]
        {
            TacticalPositionOption.GK,
            TacticalPositionOption.DCL, TacticalPositionOption.DC, TacticalPositionOption.DCR,
            TacticalPositionOption.ML, TacticalPositionOption.MCL, TacticalPositionOption.MCR, TacticalPositionOption.MR, 
            TacticalPositionOption.STCL, TacticalPositionOption.STC, TacticalPositionOption.STCR
        };
    }
}    