using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.TacticsEngine.Instructions.Individual;
using UltimateFootballSystem.Core.TacticsEngine.Utils;

namespace UltimateFootballSystem.Core.TacticsEngine
{
    /// <summary>
    /// Abstract base class for all tactical roles
    /// </summary>
    public abstract class TacticalRole
    {
        // Core properties
        public TacticalRoleOption RoleOption { get; protected set; }
        public string RoleName { get; protected set; }
        public string RoleNameShort { get; protected set; }
        public string RoleDescription { get; protected set; }

        // Available options
        public List<TacticalPositionGroupOption> AvailablePositionGroups { get; protected set; }
        public List<TacticalPositionOption> AvailablePositions { get; protected set; }
        public List<TacticalDutyOption> AvailableDuties { get; protected set; }

        // Selected options
        public TacticalDutyOption SelectedDuty { get; protected set; }
        public TacticalPositionOption SelectedPosition { get; protected set; }

        // Role capabilities flags
        protected bool CanAttack { get; set; }
        protected bool CanSupport { get; set; }
        protected bool CanDefend { get; set; }
        protected bool CanStopper { get; set; }
        protected bool CanCover { get; set; }

        // Instruction management
        protected Dictionary<TacticalDutyOption, IndividualInstruction> _dutyInstructions;
        private IndividualInstruction _customInstructions;
        private bool _hasCustomInstructions;
        public IndividualInstruction Instructions 
        { 
            get 
            {
                if (_hasCustomInstructions)
                    return _customInstructions;
                
                return _dutyInstructions[SelectedDuty];
            }
        }

        // Zone ownership
        public Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption> ZonesOwned { get; protected set; }
            = new Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption>();

        // Default selections
        private TacticalDutyOption _defaultDuty;
        private TacticalPositionOption _defaultPosition;

        protected TacticalRole(
            TacticalRoleOption roleOption,
            string roleName,
            string roleNameShort,
            string roleDescription,
            List<TacticalPositionGroupOption> availablePositionGroups,
            List<TacticalDutyOption> availableDuties)
        {
            RoleOption = roleOption;
            RoleName = roleName;
            RoleNameShort = roleNameShort;
            RoleDescription = roleDescription;

            // Initialize positions
            InitializePositions(availablePositionGroups);

            // Initialize duties
            InitializeDuties(availableDuties);

            // Initialize duty instructions
            InitializeDutyInstructions();
        }

        // Initialize positions based on available position groups
        private void InitializePositions(List<TacticalPositionGroupOption> availablePositionGroups)
        {
            AvailablePositionGroups = availablePositionGroups;
            AvailablePositions = PositionGroupManager.GetPositionOptionsForGroupAll(AvailablePositionGroups.ToArray());

            _defaultPosition = AvailablePositions.FirstOrDefault();
            SetSelectedPosition(_defaultPosition);
        }

        // Initialize duties and capability flags
        private void InitializeDuties(List<TacticalDutyOption> availableDuties)
        {
            AvailableDuties = availableDuties;
            CanAttack = AvailableDuties.Contains(TacticalDutyOption.Attack);
            CanSupport = AvailableDuties.Contains(TacticalDutyOption.Support);
            CanDefend = AvailableDuties.Contains(TacticalDutyOption.Defend);
            CanStopper = AvailableDuties.Contains(TacticalDutyOption.Stopper);
            CanCover = AvailableDuties.Contains(TacticalDutyOption.Cover);

            _defaultDuty = AvailableDuties.FirstOrDefault();
            SetSelectedDuty(_defaultDuty);
        }

        // Initialize duty instructions
        private void InitializeDutyInstructions()
        {
            _dutyInstructions = new Dictionary<TacticalDutyOption, IndividualInstruction>();
            foreach (var duty in AvailableDuties)
            {
                _dutyInstructions[duty] = CreateDefaultInstructionsForDuty(duty);
            }
        }

        // Set selected duty if valid
        public void SetSelectedDuty(TacticalDutyOption duty)
        {
            if (AvailableDuties.Contains(duty))
            {
                SelectedDuty = duty;
            }
            else
            {
                SelectedDuty = _defaultDuty;
            }
            // Update zones whenever duty changes
            UpdateZonesOwned();
        }

        // Set selected position if valid
        public void SetSelectedPosition(TacticalPositionOption position)
        {
            if (AvailablePositions.Contains(position))
            {
                SelectedPosition = position;
            }
            else
            {
                SelectedPosition = _defaultPosition;
            }
            // Update zones whenever position changes
            UpdateZonesOwned();
        }

        // Get default duty
        public TacticalDutyOption GetDefaultDuty() => _defaultDuty;

        // Get default position
        public TacticalPositionOption GetDefaultPosition() => _defaultPosition;

        // Update zones owned by this role
        protected void UpdateZonesOwned()
        {
            ZonesOwned = GetAvailableZones(SelectedPosition, SelectedDuty);
        }

        // Get availability level for a specific zone
        public TacticalZoneAvailabilityOption GetZoneAvailability(TacticalZoneOption zone)
        {
            return ZonesOwned.TryGetValue(zone, out var level) ? level : TacticalZoneAvailabilityOption.None;
        }

        // Abstract method to be implemented by each role
        public abstract Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption> GetAvailableZones(
            TacticalPositionOption position,
            TacticalDutyOption duty);

        // Create a copy of this role
        public abstract TacticalRole Clone();

        // Protected methods for instruction management
        protected abstract IndividualInstruction CreateDefaultInstructionsForDuty(TacticalDutyOption duty);

        protected void SetCustomInstructions(IndividualInstruction instructions)
        {
            _customInstructions = instructions;
            _hasCustomInstructions = true;
        }

        protected void ClearCustomInstructions()
        {
            _customInstructions = null;
            _hasCustomInstructions = false;
        }

        protected bool HasCustomInstructions => _hasCustomInstructions;
    }
}
