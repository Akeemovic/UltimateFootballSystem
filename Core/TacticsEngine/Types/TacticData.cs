using System.Collections.Generic;

namespace UltimateFootballSystem.Core.TacticsEngine.Types
{
    /// <summary>
    /// Simple serializable data container for tactic information
    /// </summary>
    public class TacticData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PositionData> Positions { get; set; } = new List<PositionData>();
        public List<int> Substitutes { get; set; } // Players' Ids
        public List<int> Reserves { get; set; } // Players' Ids
    }

    /// <summary>
    /// Simple serializable data container for position information within a tactic
    /// </summary>
    public class PositionData
    {
        public TacticalPositionOption Position { get; set; }
        public int? PlayerId { get; set; }
        public TacticalRoleOption RoleType { get; set; }
        public TacticalDutyOption Duty { get; set; }
    }
}
