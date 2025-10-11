using System.IO;
using System.Linq;
using UltimateFootballSystem.Core.Tactics;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual;
using UnityEditor;
using UnityEngine;

namespace UltimateFootballSystem.Editor.DatabaseManager.Utilities
{
    public static class RoleEditorUtility
    {
        private const string DEFAULT_ROLE_PATH = "Assets/UltimateFootballSystem/Core/Tactics/Roles";

        /// <summary>
        /// Generate a unique file name for a new role
        /// </summary>
        public static string GenerateUniqueFileName(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                roleName = "NewRole";

            // Clean the role name (remove invalid characters)
            var cleanName = CleanFileName(roleName);

            // Ensure the directory exists
            if (!Directory.Exists(DEFAULT_ROLE_PATH))
            {
                Directory.CreateDirectory(DEFAULT_ROLE_PATH);
            }

            // Generate unique path
            var basePath = Path.Combine(DEFAULT_ROLE_PATH, cleanName);
            var fullPath = basePath + ".asset";
            var counter = 1;

            while (File.Exists(fullPath))
            {
                fullPath = $"{basePath}_{counter}.asset";
                counter++;
            }

            return fullPath;
        }

        /// <summary>
        /// Clean a file name by removing invalid characters
        /// </summary>
        public static string CleanFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var cleaned = new string(fileName.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
            return cleaned.Trim();
        }

        /// <summary>
        /// Validate if a role name is unique (excluding the current role being edited)
        /// </summary>
        public static bool IsRoleNameUnique(string roleName, TacticalRoleDefinition currentRole = null)
        {
            var allRoles = AssetDatabase.FindAssets("t:TacticalRoleDefinition")
                .Select(guid => AssetDatabase.LoadAssetAtPath<TacticalRoleDefinition>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(role => role != null && role != currentRole);

            return !allRoles.Any(role => role.roleName == roleName);
        }

        /// <summary>
        /// Validate role option is unique (excluding the current role being edited)
        /// </summary>
        public static bool IsRoleOptionUnique(TacticalRoleOption roleOption, TacticalRoleDefinition currentRole = null)
        {
            var allRoles = AssetDatabase.FindAssets("t:TacticalRoleDefinition")
                .Select(guid => AssetDatabase.LoadAssetAtPath<TacticalRoleDefinition>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(role => role != null && role != currentRole);

            return !allRoles.Any(role => role.roleOption == roleOption);
        }

        /// <summary>
        /// Validate the entire role definition
        /// </summary>
        public static (bool isValid, string errorMessage) ValidateRole(TacticalRoleDefinition role, bool isNewRole)
        {
            // Check role name
            if (string.IsNullOrWhiteSpace(role.roleName))
                return (false, "Role name cannot be empty.");

            if (!IsRoleNameUnique(role.roleName, isNewRole ? null : role))
                return (false, $"A role with the name '{role.roleName}' already exists.");

            // Check role option
            if (!IsRoleOptionUnique(role.roleOption, isNewRole ? null : role))
                return (false, $"A role with the option '{role.roleOption}' already exists.");

            // Check available positions
            if (role.availablePositions == null || role.availablePositions.Count == 0)
                return (false, "Role must have at least one available position.");

            // Check available duties
            if (role.availableDuties == null || role.availableDuties.Count == 0)
                return (false, "Role must have at least one available duty.");

            return (true, string.Empty);
        }

        /// <summary>
        /// Create a new role asset
        /// </summary>
        public static TacticalRoleDefinition CreateRole(string roleName, string customPath = null)
        {
            var role = ScriptableObject.CreateInstance<TacticalRoleDefinition>();
            role.roleName = roleName;

            var path = string.IsNullOrEmpty(customPath) ? GenerateUniqueFileName(roleName) : customPath;

            // Ensure directory exists
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            AssetDatabase.CreateAsset(role, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return role;
        }

        /// <summary>
        /// Save changes to an existing role
        /// </summary>
        public static void SaveRole(TacticalRoleDefinition role)
        {
            if (role == null) return;

            EditorUtility.SetDirty(role);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Get a friendly display name for an instruction availability state
        /// </summary>
        public static string GetAvailabilityDisplayName(InstructionAvailability availability)
        {
            return availability switch
            {
                InstructionAvailability.Unavailable => "Unavailable (Hidden)",
                InstructionAvailability.Available => "Available (Optional)",
                InstructionAvailability.Required => "Required (Locked)",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Get color for instruction availability state
        /// </summary>
        public static Color GetAvailabilityColor(InstructionAvailability availability)
        {
            return availability switch
            {
                InstructionAvailability.Unavailable => new Color(0.5f, 0.5f, 0.5f),
                InstructionAvailability.Available => new Color(0.4f, 0.7f, 1f),
                InstructionAvailability.Required => new Color(1f, 0.6f, 0.2f),
                _ => Color.white
            };
        }
    }
}
