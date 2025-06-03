using System.Collections.Generic;

namespace UltimateFootballSystem.Core.TacticsEngine.Utils
{
    /// <summary>
    /// Static utility class for managing position groups and positions
    /// </summary>
    public static class PositionGroupManager
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

        static PositionGroupManager()
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
    }
}
