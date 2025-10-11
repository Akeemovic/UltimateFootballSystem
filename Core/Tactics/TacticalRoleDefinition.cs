using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Sirenix.OdinInspector;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual;
using UnityEngine;

namespace UltimateFootballSystem.Core.Tactics
{
    [CreateAssetMenu(fileName = "New Tactical Role", menuName = "Ultimate Football System/Tactical Role Definition")]
    public class TacticalRoleDefinition : ScriptableObject
    {
        [BoxGroup("Basic Information")]
        [LabelText("Role Option")]
        public TacticalRoleOption roleOption;

        [BoxGroup("Basic Information")]
        [LabelText("Role Name")]
        public string roleName;

        [BoxGroup("Basic Information")]
        [LabelText("Abbreviation")]
        [MaxLength(3)]
        public string abbreviation;

        [BoxGroup("Basic Information")]
        [LabelText("Description")]
        [TextArea(3, 6)]
        public string description;

        [BoxGroup("Position Groups")]
        [LabelText("Available Position Groups")]
        public List<TacticalPositionGroupOption> availablePositionGroups = new List<TacticalPositionGroupOption>();

        [BoxGroup("Position Groups")]
        [HideInInspector]
        public List<TacticalPositionOption> availablePositions = new List<TacticalPositionOption>();

        [BoxGroup("Duty Options")]
        [LabelText("Available Duties")]
        public List<TacticalDutyOption> availableDuties = new List<TacticalDutyOption>();

        [BoxGroup("Duty Options")]
        [LabelText("Default Duty")]
        [ValueDropdown("availableDuties")]
        public TacticalDutyOption defaultDuty;

        [BoxGroup("Zone Definitions")]
        [HideLabel]
        public List<ZoneDefinition> zoneDefinitions = new List<ZoneDefinition>();

        [HideInInspector]
        [SerializeField] private SerializableIndividualInstruction instructionConfiguration = new SerializableIndividualInstruction();

        [System.Serializable]
        public class ZoneDefinition
        {
            public TacticalPositionOption position;
            public TacticalDutyOption duty;
            public List<ZoneInfluence> zones = new List<ZoneInfluence>();
        }

        [System.Serializable]
        public class ZoneInfluence
        {
            public TacticalZoneOption zone;
            public TacticalZoneAvailabilityOption availability;
        }


        public Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption> GetZonesForPositionAndDuty(
            TacticalPositionOption position, TacticalDutyOption duty)
        {
            var result = new Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption>();

            var zoneDef = zoneDefinitions.Find(z => z.position == position && z.duty == duty);
            if (zoneDef != null)
            {
                foreach (var zone in zoneDef.zones)
                {
                    result[zone.zone] = zone.availability;
                }
            }

            return result;
        }
 
        public IndividualInstruction CreateIndividualInstruction(TacticalPositionGroupOption positionGroup)
        {
            // Convert serializable configuration to runtime instruction
            return instructionConfiguration.ToRuntimeInstruction(positionGroup);
        }

        // Get the serializable instruction configuration for editor
        public SerializableIndividualInstruction GetInstructionConfiguration()
        {
            return instructionConfiguration;
        }

        // Set the instruction configuration (used by editor)
        public void SetInstructionConfiguration(SerializableIndividualInstruction config)
        {
            instructionConfiguration = config;
        }

        // Get all available positions including those from position groups
        public List<TacticalPositionOption> GetAllAvailablePositions()
        {
            var allPositions = new List<TacticalPositionOption>(availablePositions);

            // Add positions from position groups
            foreach (var positionGroup in availablePositionGroups)
            {
                var groupPositions = GetPositionsForGroup(positionGroup);
                foreach (var position in groupPositions)
                {
                    if (!allPositions.Contains(position))
                    {
                        allPositions.Add(position);
                    }
                }
            }

            return allPositions;
        }

        // Helper method to get positions for a given group
        private List<TacticalPositionOption> GetPositionsForGroup(TacticalPositionGroupOption group)
        {
            // This would need to be implemented based on your position group definitions
            // For now, returning empty list - you'll need to implement the actual mapping
            return new List<TacticalPositionOption>();
        }


        private void OnValidate()
        {
            // Ensure default duty is within available duties
            if (availableDuties.Count > 0)
            {
                if (defaultDuty == default || !availableDuties.Contains(defaultDuty))
                {
                    defaultDuty = availableDuties[0];
                }
            }

            if (string.IsNullOrEmpty(roleName))
            {
                roleName = roleOption.ToString();
            }
        }
    }
}