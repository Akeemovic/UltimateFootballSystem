using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.TacticsEngine.Instructions.Individual;

namespace UltimateFootballSystem.Core.TacticsEngine.TacticalRoles
{
    /// <summary>
    /// Generic implementation of TacticalRole for prototyping
    /// </summary>
    public class GenericTacticalRole : TacticalRole
    {
        public GenericTacticalRole(
            TacticalRoleOption roleOption,
            string roleName,
            string roleNameShort,
            string roleDescription,
            List<TacticalPositionGroupOption> availablePositionGroups,
            List<TacticalDutyOption> availableDuties)
            : base(roleOption, roleName, roleNameShort, roleDescription, availablePositionGroups, availableDuties)
        {
        }

        public override Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption> GetAvailableZones(
            TacticalPositionOption position, TacticalDutyOption duty)
        {
            var result = new Dictionary<TacticalZoneOption, TacticalZoneAvailabilityOption>();

            // Simple zone mapping based on position
            var baseZones = GetBaseZonesForPosition(position);

            // Adjust availability based on duty
            foreach (var zone in baseZones)
            {
                var availability = TacticalZoneAvailabilityOption.Medium;

                // More forward duties increase forward zone availability
                if (duty == TacticalDutyOption.Attack)
                {
                    // For attack duty, increase availability in forward zones
                    if (zone > TacticalZoneOption.Z_20)
                    {
                        availability = TacticalZoneAvailabilityOption.High;
                    }
                }
                else if (duty == TacticalDutyOption.Defend)
                {
                    // For defend duty, decrease availability in forward zones
                    if (zone > TacticalZoneOption.Z_20)
                    {
                        availability = TacticalZoneAvailabilityOption.Low;
                    }
                }

                result[zone] = availability;
            }

            return result;
        }

        // Helper to determine base zones for a position
        private List<TacticalZoneOption> GetBaseZonesForPosition(TacticalPositionOption position)
        {
            var result = new List<TacticalZoneOption>();

            // Very simplified zone mapping
            switch (position)
            {
                case TacticalPositionOption.GK:
                    result.AddRange(new[] { TacticalZoneOption.Z_1, TacticalZoneOption.Z_2, TacticalZoneOption.Z_3 });
                    break;

                case TacticalPositionOption.DL:
                    result.AddRange(new[] { TacticalZoneOption.Z_4, TacticalZoneOption.Z_9, TacticalZoneOption.Z_14 });
                    break;

                case TacticalPositionOption.DCL:
                case TacticalPositionOption.DC:
                case TacticalPositionOption.DCR:
                    result.AddRange(new[] { TacticalZoneOption.Z_5, TacticalZoneOption.Z_6, TacticalZoneOption.Z_7 });
                    break;

                case TacticalPositionOption.DR:
                    result.AddRange(new[] { TacticalZoneOption.Z_8, TacticalZoneOption.Z_13, TacticalZoneOption.Z_18 });
                    break;

                case TacticalPositionOption.DMC:
                    result.AddRange(new[] { TacticalZoneOption.Z_10, TacticalZoneOption.Z_11, TacticalZoneOption.Z_12 });
                    break;

                case TacticalPositionOption.MCL:
                case TacticalPositionOption.MC:
                case TacticalPositionOption.MCR:
                    result.AddRange(new[] { TacticalZoneOption.Z_15, TacticalZoneOption.Z_16, TacticalZoneOption.Z_17 });
                    break;

                case TacticalPositionOption.ML:
                    result.AddRange(new[] { TacticalZoneOption.Z_14, TacticalZoneOption.Z_19, TacticalZoneOption.Z_24 });
                    break;

                case TacticalPositionOption.MR:
                    result.AddRange(new[] { TacticalZoneOption.Z_18, TacticalZoneOption.Z_23, TacticalZoneOption.Z_28 });
                    break;

                case TacticalPositionOption.AMC:
                    result.AddRange(new[] { TacticalZoneOption.Z_20, TacticalZoneOption.Z_21, TacticalZoneOption.Z_22 });
                    break;

                case TacticalPositionOption.AML:
                    result.AddRange(new[] { TacticalZoneOption.Z_19, TacticalZoneOption.Z_24, TacticalZoneOption.Z_29 });
                    break;

                case TacticalPositionOption.AMR:
                    result.AddRange(new[] { TacticalZoneOption.Z_23, TacticalZoneOption.Z_28, TacticalZoneOption.Z_33 });
                    break;

                case TacticalPositionOption.STC:
                case TacticalPositionOption.STCL:
                case TacticalPositionOption.STCR:
                    result.AddRange(new[] { TacticalZoneOption.Z_30, TacticalZoneOption.Z_31, TacticalZoneOption.Z_32 });
                    break;

                default:
                    // For any other position, just add a few generic zones
                    result.AddRange(new[] { TacticalZoneOption.Z_1, TacticalZoneOption.Z_2 });
                    break;
            }

            return result;
        }

