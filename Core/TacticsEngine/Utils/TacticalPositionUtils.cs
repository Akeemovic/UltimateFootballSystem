using System.Collections.Generic;

namespace UltimateFootballSystem.Core.TacticsEngine.Utils
{
    /// <summary>
    /// Static utility class for managing position groups, position types and positions
    /// </summary>
    public static class TacticalPositionUtils
    {
        private static readonly Dictionary<TacticalPositionGroupOption, List<TacticalPositionOption>> GroupToPositions =
            new Dictionary<TacticalPositionGroupOption, List<TacticalPositionOption>>
            {
                { TacticalPositionGroupOption.GK, new List<TacticalPositionOption> { TacticalPositionOption.GK } },
                { TacticalPositionGroupOption.D_Center, new List<TacticalPositionOption> { TacticalPositionOption.DC, TacticalPositionOption.DCL, TacticalPositionOption.DCR } },
                { TacticalPositionGroupOption.D_Flank, new List<TacticalPositionOption> { TacticalPositionOption.DL, TacticalPositionOption.DR } },
                { TacticalPositionGroupOption.DM_Center, new List<TacticalPositionOption> { TacticalPositionOption.DMC, TacticalPositionOption.DMCL, TacticalPositionOption.DMCR } },
                { TacticalPositionGroupOption.DM_Flank, new List<TacticalPositionOption> { TacticalPositionOption.DML, TacticalPositionOption.DMR } },
                { TacticalPositionGroupOption.M_Center, new List<TacticalPositionOption> { TacticalPositionOption.MC, TacticalPositionOption.MCL, TacticalPositionOption.MCR } },
                { TacticalPositionGroupOption.M_Flank, new List<TacticalPositionOption> { TacticalPositionOption.ML, TacticalPositionOption.MR } },
                { TacticalPositionGroupOption.AM_Center, new List<TacticalPositionOption> { TacticalPositionOption.AMC, TacticalPositionOption.AMCL, TacticalPositionOption.AMCR } },
                { TacticalPositionGroupOption.AM_Flank, new List<TacticalPositionOption> { TacticalPositionOption.AML, TacticalPositionOption.AMR } },
                { TacticalPositionGroupOption.ST_Center, new List<TacticalPositionOption> { TacticalPositionOption.STC, TacticalPositionOption.STCL, TacticalPositionOption.STCR } }
            };

        private static readonly Dictionary<TacticalPositionOption, TacticalPositionGroupOption> PositionToGroup =
            new Dictionary<TacticalPositionOption, TacticalPositionGroupOption>();

        static TacticalPositionUtils()
        {
            // Build the reverse lookup
            foreach (var group in GroupToPositions)
            {
                foreach (var position in group.Value)
                {
                    PositionToGroup[position] = group.Key;
                }
            }
        }

        public static List<TacticalPositionOption> GetPositionsForGroup(TacticalPositionGroupOption group)
        {
            return GroupToPositions.TryGetValue(group, out var positions)
                ? positions
                : new List<TacticalPositionOption>();
        }

        public static List<TacticalPositionOption> GetPositionOptionsForGroupAll(params TacticalPositionGroupOption[] groups)
        {
            var result = new List<TacticalPositionOption>();
            foreach (var group in groups)
            {
                result.AddRange(GetPositionsForGroup(group));
            }
            return result;
        }

        public static TacticalPositionGroupOption GetGroupForPosition(TacticalPositionOption position)
        {
            return PositionToGroup.TryGetValue(position, out var group)
                ? group
                : default;
        }

        public static bool IsPositionInGroup(TacticalPositionOption position, TacticalPositionGroupOption group)
        {
            return GroupToPositions.TryGetValue(group, out var positions) && positions.Contains(position);
        }
        
