using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual;
using UltimateFootballSystem.Core.Tactics.Utils;
using UnityEngine;

namespace UltimateFootballSystem.Core.Tactics
{
    /// <summary>
    /// Runtime instance of a tactical role based on a TacticalRoleDefinition
    /// </summary>
    public class TacticalRole
    {
        // Reference to the definition asset
        private TacticalRoleDefinition _definition;

        // Basic Information (from definition)
        public TacticalRoleOption RoleOption => _definition.roleOption;
        public string RoleName => _definition.roleName;

        // Position Compatibility (from definition)
        public List<TacticalPositionOption> AvailablePositions => _definition.availablePositions;

        // Duty Options (from definition)
        public List<TacticalDutyOption> AvailableDuties => _definition.availableDuties;
        public TacticalDutyOption SelectedDuty { get; private set; }

        // Runtime State
        public TacticalPositionOption SelectedPosition { get; private set; }
        public Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption> ZonesOwned { get; private set; } = new Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption>();

        // Instructions (individual instruction system)
        private IndividualInstruction _baseInstructions;
        private IndividualInstruction _customInstructions;
        private bool _hasCustomInstructions;

        public TacticalRole(TacticalRoleDefinition definition)
        {
            _definition = definition;

            // Initialize base instructions based on the most common position group for this role
            var primaryPositionGroup = GetPrimaryPositionGroup();
            _baseInstructions = _definition.CreateIndividualInstruction(primaryPositionGroup);

            // Set default selections
            if (AvailableDuties.Count > 0)
            {
                SelectedDuty = _definition.defaultDuty != default ? _definition.defaultDuty : AvailableDuties[0];
            }

            if (AvailablePositions.Count > 0)
            {
                SelectedPosition = AvailablePositions[0];
            }

            UpdateZonesOwned();
        }

        private TacticalPositionGroupOption GetPrimaryPositionGroup()
        {
            // Get the position group of the first available position
            if (AvailablePositions.Count > 0)
            {
                return TacticalPositionUtils.GetGroupForPosition(AvailablePositions[0]);
            }
            return TacticalPositionGroupOption.M_Center; // Default fallback
        }

        // Set selected duty if valid
        public void SetSelectedDuty(TacticalDutyOption duty)
        {
            if (AvailableDuties.Contains(duty))
            {
                SelectedDuty = duty;
                UpdateZonesOwned();
            }
        }

        // Set selected position if valid
        public void SetSelectedPosition(TacticalPositionOption position)
        {
            if (AvailablePositions.Contains(position))
            {
                SelectedPosition = position;
                UpdateZonesOwned();
            }
        }

        // Update zones owned by this role
        private void UpdateZonesOwned()
        {
            if (_definition != null)
            {
                ZonesOwned = _definition.GetZonesForPositionAndDuty(SelectedPosition, SelectedDuty);
            }
        }

        // Get availability level for a specific zone
        public TacticalZoneAvailabilityOption GetZoneAvailability(TacticalZoneOption zone)
        {
            return ZonesOwned.TryGetValue(zone, out var level) ? level : TacticalZoneAvailabilityOption.None;
        }

        // Create a copy of this role
        public TacticalRole Clone()
        {
            var clone = new TacticalRole(_definition);
            clone.SetSelectedPosition(SelectedPosition);
            clone.SetSelectedDuty(SelectedDuty);

            // Copy any custom instructions if they exist
            if (_hasCustomInstructions)
            {
                // Deep copy custom instructions
                clone._customInstructions = _customInstructions; // Note: might need deep copy depending on implementation
                clone._hasCustomInstructions = true;
            }

            return clone;
        }

        // Custom instruction support
        public void SetCustomInstructions(IndividualInstruction instructions)
        {
            _customInstructions = instructions;
            _hasCustomInstructions = true;
        }

        public void ClearCustomInstructions()
        {
            _customInstructions = null;
            _hasCustomInstructions = false;
        }

        public bool HasCustomInstructions => _hasCustomInstructions;

        public IndividualInstruction GetInstructions()
        {
            if (_hasCustomInstructions)
                return _customInstructions;

            return _baseInstructions;
        }
    }
}
