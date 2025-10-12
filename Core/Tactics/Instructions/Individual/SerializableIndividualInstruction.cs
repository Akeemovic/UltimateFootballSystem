using System;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual.OnPlayerHasBall;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual.OnTeamHasBall;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual.OnOppositionHasBall;

namespace UltimateFootballSystem.Core.Tactics.Instructions.Individual
{
    /// <summary>
    /// Defines the availability state of an individual instruction option
    /// </summary>
    public enum InstructionAvailability
    {
        Unavailable = 0,  // Instruction is hidden/disabled (null at runtime)
        Available = 1,    // Instruction is optional, user can customize (value + Required=false)
        Required = 2      // Instruction is locked with default value (value + Required=true)
    }

    /// <summary>
    /// Serializable wrapper for IndividualInstruction that can be stored in ScriptableObjects
    /// </summary>
    [Serializable]
    public class SerializableIndividualInstruction
    {
        public SerializableOnPlayerHasBall onPlayerHasBall = new SerializableOnPlayerHasBall();
        public SerializableOnTeamHasBall onTeamHasBall = new SerializableOnTeamHasBall();
        public SerializableOnOppositionHasBall onOppositionHasBall = new SerializableOnOppositionHasBall();

        /// <summary>
        /// Convert this serializable data into a runtime IndividualInstruction instance
        /// </summary>
        public IndividualInstruction ToRuntimeInstruction(TacticalPositionGroupOption positionGroup)
        {
            var instruction = new IndividualInstruction(positionGroup);

            // Apply OnPlayerHasBall configuration
            onPlayerHasBall.ApplyToRuntime(instruction.OnPlayerHasBall);

            return instruction;
        }

        /// <summary>
        /// Populate this serializable data from a runtime IndividualInstruction instance
        /// </summary>
        public void FromRuntimeInstruction(IndividualInstruction instruction)
        {
            if (instruction?.OnPlayerHasBall != null)
            {
                onPlayerHasBall.FromRuntime(instruction.OnPlayerHasBall);
            }
        }
    }

    /// <summary>
    /// Serializable wrapper for OnPlayerHasBall instructions
    /// </summary>
    [Serializable]
    public class SerializableOnPlayerHasBall
    {
        // Hold Up Ball
        public InstructionAvailability holdUpBallAvailability = InstructionAvailability.Unavailable;
        public HoldUpBallOption holdUpBallDefault = HoldUpBallOption.None;

        // Wing Play
        public InstructionAvailability wingPlayAvailability = InstructionAvailability.Unavailable;
        public WingPlayOption wingPlayDefault = WingPlayOption.None;

        // Shooting Frequency
        public InstructionAvailability shootingFrequencyAvailability = InstructionAvailability.Unavailable;
        public ShootingFrequencyOption shootingFrequencyDefault = ShootingFrequencyOption.None;

        // Dribbling Frequency
        public InstructionAvailability dribblingFrequencyAvailability = InstructionAvailability.Unavailable;
        public DribblingFrequencyOption dribblingFrequencyDefault = DribblingFrequencyOption.None;

        // Crossing Frequency
        public InstructionAvailability crossingFrequencyAvailability = InstructionAvailability.Unavailable;
        public CrossingFrequencyOption crossingFrequencyDefault = CrossingFrequencyOption.None;

        // Cross Distance
        public InstructionAvailability crossDistanceAvailability = InstructionAvailability.Unavailable;
        public CrossDistanceOption crossDistanceDefault = CrossDistanceOption.None;

        // Cross Target
        public InstructionAvailability crossTargetAvailability = InstructionAvailability.Unavailable;
        public CrossTargetOption crossTargetDefault = CrossTargetOption.None;

        // Passing Style
        public InstructionAvailability passingStyleAvailability = InstructionAvailability.Unavailable;
        public PassingStyleOption passingStyleDefault = PassingStyleOption.None;

        // Creative Passing
        public InstructionAvailability creativePassingAvailability = InstructionAvailability.Unavailable;
        public CreativePassingOption creativePassingDefault = CreativePassingOption.None;

