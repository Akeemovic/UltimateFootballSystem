using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual;
using UltimateFootballSystem.Core.Tactics.Utils;

namespace UltimateFootballSystem.Core.Tactics
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

            // Filter roles that are compatible with this position and clone them
            foreach (var role in availableRoles)
            {
                if (role.AvailablePositions.Contains(Position))
                {
                    // Make a clone to avoid affecting the original
                    TacticalRole roleClone = role.Clone();
                    roleClone.SetSelectedPosition(Position);
                    AvailableRoles.Add(roleClone);
                    AvailableRoleOptions.Add(roleClone.RoleOption);
                }
            }

            // Set default role if available
            if (AvailableRoles.Any())
            {
                SelectedRole = AvailableRoles.First();
                SelectedRoleOption = SelectedRole.RoleOption;
            }
        }

        // Alternative constructor that uses RoleManager directly
        public TacticalPosition(
            TacticalPositionGroupOption positionGroup,
            TacticalPositionOption position)
        {
            Position = position;
            PositionGroup = positionGroup;
            PositionType = TacticalPositionUtils.GetTypeForPosition(Position);

            // Get available roles for this position from RoleManager
            var availableRoleOptions = RoleManager.GetRolesForPosition(position);
            AvailableRoleOptions = availableRoleOptions;

            UnityEngine.Debug.Log($"TacticalPosition: Position {position} received {availableRoleOptions.Count} role options from RoleManager");

            foreach (var roleOption in availableRoleOptions)
            {
                var role = RoleManager.GetRole(roleOption);
                if (role != null)
                {
                    role.SetSelectedPosition(Position);
                    AvailableRoles.Add(role);
                    UnityEngine.Debug.Log($"TacticalPosition: Successfully added role {role.RoleName} for position {position}");
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"TacticalPosition: RoleManager.GetRole returned null for {roleOption}");
                }
            }

            // Set default role if available
            if (AvailableRoles.Any())
            {
                SelectedRole = AvailableRoles.First();
                SelectedRoleOption = SelectedRole.RoleOption;
                UnityEngine.Debug.Log($"TacticalPosition: Set default role {SelectedRole.RoleName} for position {position}");
            }
            else
            {
                UnityEngine.Debug.LogError($"TacticalPosition: No roles available for position {position} - SelectedRole will be null");
            }
        }

        // NEW METHOD: Set selected role by role type - more convenient API
        public bool SetRoleOption(TacticalRoleOption roleType)
        {
            var matchingRole = AvailableRoles.FirstOrDefault(r => r.RoleOption == roleType);
            if (matchingRole != null)
            {
                SelectedRole = matchingRole;
                SelectedRoleOption = roleType;
                return true;
            }
            return false;
        }

        // Set selected duty for the current role
        public void SetDutyOption(TacticalDutyOption duty)
        {
            SelectedRole?.SetSelectedDuty(duty);
        }

        // Get current selected duty
        public TacticalDutyOption GetSelectedDutyOption()
        {
            return SelectedRole?.SelectedDuty ?? default;
        }

        // Get available duties for the currently selected role
        public List<TacticalDutyOption> GetAvailableDuties()
        {
            return SelectedRole?.AvailableDuties ?? new List<TacticalDutyOption>();
        }

        // Get default duty for the currently selected role
        public TacticalDutyOption GetDefaultDutyOption()
        {
            if (SelectedRole?.AvailableDuties.Count > 0)
            {
                // Return the first available duty as default
                return SelectedRole.AvailableDuties[0];
            }
            return default;
        }

        // Get individual instructions from the selected role
        public IndividualInstruction GetRoleInstructions()
        {
            return SelectedRole?.GetInstructions();
        }

        // Get zone ownership from the selected role
        public Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption> GetRoleZones()
        {
            return SelectedRole?.ZonesOwned ?? new Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption>();
        }

        // Check if a specific zone is available for this position's role
        public TacticalZoneAvailabilityOption GetZoneAvailability(TacticalZoneOption zone)
        {
            return SelectedRole?.GetZoneAvailability(zone) ?? TacticalZoneAvailabilityOption.None;
        }

        // Check if role supports custom instructions
        public bool HasCustomInstructions()
        {
            return SelectedRole?.HasCustomInstructions ?? false;
        }

        // Set custom instructions for the selected role (for future user modifications)
        public void SetCustomInstructions(IndividualInstruction instructions)
        {
            SelectedRole?.SetCustomInstructions(instructions);
        }

        // Clear custom instructions and revert to default
        public void ClearCustomInstructions()
        {
            SelectedRole?.ClearCustomInstructions();
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
