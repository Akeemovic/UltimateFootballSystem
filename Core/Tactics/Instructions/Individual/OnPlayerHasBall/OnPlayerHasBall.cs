using System;

namespace UltimateFootballSystem.Core.Tactics.Instructions.Individual
{
    public class OnPlayerHasBall
    {
        public HoldUpBallOption? HoldUpBall { get; set; }
        public WingPlayOption? WingPlay { get; set; }
        public ShootingFrequencyOption? ShootingFrequency { get; set; }
        public DribblingFrequencyOption? DribblingFrequency { get; set; }
        public CrossingFrequencyOption? CrossingFrequency { get; set; }
        public CrossDistanceOption? CrossDistance { get; set; }
        public CrossTargetOption? CrossTarget { get; set; }
        public PassingStyleOption? PassingStyle { get; set; }
        public CreativePassingOption? CreativePassing { get; set; }

        public bool HoldUpBallRequired { get; private set; } = false;
        public bool WingPlayRequired { get; private set; } = false;
        public bool ShootingFrequencyRequired { get; private set; } = false;
        public bool DribblingFrequencyRequired { get; private set; } = false;
        public bool CrossingFrequencyRequired { get; private set; } = false;
        public bool CrossDistanceRequired { get; private set; } = false;
        public bool CrossTargetRequired { get; private set; } = false;
        public bool PassingStyleRequired { get; private set; } = false;
        public bool CreativePassingRequired { get; private set; } = false;

        public OnPlayerHasBall()
        {
            // All options are unavailable unless explicitly set, so null values indicate that 
            HoldUpBall = null;
            WingPlay = null;
            ShootingFrequency = null;
            DribblingFrequency = null;
            CrossingFrequency = null;
            CrossDistance = null;
            CrossTarget = null;
            PassingStyle = null;
            CreativePassing = null;
        }

        public OnPlayerHasBall(
            HoldUpBallOption? holdUpBall = null,
            WingPlayOption? wingPlay = null,
            ShootingFrequencyOption? shootingFrequency = null,
            DribblingFrequencyOption? dribblingFrequency = null,
            CrossingFrequencyOption? crossingFrequency = null,
            CrossDistanceOption? crossDistance = null,
            CrossTargetOption? crossTarget = null,
            PassingStyleOption? passingStyle = null,
            CreativePassingOption? creativePassing = null)
        {
            // Assign the values
            HoldUpBall = holdUpBall;
            WingPlay = wingPlay;
            ShootingFrequency = shootingFrequency;
            DribblingFrequency = dribblingFrequency;
            CrossingFrequency = crossingFrequency;
            CrossDistance = crossDistance;
            CrossTarget = crossTarget;
            PassingStyle = passingStyle;
            CreativePassing = creativePassing;

            // Set the required flags to true for each attribute
            if (holdUpBall != null) HoldUpBallRequired = true;
            if (wingPlay != null) WingPlayRequired = true;
            if (shootingFrequency != null) ShootingFrequencyRequired = true;
            if (dribblingFrequency != null) DribblingFrequencyRequired = true;
            if (crossingFrequency != null) CrossingFrequencyRequired = true;
            if (crossDistance != null) CrossDistanceRequired = true;
            if (crossTarget != null) CrossTargetRequired = true;
            if (passingStyle != null) PassingStyleRequired = true;
            if (creativePassing != null) CreativePassingRequired = true;
        }

        public void SetRequiredDefault<T>(T option)
        {
            switch (option)
            {
                case HoldUpBallOption holdUpBallOption:
                    HoldUpBall = holdUpBallOption;
                    HoldUpBallRequired = true;
                    break;
                case WingPlayOption wingPlayOption:
                    WingPlay = wingPlayOption;
                    WingPlayRequired = true;
                    break;
                case ShootingFrequencyOption shootingFrequencyOption:
                    ShootingFrequency = shootingFrequencyOption;
                    ShootingFrequencyRequired = true;
                    break;
                case DribblingFrequencyOption dribblingFrequencyOption:
                    DribblingFrequency = dribblingFrequencyOption;
                    DribblingFrequencyRequired = true;
                    break;
                case CrossingFrequencyOption crossingFrequencyOption:
                    CrossingFrequency = crossingFrequencyOption;
                    CrossingFrequencyRequired = true;
                    break;
                case CrossDistanceOption crossDistanceOption:
                    CrossDistance = crossDistanceOption;
                    CrossDistanceRequired = true;
                    break;
                case CrossTargetOption crossTargetOption:
                    CrossTarget = crossTargetOption;
                    CrossTargetRequired = true;
                    break;
                case PassingStyleOption passingStyleOption:
                    PassingStyle = passingStyleOption;
                    PassingStyleRequired = true;
                    break;
                case CreativePassingOption creativePassingOption:
                    CreativePassing = creativePassingOption;
                    CreativePassingRequired = true;
                    break;
                default:
                    throw new ArgumentException("Invalid option type.");
            }
        }

