using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateFootballSystem.Core.TacticsEngine.Utils
{
    /// <summary>
    /// MonoBehaviour singleton for managing tactical role definitions and creating runtime instances
    /// </summary>
    public class RoleManager : MonoBehaviour
    {
        private static RoleManager _instance;
        public static RoleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RoleManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("RoleManager");
                        _instance = go.AddComponent<RoleManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        [Header("Role Definitions")]
        [SerializeField] private List<TacticalRoleDefinition> roleDefinitions = new List<TacticalRoleDefinition>();

        private Dictionary<TacticalRoleOption, TacticalRoleDefinition> _roleDefinitionLookup;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                Debug.Log("RoleManager: Instance created and initializing");
                InitializeRoleDefinitions();
            }
            else if (_instance != this)
            {
                Debug.Log("RoleManager: Duplicate instance destroyed");
                Destroy(gameObject);
            }
        }

        private void InitializeRoleDefinitions()
        {
            _roleDefinitionLookup = new Dictionary<TacticalRoleOption, TacticalRoleDefinition>();

            Debug.Log($"RoleManager: Initializing with {roleDefinitions.Count} role definitions");

            foreach (var definition in roleDefinitions)
            {
                if (definition != null)
                {
                    _roleDefinitionLookup[definition.roleOption] = definition;
                    Debug.Log($"RoleManager: Added role {definition.roleOption} ({definition.roleName}) with {definition.availablePositions.Count} positions");
                }
                else
                {
                    Debug.LogWarning("RoleManager: Found null role definition in list");
                }
            }

            Debug.Log($"RoleManager: Initialization complete. {_roleDefinitionLookup.Count} roles loaded");
        }

        // Get a new runtime instance of a role by type
        public static TacticalRole GetRole(TacticalRoleOption type)
        {
            if (Instance._roleDefinitionLookup.TryGetValue(type, out var definition))
            {
                return new TacticalRole(definition);
            }

            Debug.LogWarning($"Role definition for {type} not found in RoleManager");
            return null;
        }

        // Get all available roles as runtime instances
        public static List<TacticalRole> GetAllRoles()
        {
            var roles = new List<TacticalRole>();
            foreach (var definition in Instance._roleDefinitionLookup.Values)
            {
                roles.Add(new TacticalRole(definition));
            }
            return roles;
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

            Debug.Log($"RoleManager: Getting roles for position {position}");

            if (Instance._roleDefinitionLookup == null)
            {
                Debug.LogWarning("RoleManager: _roleDefinitionLookup is null, initializing...");
                Instance.InitializeRoleDefinitions();
            }

            foreach (var definition in Instance._roleDefinitionLookup.Values)
            {
                bool roleAvailable = false;

                // Check direct position assignment (legacy)
                if (definition.availablePositions.Contains(position))
                {
                    roleAvailable = true;
                    Debug.Log($"RoleManager: Found role {definition.roleOption} for position {position} (direct match)");
                }

                // Check position groups (primary method)
                foreach (var positionGroup in definition.availablePositionGroups)
                {
                    var positionsInGroup = TacticalPositionUtils.GetPositionsForGroup(positionGroup);
                    if (positionsInGroup.Contains(position))
                    {
                        roleAvailable = true;
                        Debug.Log($"RoleManager: Found role {definition.roleOption} for position {position} (via group {positionGroup})");
                        break;
                    }
                }

                if (roleAvailable)
                {
                    result.Add(definition.roleOption);
                }
            }

            Debug.Log($"RoleManager: Found {result.Count} roles for position {position}: {string.Join(", ", result)}");
            return result;
        }

        // Check if a role is available for a position
        public static bool IsRoleAvailableForPosition(TacticalRoleOption roleType, TacticalPositionOption position)
        {
            if (Instance._roleDefinitionLookup.TryGetValue(roleType, out var definition))
            {
                // Check direct position assignment (legacy)
                if (definition.availablePositions.Contains(position))
                {
                    return true;
                }

                // Check position groups (primary method)
                foreach (var positionGroup in definition.availablePositionGroups)
                {
                    var positionsInGroup = TacticalPositionUtils.GetPositionsForGroup(positionGroup);
                    if (positionsInGroup.Contains(position))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Get available duties for a role type
        public static List<TacticalDutyOption> GetAvailableDutiesForRole(TacticalRoleOption roleType)
        {
            if (Instance._roleDefinitionLookup.TryGetValue(roleType, out var definition))
            {
                return new List<TacticalDutyOption>(definition.availableDuties);
            }
            return new List<TacticalDutyOption>();
        }

        // Get zone influence for a specific role at a position with a duty
        public static Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption> GetZonesForRole(
            TacticalRoleOption roleType,
            TacticalPositionOption position,
            TacticalDutyOption duty)
        {
            if (Instance._roleDefinitionLookup.TryGetValue(roleType, out var definition))
            {
                return definition.GetZonesForPositionAndDuty(position, duty);
            }
            return new Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption>();
        }

        // Editor helper methods
        public void AddRoleDefinition(TacticalRoleDefinition definition)
        {
            if (definition != null && !roleDefinitions.Contains(definition))
            {
                roleDefinitions.Add(definition);
                _roleDefinitionLookup[definition.roleOption] = definition;
            }
        }

        public void RemoveRoleDefinition(TacticalRoleDefinition definition)
        {
            if (definition != null)
            {
                roleDefinitions.Remove(definition);
                _roleDefinitionLookup?.Remove(definition.roleOption);
            }
        }

        // Get all role definitions (for editor/debugging)
        public List<TacticalRoleDefinition> GetAllRoleDefinitions()
        {
            return new List<TacticalRoleDefinition>(roleDefinitions);
        }
    }
}