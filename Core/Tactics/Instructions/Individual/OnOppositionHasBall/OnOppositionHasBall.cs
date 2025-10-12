using System;

namespace UltimateFootballSystem.Core.Tactics.Instructions.Individual
{
    public class OnOppositionHasBall
    {
        public PressingFrequencyOption? PressingFrequency { get; set; }
        public PressingStyleOption? PressingStyle { get; set; }
        public bool? TighterMarking { get; set; }
        public TacklingStyleOption? TacklingStyle { get; set; }
        public object OpponentToMark { get; set; }
        public object PositionToMark { get; set; }

        public bool PressingFrequencyRequired { get; private set; } = false;
        public bool PressingStyleRequired { get; private set; } = false;
        public bool TighterMarkingRequired { get; private set; } = false;
        public bool TacklingStyleRequired { get; private set; } = false;

        public OnOppositionHasBall()
        {
            // All options are unavailable unless explicitly set, so null values indicate that
            PressingFrequency = null;
            PressingStyle = null;
            TighterMarking = null;
            TacklingStyle = null;
        }

        public OnOppositionHasBall(
            PressingFrequencyOption? pressingFrequency = null,
            PressingStyleOption? pressingStyle = null,
            bool? tighterMarking = null,
            TacklingStyleOption? tacklingStyle = null)
        {
            // Assign the values
            PressingFrequency = pressingFrequency;
            PressingStyle = pressingStyle;
            TighterMarking = tighterMarking;
            TacklingStyle = tacklingStyle;

            // Set the required flags to true for each attribute
            if (pressingFrequency != null) PressingFrequencyRequired = true;
            if (pressingStyle != null) PressingStyleRequired = true;
            if (tighterMarking != null) TighterMarkingRequired = true;
            if (tacklingStyle != null) TacklingStyleRequired = true;
        }

        public void SetRequiredDefault<T>(T option)
        {
            switch (option)
            {
                case PressingFrequencyOption pressingFrequencyOption:
                    PressingFrequency = pressingFrequencyOption;
                    PressingFrequencyRequired = true;
                    break;
                case PressingStyleOption pressingStyleOption:
                    PressingStyle = pressingStyleOption;
                    PressingStyleRequired = true;
                    break;
                case TacklingStyleOption tacklingStyleOption:
                    TacklingStyle = tacklingStyleOption;
                    TacklingStyleRequired = true;
                    break;
                default:
                    throw new ArgumentException("Invalid option type.");
            }
        }

        public void SetRequiredDefaultBool(bool value)
        {
            TighterMarking = value;
            TighterMarkingRequired = true;
        }

        public void Set<T>(T option)
        {
            switch (option)
            {
                case PressingFrequencyOption _ when PressingFrequency == null || PressingFrequencyRequired:
                    throw new InvalidOperationException("PressingFrequency is not available.");
                case PressingStyleOption _ when PressingStyle == null || PressingStyleRequired:
                    throw new InvalidOperationException("PressingStyle is not available.");
                case TacklingStyleOption _ when TacklingStyle == null || TacklingStyleRequired:
                    throw new InvalidOperationException("TacklingStyle is not available.");
            }
        }

        public void SetBool(bool value)
        {
            if (TighterMarking == null || TighterMarkingRequired)
                throw new InvalidOperationException("TighterMarking is not available.");
            TighterMarking = value;
        }

        public void MakeAvailable<T>()
        {
            switch (typeof(T).Name)
            {
                case nameof(PressingFrequency):
                    PressingFrequency = PressingFrequencyOption.Balanced;
                    PressingFrequencyRequired = false;
                    break;
                case nameof(PressingStyle):
                    PressingStyle = PressingStyleOption.Conservative;
                    PressingStyleRequired = false;
                    break;
                case nameof(TighterMarking):
                    TighterMarking = false;
                    TighterMarkingRequired = false;
                    break;
                case nameof(TacklingStyle):
                    TacklingStyle = TacklingStyleOption.Balanced;
                    TacklingStyleRequired = false;
                    break;
                default:
                    throw new ArgumentException("Invalid option type.");
            }
        }

        public void MakeUnavailable<T>()
        {
            switch (typeof(T).Name)
            {
                case nameof(PressingFrequency):
                    PressingFrequency = null;
                    PressingFrequencyRequired = false;
                    break;
                case nameof(PressingStyle):
                    PressingStyle = null;
                    PressingStyleRequired = false;
                    break;
                case nameof(TighterMarking):
                    TighterMarking = null;
                    TighterMarkingRequired = false;
                    break;
                case nameof(TacklingStyle):
                    TacklingStyle = null;
                    TacklingStyleRequired = false;
                    break;
                default:
                    throw new ArgumentException("Invalid option type.");
            }
        }

        public void MakeAllAvailable()
        {
            PressingFrequency = PressingFrequencyOption.Balanced;
            PressingFrequencyRequired = false;

            PressingStyle = PressingStyleOption.Conservative;
            PressingStyleRequired = false;

            TighterMarking = false;
            TighterMarkingRequired = false;

            TacklingStyle = TacklingStyleOption.Balanced;
            TacklingStyleRequired = false;
        }

        public void MakeAllUnavailable()
        {
            PressingFrequency = null;
            PressingFrequencyRequired = false;

            PressingStyle = null;
            PressingStyleRequired = false;

            TighterMarking = null;
            TighterMarkingRequired = false;

            TacklingStyle = null;
            TacklingStyleRequired = false;
        }
    }
}