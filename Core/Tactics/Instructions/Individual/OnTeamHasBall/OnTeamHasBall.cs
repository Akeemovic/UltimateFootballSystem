using System;

namespace UltimateFootballSystem.Core.Tactics.Instructions.Individual
{
    public class OnTeamHasBall
    {
        public MoreForwardRunsOption? MoreForwardRuns { get; set; }
        public OpenChannelRunsOption? OpenChannelRuns { get; set; }
        public MobilityOption? Mobility { get; set; }
        public PositioningWidthOption? PositioningWidth { get; set; }

        public bool MoreForwardRunsRequired { get; private set; } = false;
        public bool OpenChannelRunsRequired { get; private set; } = false;
        public bool MobilityRequired { get; private set; } = false;
        public bool PositioningWidthRequired { get; private set; } = false;

        public OnTeamHasBall()
        {
            // All options are unavailable unless explicitly set, so null values indicate that
            MoreForwardRuns = null;
            OpenChannelRuns = null;
            Mobility = null;
            PositioningWidth = null;
        }

        public OnTeamHasBall(
            MoreForwardRunsOption? moreForwardRuns = null,
            OpenChannelRunsOption? openChannelRuns = null,
            MobilityOption? mobility = null,
            PositioningWidthOption? positioningWidth = null)
        {
            // Assign the values
            MoreForwardRuns = moreForwardRuns;
            OpenChannelRuns = openChannelRuns;
            Mobility = mobility;
            PositioningWidth = positioningWidth;

            // Set the required flags to true for each attribute
            if (moreForwardRuns != null) MoreForwardRunsRequired = true;
            if (openChannelRuns != null) OpenChannelRunsRequired = true;
            if (mobility != null) MobilityRequired = true;
            if (positioningWidth != null) PositioningWidthRequired = true;
        }

        public void SetRequiredDefault<T>(T option)
        {
            switch (option)
            {
                case MoreForwardRunsOption moreForwardRunsOption:
                    MoreForwardRuns = moreForwardRunsOption;
                    MoreForwardRunsRequired = true;
                    break;
                case OpenChannelRunsOption openChannelRunsOption:
                    OpenChannelRuns = openChannelRunsOption;
                    OpenChannelRunsRequired = true;
                    break;
                case MobilityOption mobilityOption:
                    Mobility = mobilityOption;
                    MobilityRequired = true;
                    break;
                case PositioningWidthOption positioningWidthOption:
                    PositioningWidth = positioningWidthOption;
                    PositioningWidthRequired = true;
                    break;
                default:
                    throw new ArgumentException("Invalid option type.");
            }
        }

        public void Set<T>(T option)
        {
            switch (option)
            {
                case MoreForwardRunsOption _ when MoreForwardRuns == null || MoreForwardRunsRequired:
                    throw new InvalidOperationException("MoreForwardRuns is not available.");
                case OpenChannelRunsOption _ when OpenChannelRuns == null || OpenChannelRunsRequired:
                    throw new InvalidOperationException("OpenChannelRuns is not available.");
                case MobilityOption _ when Mobility == null || MobilityRequired:
                    throw new InvalidOperationException("Mobility is not available.");
                case PositioningWidthOption _ when PositioningWidth == null || PositioningWidthRequired:
                    throw new InvalidOperationException("PositioningWidth is not available.");
            }
        }

        public void MakeAvailable<T>()
        {
            switch (typeof(T).Name)
            {
                case nameof(MoreForwardRuns):
                    MoreForwardRuns = MoreForwardRunsOption.None;
                    MoreForwardRunsRequired = false;
                    break;
                case nameof(OpenChannelRuns):
                    OpenChannelRuns = OpenChannelRunsOption.None;
                    OpenChannelRunsRequired = false;
                    break;
                case nameof(Mobility):
                    Mobility = MobilityOption.None;
                    MobilityRequired = false;
                    break;
                case nameof(PositioningWidth):
                    PositioningWidth = PositioningWidthOption.None;
                    PositioningWidthRequired = false;
                    break;
                default:
                    throw new ArgumentException("Invalid option type.");
            }
        }

        public void MakeUnavailable<T>()
        {
            switch (typeof(T).Name)
            {
                case nameof(MoreForwardRuns):
                    MoreForwardRuns = null;
                    MoreForwardRunsRequired = false;
                    break;
                case nameof(OpenChannelRuns):
                    OpenChannelRuns = null;
                    OpenChannelRunsRequired = false;
                    break;
                case nameof(Mobility):
                    Mobility = null;
                    MobilityRequired = false;
                    break;
                case nameof(PositioningWidth):
                    PositioningWidth = null;
                    PositioningWidthRequired = false;
                    break;
                default:
                    throw new ArgumentException("Invalid option type.");
            }
        }

        public void MakeAllAvailable()
        {
            MoreForwardRuns = MoreForwardRunsOption.None;
            MoreForwardRunsRequired = false;

            OpenChannelRuns = OpenChannelRunsOption.None;
            OpenChannelRunsRequired = false;

            Mobility = MobilityOption.None;
            MobilityRequired = false;

            PositioningWidth = PositioningWidthOption.None;
            PositioningWidthRequired = false;
        }

        public void MakeAllUnavailable()
        {
            MoreForwardRuns = null;
            MoreForwardRunsRequired = false;

            OpenChannelRuns = null;
            OpenChannelRunsRequired = false;

            Mobility = null;
            MobilityRequired = false;

            PositioningWidth = null;
            PositioningWidthRequired = false;
        }
    }
}