        /// <summary>
        /// Apply this configuration to a runtime OnPlayerHasBall instance
        /// </summary>
        public void ApplyToRuntime(OnPlayerHasBall.OnPlayerHasBall runtime)
        {
            // Hold Up Ball
            ApplyInstruction(runtime, holdUpBallAvailability, holdUpBallDefault,
                (val) => runtime.HoldUpBall = val,
                (val) => runtime.SetRequiredDefault(val));

            // Wing Play
            ApplyInstruction(runtime, wingPlayAvailability, wingPlayDefault,
                (val) => runtime.WingPlay = val,
                (val) => runtime.SetRequiredDefault(val));

            // Shooting Frequency
            ApplyInstruction(runtime, shootingFrequencyAvailability, shootingFrequencyDefault,
                (val) => runtime.ShootingFrequency = val,
                (val) => runtime.SetRequiredDefault(val));

            // Dribbling Frequency
            ApplyInstruction(runtime, dribblingFrequencyAvailability, dribblingFrequencyDefault,
                (val) => runtime.DribblingFrequency = val,
                (val) => runtime.SetRequiredDefault(val));

            // Crossing Frequency
            ApplyInstruction(runtime, crossingFrequencyAvailability, crossingFrequencyDefault,
                (val) => runtime.CrossingFrequency = val,
                (val) => runtime.SetRequiredDefault(val));

            // Cross Distance
            ApplyInstruction(runtime, crossDistanceAvailability, crossDistanceDefault,
                (val) => runtime.CrossDistance = val,
                (val) => runtime.SetRequiredDefault(val));

            // Cross Target
            ApplyInstruction(runtime, crossTargetAvailability, crossTargetDefault,
                (val) => runtime.CrossTarget = val,
                (val) => runtime.SetRequiredDefault(val));

            // Passing Style
            ApplyInstruction(runtime, passingStyleAvailability, passingStyleDefault,
                (val) => runtime.PassingStyle = val,
                (val) => runtime.SetRequiredDefault(val));

            // Creative Passing
            ApplyInstruction(runtime, creativePassingAvailability, creativePassingDefault,
                (val) => runtime.CreativePassing = val,
                (val) => runtime.SetRequiredDefault(val));
        }

        private void ApplyInstruction<T>(
            OnPlayerHasBall.OnPlayerHasBall runtime,
            InstructionAvailability availability,
            T defaultValue,
            Action<T?> setAvailable,
            Action<T> setRequired) where T : struct
        {
            switch (availability)
            {
                case InstructionAvailability.Unavailable:
                    // Leave as null (default state)
                    setAvailable(null);
                    break;

                case InstructionAvailability.Available:
                    // Set value, mark as available (not required)
                    setAvailable(defaultValue);
                    break;

                case InstructionAvailability.Required:
                    // Set value and mark as required
                    setRequired(defaultValue);
                    break;
            }
        }

        /// <summary>
        /// Populate this serializable data from a runtime OnPlayerHasBall instance
        /// </summary>
        public void FromRuntime(OnPlayerHasBall.OnPlayerHasBall runtime)
        {
            // Hold Up Ball
            GetInstructionState(runtime.HoldUpBall, runtime.HoldUpBallRequired,
                out holdUpBallAvailability, out holdUpBallDefault);

            // Wing Play
            GetInstructionState(runtime.WingPlay, runtime.WingPlayRequired,
                out wingPlayAvailability, out wingPlayDefault);

            // Shooting Frequency
            GetInstructionState(runtime.ShootingFrequency, runtime.ShootingFrequencyRequired,
                out shootingFrequencyAvailability, out shootingFrequencyDefault);

            // Dribbling Frequency
            GetInstructionState(runtime.DribblingFrequency, runtime.DribblingFrequencyRequired,
                out dribblingFrequencyAvailability, out dribblingFrequencyDefault);

            // Crossing Frequency
            GetInstructionState(runtime.CrossingFrequency, runtime.CrossingFrequencyRequired,
                out crossingFrequencyAvailability, out crossingFrequencyDefault);

            // Cross Distance
            GetInstructionState(runtime.CrossDistance, runtime.CrossDistanceRequired,
                out crossDistanceAvailability, out crossDistanceDefault);

            // Cross Target
            GetInstructionState(runtime.CrossTarget, runtime.CrossTargetRequired,
                out crossTargetAvailability, out crossTargetDefault);

            // Passing Style
            GetInstructionState(runtime.PassingStyle, runtime.PassingStyleRequired,
                out passingStyleAvailability, out passingStyleDefault);

            // Creative Passing
            GetInstructionState(runtime.CreativePassing, runtime.CreativePassingRequired,
                out creativePassingAvailability, out creativePassingDefault);
        }

