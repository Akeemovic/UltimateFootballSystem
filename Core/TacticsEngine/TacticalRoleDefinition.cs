using System.Collections.Generic;
using UnityEngine;
using UltimateFootballSystem.Core.TacticsEngine.Instructions.Individual;

namespace UltimateFootballSystem.Core.TacticsEngine
{
    [CreateAssetMenu(fileName = "New Tactical Role", menuName = "Ultimate Football System/Tactical Role Definition")]
    public class TacticalRoleDefinition : ScriptableObject
    {
        [Header("Basic Information")]
        public TacticalRoleOption roleOption;
        public string roleName;

        [Header("Position Compatibility")]
        public List<TacticalPositionOption> availablePositions = new List<TacticalPositionOption>();
        public List<TacticalPositionGroupOption> availablePositionGroups = new List<TacticalPositionGroupOption>();

        [Header("Duty Options")]
        public List<TacticalDutyOption> availableDuties = new List<TacticalDutyOption>();
        public TacticalDutyOption defaultDuty;

        [Header("Zone Definitions")]
        public List<ZoneDefinition> zoneDefinitions = new List<ZoneDefinition>();

        [Header("Instructions")]
        public IndividualInstruction individualInstruction;
        // [SerializeField] private IndividualInstruction individualInstruction;
        // [SerializeField] private TacticalPositionGroupOption defaultPositionGroup = TacticalPositionGroupOption.M_Center;

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
            // Return configured instruction or create new one if not set
            if (individualInstruction != null)
            {
                return individualInstruction;
            }

            var instruction = new IndividualInstruction(positionGroup);
            return instruction;
        }

        // Configure the individual instruction for this role
        public void SetIndividualInstructionOption(IndividualInstruction instruction)
        {
            individualInstruction = instruction;
        }

        // Get the configured individual instruction
        public IndividualInstruction GetIndividualInstruction()
        {
            return individualInstruction;
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
            if (availableDuties.Count > 0 && defaultDuty == default)
            {
                defaultDuty = availableDuties[0];
            }

            if (string.IsNullOrEmpty(roleName))
            {
                roleName = roleOption.ToString();
            }
        }
    }
}