        public override TacticalRole Clone()
        {
            var clone = new GenericTacticalRole(
                RoleOption,
                RoleName,
                RoleNameShort,
                RoleDescription,
                new List<TacticalPositionGroupOption>(AvailablePositionGroups),
                new List<TacticalDutyOption>(AvailableDuties)
            );

            clone.SetSelectedPosition(SelectedPosition);
            clone.SetSelectedDuty(SelectedDuty);

            return clone;
        }

        protected override IndividualInstruction CreateDefaultInstructionsForDuty(TacticalDutyOption duty)
        {
            // Use the role's position group to create appropriate instructions
            return new IndividualInstruction(AvailablePositionGroups.First());
        }
    }

    // Concrete role implementations for testing

    public class Goalkeeper : GenericTacticalRole
    {
        public Goalkeeper() : base(
            TacticalRoleOption.Goalkeeper,
            "Goalkeeper",
            "GK",
            "Standard goalkeeper position",
            new List<TacticalPositionGroupOption> { TacticalPositionGroupOption.GK },
            new List<TacticalDutyOption> { TacticalDutyOption.Defend, TacticalDutyOption.Support })
        {
        }
    }

    public class Winger : GenericTacticalRole
    {
        public Winger() : base(
            TacticalRoleOption.Winger,
            "Winger",
            "W",
            "Wide attacking player who hugs the touchline",
            new List<TacticalPositionGroupOption> { TacticalPositionGroupOption.M_Flank, TacticalPositionGroupOption.AM_Flank },
            new List<TacticalDutyOption> { TacticalDutyOption.Defend, TacticalDutyOption.Support, TacticalDutyOption.Attack })
        {
        }
    }

    public class BoxToBoxMidfielder : GenericTacticalRole
    {
        public BoxToBoxMidfielder() : base(
            TacticalRoleOption.BoxToBoxMidfielder,
            "Box-to-Box Midfielder",
            "BBM",
            "Midfielder who contributes in both attack and defense",
            new List<TacticalPositionGroupOption> { TacticalPositionGroupOption.M_Center },
            new List<TacticalDutyOption> { TacticalDutyOption.Support, TacticalDutyOption.Attack })
        {
        }
    }

    public class FalseNine : GenericTacticalRole
    {
        public FalseNine() : base(
            TacticalRoleOption.FalseNine,
            "False Nine",
            "F9",
            "Forward who drops into midfield to create space",
            new List<TacticalPositionGroupOption> { TacticalPositionGroupOption.ST_Center },
            new List<TacticalDutyOption> { TacticalDutyOption.Support, TacticalDutyOption.Attack })
        {
        }
    }

    public class FullBack : GenericTacticalRole
    {
        public FullBack() : base(
            TacticalRoleOption.FullBack,
            "Full Back",
            "FB",
            "Defensive wide player who supports both defense and attack",
            new List<TacticalPositionGroupOption> { TacticalPositionGroupOption.D_Flank },
            new List<TacticalDutyOption> { TacticalDutyOption.Defend, TacticalDutyOption.Support })
        {
        }
    }

    public class CentralMidfielder : GenericTacticalRole
    {
        public CentralMidfielder() : base(
            TacticalRoleOption.CentralMidfielder,
            "Central Midfielder",
            "CM",
            "Balanced midfielder responsible for both defense and attack",
            new List<TacticalPositionGroupOption> { TacticalPositionGroupOption.M_Center },
            new List<TacticalDutyOption> { TacticalDutyOption.Defend, TacticalDutyOption.Support, TacticalDutyOption.Attack })
        {
        }
    }
}