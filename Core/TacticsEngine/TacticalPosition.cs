using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.TacticsEngine.Utils;

namespace UltimateFootballSystem.Core.TacticsEngine
{
    /// <summary>
    /// Represents a tactical position on the field
    /// </summary>
    public class TacticalPosition
    {
        // Core properties
        public TacticalPositionOption Position { get; }
        public TacticalPositionGroupOption PositionGroup { get; }
        public TacticalPositionTypeOption PositionType { get; }

        // Role management
        public List<TacticalRole> AvailableRoles { get; private set; } = new List<TacticalRole>();
        public List<TacticalRoleOption> AvailableRoleOptions { get; private set; } = new List<TacticalRoleOption>();
        public TacticalRole SelectedRole { get; private set; }
        public TacticalRoleOption SelectedRoleOption { get; private set; }

        // Player assignment
        public int? AssignedPlayerId { get; set; }
        public Player AssignedPlayer { get; set; }

        // public TacticalPosition(
        //     TacticalPositionGroupOption positionGroup,
        //     TacticalPositionOption position,
        //     IEnumerable<TacticalRole> availableRoles)
        // {
        // }

        public TacticalPosition(
            TacticalPositionGroupOption positionGroup,
            TacticalPositionOption position,
            List<TacticalRole> availableRoles)
        {
            Position = position;
            PositionGroup = positionGroup;
            PositionType = TacticalPositionUtils.GetTypeForPosition(Position);

            // Filter roles that are compatible with this position
            foreach (var role in availableRoles)
            {
                if (role.AvailablePositions.Contains(Position))
                {
                    // Make a clone to avoid affecting the original
                    TacticalRole roleClone = role.Clone();
                    roleClone.SetSelectedPosition(Position);
                    AvailableRoles.Add(roleClone);
                }
            }

            // Set default role if available
            if (AvailableRoles.Any())
            {
                SelectedRole = AvailableRoles.First();
            }
        }

        // NEW METHOD: Set selected role by role type - more convenient API
        public bool SetRole(TacticalRoleOption roleType)
        {
            var matchingRole = AvailableRoles.FirstOrDefault(r => r.RoleOption == roleType);
            if (matchingRole != null)
            {
                SelectedRole = matchingRole;
                return true;
            }
            return false;
        }

        // Original method kept for backward compatibility
        public void SetSelectedRole(TacticalRole role)
        {
            if (AvailableRoles.Any(r => r.RoleOption == role.RoleOption))
            {
                var matchingRole = AvailableRoles.First(r => r.RoleOption == role.RoleOption);
                SelectedRole = matchingRole;
            }
        }

        // Set selected duty for the current role
        public void SetSelectedDuty(TacticalDutyOption duty)
        {
            SelectedRole?.SetSelectedDuty(duty);
        }

        // Assign a player to this position
        public void AssignPlayer(Player player)
        {
            AssignedPlayer = player;
            AssignedPlayerId = player?.Id;
        }

        // Remove player from this position
        public void ClearPlayer()
        {
            AssignedPlayer = null;
            AssignedPlayerId = null;
        }

        // Get the player rating for this position and role
        public int GetPlayerRating()
        {
            if (AssignedPlayer == null)
                return 0;

            // Base rating from position
            // int positionRating = AssignedPlayer.GetPositionRating(Position);

            // Additional rating from role suitability
            int roleRating = AssignedPlayer.GetRoleRating(SelectedRole.RoleOption);

            // Combine ratings with some formula (could be more sophisticated)
            // return (positionRating * 3 + roleRating * 2) / 5;
            return 0;
        }
    }
}
