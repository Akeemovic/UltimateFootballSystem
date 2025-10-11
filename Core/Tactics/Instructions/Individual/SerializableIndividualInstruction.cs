using System;
using UltimateFootballSystem.Core.Tactics.Instructions.Individual.OnPlayerHasBall;

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
        // Future: Add OnTeamHasBall and OnOppositionHasBall wrappers when needed
        // public SerializableOnTeamHasBall onTeamHasBall = new SerializableOnTeamHasBall();
        // public SerializableOnOppositionHasBall onOppositionHasBall = new SerializableOnOppositionHasBall();

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
}
