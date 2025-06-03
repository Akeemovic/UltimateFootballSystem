using System;
using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.TacticsEngine.TacticalRoles;

namespace UltimateFootballSystem.Core.TacticsEngine.Utils
{
    /// <summary>
    /// Factory class for creating and managing all available tactical roles
    /// </summary>
    public static class RoleManager
    {
        private static readonly Dictionary<TacticalRoleOption, TacticalRole> _rolePrototypes =
            new Dictionary<TacticalRoleOption, TacticalRole>();

        static RoleManager()
        {
            RegisterAllRoles();
        }

        private static void RegisterAllRoles()
        {
            // Register concrete role implementations when available
            try
            {
                // Use these when you have the actual classes implemented
                RegisterRole(new Goalkeeper());
                RegisterRole(new Winger());
                RegisterRole(new BoxToBoxMidfielder());
                RegisterRole(new FalseNine());
            }
            catch (Exception)
            {
                // Use these when you have the actual classes implemented
            }
        }

        private static void RegisterRole(TacticalRole role)
        {
            _rolePrototypes[role.RoleOption] = role;
        }

        // Get a clone of a role by type
        public static TacticalRole GetRole(TacticalRoleOption type)
        {
            return _rolePrototypes.TryGetValue(type, out var role) ? role.Clone() : null;
        }

        // Get all available roles
        public static List<TacticalRole> GetAllRoles()
        {
            return _rolePrototypes.Values.Select(r => r.Clone()).ToList();
        }

        // Get all roles as a collection of role objects for use in UI
        public static List<TacticalRole> GetAllAvailableRoles()
        {
            return GetAllRoles();
        }

        // Get a list of roles available for a specific position
        public static List<TacticalRoleOption> GetRolesForPosition(TacticalPositionOption position)
        {
            var result = new List<TacticalRoleOption>();

            foreach (var role in _rolePrototypes.Values)
            {
                if (role.AvailablePositions.Contains(position))
                {
                    result.Add(role.RoleOption);
                }
            }

            return result;
        }

        // Check if a role is available for a position
        public static bool IsRoleAvailableForPosition(TacticalRoleOption roleType, TacticalPositionOption position)
        {
            if (_rolePrototypes.TryGetValue(roleType, out var role))
            {
                return role.AvailablePositions.Contains(position);
            }
            return false;
        }

        // Get available duties for a role type
        public static List<TacticalDutyOption> GetAvailableDutiesForRole(TacticalRoleOption roleType)
        {
            if (_rolePrototypes.TryGetValue(roleType, out var role))
            {
                return new List<TacticalDutyOption>(role.AvailableDuties);
            }
            return new List<TacticalDutyOption>();
        }

        // Get zone influence for a specific role at a position with a duty
        public static Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption> GetZonesForRole(
            TacticalRoleOption roleType,
            TacticalPositionOption position,
            TacticalDutyOption duty)
        {
            if (_rolePrototypes.TryGetValue(roleType, out var role))
            {
                var roleClone = role.Clone();
                roleClone.SetSelectedPosition(position);
                roleClone.SetSelectedDuty(duty);
                return roleClone.ZonesOwned;
            }
            return new Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption>();
        }
    }
}