        public void Set<T>(T option)
        {
            switch (option)
            {
                case HoldUpBallOption _ when HoldUpBall == null || HoldUpBallRequired:
                    throw new InvalidOperationException("HoldUpBall is not available.");
                case WingPlayOption _ when WingPlay == null || WingPlayRequired:
                    throw new InvalidOperationException("WingPlay is not available.");
                case ShootingFrequencyOption _ when ShootingFrequency == null || ShootingFrequencyRequired:
                    throw new InvalidOperationException("ShootingFrequency is not available.");
                case DribblingFrequencyOption _ when DribblingFrequency == null || DribblingFrequencyRequired:
                    throw new InvalidOperationException("DribblingFrequency is not available.");
                case CrossingFrequencyOption _ when CrossingFrequency == null || CrossingFrequencyRequired:
                    throw new InvalidOperationException("CrossingFrequency is not available.");
                case CrossDistanceOption _ when CrossDistance == null || CrossDistanceRequired:
                    throw new InvalidOperationException("CrossDistance is not available.");
                case CrossTargetOption _ when CrossTarget == null || CrossTargetRequired:
                    throw new InvalidOperationException("CrossTarget is not available.");
                case PassingStyleOption _ when PassingStyle == null || PassingStyleRequired:
                    throw new InvalidOperationException("PassingStyle is not available.");
                case CreativePassingOption _ when CreativePassing == null || CreativePassingRequired:
                    throw new InvalidOperationException("CreativePassing is not available.");
            }
        }

        public void MakeAvailable<T>()
        {
            switch (typeof(T).Name)
            {
                case nameof(HoldUpBall):
                    HoldUpBall = HoldUpBallOption.None;
                    HoldUpBallRequired = false;
                    break;
                case nameof(WingPlay):
                    WingPlay = WingPlayOption.None;
                    WingPlayRequired = false;
                    break;
                case nameof(ShootingFrequency):
                    ShootingFrequency = ShootingFrequencyOption.None;
                    ShootingFrequencyRequired = false;
                    break;
                case nameof(DribblingFrequency):
                    DribblingFrequency = DribblingFrequencyOption.None;
                    DribblingFrequencyRequired = false;
                    break;
                case nameof(CrossingFrequency):
                    CrossingFrequency = CrossingFrequencyOption.None;
                    CrossingFrequencyRequired = false;
                    break;
                case nameof(CrossDistance):
                    CrossDistance = CrossDistanceOption.None;
                    CrossDistanceRequired = false;
                    break;
                case nameof(CrossTarget):
                    CrossTarget = CrossTargetOption.None;
                    CrossTargetRequired = false;
                    break;
                case nameof(PassingStyle):
                    PassingStyle = PassingStyleOption.None;
                    PassingStyleRequired = false;
                    break;
                case nameof(CreativePassing):
                    CreativePassing = CreativePassingOption.None;
                    CreativePassingRequired = false;
                    break;
                default:
                    throw new ArgumentException("Invalid option type.");
            }
        }

        public void MakeUnavailable<T>()
        {
            switch (typeof(T).Name)
            {
                case nameof(HoldUpBall):
                    HoldUpBall = null;
                    HoldUpBallRequired = false;
                    break;
                case nameof(WingPlay):
                    WingPlay = null;
                    WingPlayRequired = false;
                    break;
                case nameof(ShootingFrequency):
                    ShootingFrequency = null;
                    ShootingFrequencyRequired = false;
                    break;
                case nameof(DribblingFrequency):
                    DribblingFrequency = null;
                    DribblingFrequencyRequired = false;
                    break;
                case nameof(CrossingFrequency):
                    CrossingFrequency = null;
                    CrossingFrequencyRequired = false;
                    break;
                case nameof(CrossDistance):
                    CrossDistance = null;
                    CrossDistanceRequired = false;
                    break;
                case nameof(CrossTarget):
                    CrossTarget = null;
                    CrossTargetRequired = false;
                    break;
                case nameof(PassingStyle):
                    PassingStyle = null;
                    PassingStyleRequired = false;
                    break;
                case nameof(CreativePassing):
                    CreativePassing = null;
                    CreativePassingRequired = false;
                    break;
                default:
                    throw new ArgumentException("Invalid option type.");
            }
        }

        public void MakeAllAvailable()
        {
            HoldUpBall = HoldUpBallOption.None;
            HoldUpBallRequired = false;

            WingPlay = WingPlayOption.None;
            WingPlayRequired = false;

            ShootingFrequency = ShootingFrequencyOption.None;
            ShootingFrequencyRequired = false;

            DribblingFrequency = DribblingFrequencyOption.None;
            DribblingFrequencyRequired = false;

            CrossingFrequency = CrossingFrequencyOption.None;
            CrossingFrequencyRequired = false;

            CrossDistance = CrossDistanceOption.None;
            CrossDistanceRequired = false;

            CrossTarget = CrossTargetOption.None;
            CrossTargetRequired = false;

            PassingStyle = PassingStyleOption.None;
            PassingStyleRequired = false;

            CreativePassing = CreativePassingOption.None;
            CreativePassingRequired = false;
        }

        public void MakeAllUnavailable()
        {
            HoldUpBall = null;
            HoldUpBallRequired = false;

            WingPlay = null;
            WingPlayRequired = false;

            ShootingFrequency = null;
            ShootingFrequencyRequired = false;

            DribblingFrequency = null;
            DribblingFrequencyRequired = false;

            CrossingFrequency = null;
            CrossingFrequencyRequired = false;

            CrossDistance = null;
            CrossDistanceRequired = false;

            CrossTarget = null;
            CrossTargetRequired = false;

            PassingStyle = null;
            PassingStyleRequired = false;

            CreativePassing = null;
            CreativePassingRequired = false;
        }
    }
}