        private void GetInstructionState<T>(
            T? value,
            bool required,
            out InstructionAvailability availability,
            out T defaultValue) where T : struct
        {
            if (!value.HasValue)
            {
                availability = InstructionAvailability.Unavailable;
                defaultValue = default;
            }
            else if (required)
            {
                availability = InstructionAvailability.Required;
                defaultValue = value.Value;
            }
            else
            {
                availability = InstructionAvailability.Available;
                defaultValue = value.Value;
            }
        }
    }

    /// <summary>
    /// Serializable wrapper for OnTeamHasBall instructions
    /// </summary>
    [Serializable]
    public class SerializableOnTeamHasBall
    {
        // More Forward Runs
        public InstructionAvailability moreForwardRunsAvailability = InstructionAvailability.Unavailable;
        public MoreForwardRunsOption moreForwardRunsDefault = MoreForwardRunsOption.None;

        // Open Channel Runs
        public InstructionAvailability openChannelRunsAvailability = InstructionAvailability.Unavailable;
        public OpenChannelRunsOption openChannelRunsDefault = OpenChannelRunsOption.None;

        // Mobility
        public InstructionAvailability mobilityAvailability = InstructionAvailability.Unavailable;
        public MobilityOption mobilityDefault = MobilityOption.None;

        // Positioning Width
        public InstructionAvailability positioningWidthAvailability = InstructionAvailability.Unavailable;
        public PositioningWidthOption positioningWidthDefault = PositioningWidthOption.None;

        public void ApplyToRuntime(OnTeamHasBall.OnTeamHasBall runtime)
        {
            // More Forward Runs
            ApplyInstruction(runtime, moreForwardRunsAvailability, moreForwardRunsDefault,
                (val) => runtime.MoreForwardRuns = val,
                (val) => runtime.SetRequiredDefault(val));

            // Open Channel Runs
            ApplyInstruction(runtime, openChannelRunsAvailability, openChannelRunsDefault,
                (val) => runtime.OpenChannelRuns = val,
                (val) => runtime.SetRequiredDefault(val));

            // Mobility
            ApplyInstruction(runtime, mobilityAvailability, mobilityDefault,
                (val) => runtime.Mobility = val,
                (val) => runtime.SetRequiredDefault(val));

            // Positioning Width
            ApplyInstruction(runtime, positioningWidthAvailability, positioningWidthDefault,
                (val) => runtime.PositioningWidth = val,
                (val) => runtime.SetRequiredDefault(val));
        }

        private void ApplyInstruction<T>(
            OnTeamHasBall.OnTeamHasBall runtime,
            InstructionAvailability availability,
            T defaultValue,
            Action<T?> setAvailable,
            Action<T> setRequired) where T : struct
        {
            switch (availability)
            {
                case InstructionAvailability.Unavailable:
                    setAvailable(null);
                    break;
                case InstructionAvailability.Available:
                    setAvailable(defaultValue);
                    break;
                case InstructionAvailability.Required:
                    setRequired(defaultValue);
                    break;
            }
        }

        public void FromRuntime(OnTeamHasBall.OnTeamHasBall runtime)
        {
            // More Forward Runs
            GetInstructionState(runtime.MoreForwardRuns, runtime.MoreForwardRunsRequired,
                out moreForwardRunsAvailability, out moreForwardRunsDefault);

            // Open Channel Runs
            GetInstructionState(runtime.OpenChannelRuns, runtime.OpenChannelRunsRequired,
                out openChannelRunsAvailability, out openChannelRunsDefault);

            // Mobility
            GetInstructionState(runtime.Mobility, runtime.MobilityRequired,
                out mobilityAvailability, out mobilityDefault);

            // Positioning Width
            GetInstructionState(runtime.PositioningWidth, runtime.PositioningWidthRequired,
                out positioningWidthAvailability, out positioningWidthDefault);
        }

        private void GetInstructionState<T>(
            T? value,
            bool required,
            out InstructionAvailability availability,
            out T defaultValue) where T : struct
        {
            if (!value.HasValue)
            {
                availability = InstructionAvailability.Unavailable;
                defaultValue = default;
            }
            else if (required)
            {
                availability = InstructionAvailability.Required;
                defaultValue = value.Value;
            }
            else
            {
                availability = InstructionAvailability.Available;
                defaultValue = value.Value;
            }
        }
    }

    /// <summary>
    /// Serializable wrapper for OnOppositionHasBall instructions
    /// </summary>
    [Serializable]
    public class SerializableOnOppositionHasBall
    {
        // Pressing Frequency
        public InstructionAvailability pressingFrequencyAvailability = InstructionAvailability.Unavailable;
        public PressingFrequencyOption pressingFrequencyDefault = PressingFrequencyOption.Balanced;

