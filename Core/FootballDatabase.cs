using UltimateFootballSystem.Core.Tactics;
using UnityEngine;

namespace UltimateFootballSystem.Core
{
    [CreateAssetMenu(
        fileName = "FootballDatabase",
        menuName = "Ultimate Football System/Football Database",
        order = 0)]
    public class FootballDatabase : ScriptableObject
    {
        [Header("Definitions")]
        [SerializeField] private TacticalRoleDefinition[] roleDefinitions;

        // Future: Formation templates, position definitions, etc.
        // [SerializeField] private FormationTemplate[] formationTemplates;

        [Header("Metadata")]
        [SerializeField] private string databaseName = "Default Database";
        [SerializeField] private string databaseVersion = "1.0.0";
        [SerializeField, TextArea(3, 6)] private string description = "Football database containing all game definitions.";

        #region Public Accessors

        public TacticalRoleDefinition[] RoleDefinitions => roleDefinitions;
        public string DatabaseName => databaseName;
        public string DatabaseVersion => databaseVersion;
        public string Description => description;

        // Statistics
        public int RoleDefinitionCount => roleDefinitions?.Length ?? 0;

        // Validation
        public bool IsValid => ValidateDatabase();

        #endregion

        #region Validation

        private bool ValidateDatabase()
        {
            if (roleDefinitions == null || roleDefinitions.Length == 0)
                return false;

            // Check for nulls
            foreach (var role in roleDefinitions)
            {
                if (role == null) return false;
            }

            return true;
        }

        #endregion
    }
}