        public static (TacticalPositionGroupOption group, TacticalPositionTypeOption type) GetGroupAndType(TacticalPositionOption pos)
        {
            switch (pos)
            {
                case TacticalPositionOption.GK:
                    return (TacticalPositionGroupOption.GK, TacticalPositionTypeOption.GK);
        
                // Defense
                case TacticalPositionOption.DL:
                    return (TacticalPositionGroupOption.D_Flank, TacticalPositionTypeOption.DL);
                case TacticalPositionOption.DCL:
                case TacticalPositionOption.DC:
                case TacticalPositionOption.DCR:
                    return (TacticalPositionGroupOption.D_Center, TacticalPositionTypeOption.DC);
                case TacticalPositionOption.DR:
                    return (TacticalPositionGroupOption.D_Flank, TacticalPositionTypeOption.DR);
        
                // Defensive Midfield
                case TacticalPositionOption.DML:
                    return (TacticalPositionGroupOption.DM_Flank, TacticalPositionTypeOption.WBL);
                case TacticalPositionOption.DMCL:
                case TacticalPositionOption.DMC:
                case TacticalPositionOption.DMCR:
                    return (TacticalPositionGroupOption.DM_Center, TacticalPositionTypeOption.DMC);
                case TacticalPositionOption.DMR:
                    return (TacticalPositionGroupOption.DM_Flank, TacticalPositionTypeOption.WBR);
        
                // Midfield
                case TacticalPositionOption.ML:
                    return (TacticalPositionGroupOption.M_Flank, TacticalPositionTypeOption.ML);
                case TacticalPositionOption.MCL:
                case TacticalPositionOption.MC:
                case TacticalPositionOption.MCR:
                    return (TacticalPositionGroupOption.M_Center, TacticalPositionTypeOption.MC);
                case TacticalPositionOption.MR:
                    return (TacticalPositionGroupOption.M_Flank, TacticalPositionTypeOption.MR);
        
                // Attacking Midfield
                case TacticalPositionOption.AML:
                    return (TacticalPositionGroupOption.AM_Flank, TacticalPositionTypeOption.AML);
                case TacticalPositionOption.AMCL:
                case TacticalPositionOption.AMC:
                case TacticalPositionOption.AMCR:
                    return (TacticalPositionGroupOption.AM_Center, TacticalPositionTypeOption.AMC);
                case TacticalPositionOption.AMR:
                    return (TacticalPositionGroupOption.AM_Flank, TacticalPositionTypeOption.AMR);
        
                // Striker
                case TacticalPositionOption.STCL:
                case TacticalPositionOption.STC:
                case TacticalPositionOption.STCR:
                    return (TacticalPositionGroupOption.ST_Center, TacticalPositionTypeOption.ST);
        
                default:
                    return (TacticalPositionGroupOption.None, 0);
            }
        }

        public static TacticalPositionTypeOption GetTypeForPosition(TacticalPositionOption pos)
        {
            switch (pos)
            {
                case TacticalPositionOption.GK:
                    return TacticalPositionTypeOption.GK;

                // Defense
                case TacticalPositionOption.DL:
                    return TacticalPositionTypeOption.DL;
                case TacticalPositionOption.DCL:
                case TacticalPositionOption.DC:
                case TacticalPositionOption.DCR:
                    return TacticalPositionTypeOption.DC;
                case TacticalPositionOption.DR:
                    return TacticalPositionTypeOption.DR;

                // Defensive Midfield
                case TacticalPositionOption.DML:
                    return TacticalPositionTypeOption.WBL;
                case TacticalPositionOption.DMCL:
                case TacticalPositionOption.DMC:
                case TacticalPositionOption.DMCR:
                    return TacticalPositionTypeOption.DMC;
                case TacticalPositionOption.DMR:
                    return TacticalPositionTypeOption.WBR;

                // Midfield
                case TacticalPositionOption.ML:
                    return TacticalPositionTypeOption.ML;
                case TacticalPositionOption.MCL:
                case TacticalPositionOption.MC:
                case TacticalPositionOption.MCR:
                    return TacticalPositionTypeOption.MC;
                case TacticalPositionOption.MR:
                    return TacticalPositionTypeOption.MR;

                // Attacking Midfield
                case TacticalPositionOption.AML:
                    return TacticalPositionTypeOption.AML;
                case TacticalPositionOption.AMCL:
                case TacticalPositionOption.AMC:
                case TacticalPositionOption.AMCR:
                    return TacticalPositionTypeOption.AMC;
                case TacticalPositionOption.AMR:
                    return TacticalPositionTypeOption.AMR;

                // Forwards
                case TacticalPositionOption.STCL:
                case TacticalPositionOption.STC:
                case TacticalPositionOption.STCR:
                    return TacticalPositionTypeOption.ST;

                default:
                    return 0; // Or TacticalPositionTypeOption.None if defined
            }
        }

    }
}