        // Pressing Style
        public InstructionAvailability pressingStyleAvailability = InstructionAvailability.Unavailable;
        public PressingStyleOption pressingStyleDefault = PressingStyleOption.Conservative;

        // Tighter Marking
        public InstructionAvailability tighterMarkingAvailability = InstructionAvailability.Unavailable;
        public bool tighterMarkingDefault = false;

        // Tackling Style
        public InstructionAvailability tacklingStyleAvailability = InstructionAvailability.Unavailable;
        public TacklingStyleOption tacklingStyleDefault = TacklingStyleOption.Balanced;

        public void ApplyToRuntime(OnOppositionHasBall.OnOppositionHasBall runtime)
        {
            // Pressing Frequency
            ApplyInstruction(runtime, pressingFrequencyAvailability, pressingFrequencyDefault,
                (val) => runtime.PressingFrequency = val,
                (val) => runtime.SetRequiredDefault(val));

            // Pressing Style
            ApplyInstruction(runtime, pressingStyleAvailability, pressingStyleDefault,
                (val) => runtime.PressingStyle = val,
                (val) => runtime.SetRequiredDefault(val));

            // Tighter Marking
            ApplyBoolInstruction(runtime, tighterMarkingAvailability, tighterMarkingDefault,
                (val) => runtime.TighterMarking = val,
                (val) => runtime.SetRequiredDefaultBool(val));

            // Tackling Style
            ApplyInstruction(runtime, tacklingStyleAvailability, tacklingStyleDefault,
                (val) => runtime.TacklingStyle = val,
                (val) => runtime.SetRequiredDefault(val));
        }

        private void ApplyInstruction<T>(
            OnOppositionHasBall.OnOppositionHasBall runtime,
            InstructionAvailability availability,
            T defaultValue,
            Action<T?> setAvailable,
            Action<T> setRequired) where T : struct
        {
            switch (availability)
            {
                case InstructionAvailability.Unavailable:
                    setAvailable(null);
                    break;
                case InstructionAvailability.Available:
                    setAvailable(defaultValue);
                    break;
                case InstructionAvailability.Required:
                    setRequired(defaultValue);
                    break;
            }
        }

        private void ApplyBoolInstruction(
            OnOppositionHasBall.OnOppositionHasBall runtime,
            InstructionAvailability availability,
            bool defaultValue,
            Action<bool?> setAvailable,
            Action<bool> setRequired)
        {
            switch (availability)
            {
                case InstructionAvailability.Unavailable:
                    setAvailable(null);
                    break;
                case InstructionAvailability.Available:
                    setAvailable(defaultValue);
                    break;
                case InstructionAvailability.Required:
                    setRequired(defaultValue);
                    break;
            }
        }

        public void FromRuntime(OnOppositionHasBall.OnOppositionHasBall runtime)
        {
            // Pressing Frequency
            GetInstructionState(runtime.PressingFrequency, runtime.PressingFrequencyRequired,
                out pressingFrequencyAvailability, out pressingFrequencyDefault);

            // Pressing Style
            GetInstructionState(runtime.PressingStyle, runtime.PressingStyleRequired,
                out pressingStyleAvailability, out pressingStyleDefault);

            // Tighter Marking
            GetBoolInstructionState(runtime.TighterMarking, runtime.TighterMarkingRequired,
                out tighterMarkingAvailability, out tighterMarkingDefault);

            // Tackling Style
            GetInstructionState(runtime.TacklingStyle, runtime.TacklingStyleRequired,
                out tacklingStyleAvailability, out tacklingStyleDefault);
        }

        private void GetInstructionState<T>(
            T? value,
            bool required,
            out InstructionAvailability availability,
            out T defaultValue) where T : struct
        {
            if (!value.HasValue)
            {
                availability = InstructionAvailability.Unavailable;
                defaultValue = default;
            }
            else if (required)
            {
                availability = InstructionAvailability.Required;
                defaultValue = value.Value;
            }
            else
            {
                availability = InstructionAvailability.Available;
                defaultValue = value.Value;
            }
        }

        private void GetBoolInstructionState(
            bool? value,
            bool required,
            out InstructionAvailability availability,
            out bool defaultValue)
        {
            if (!value.HasValue)
            {
                availability = InstructionAvailability.Unavailable;
                defaultValue = false;
            }
            else if (required)
            {
                availability = InstructionAvailability.Required;
                defaultValue = value.Value;
            }
            else
            {
                availability = InstructionAvailability.Available;
                defaultValue = value.Value;
            }
        }
    }
}
