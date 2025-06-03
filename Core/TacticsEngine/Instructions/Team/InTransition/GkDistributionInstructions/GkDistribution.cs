using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimateFootballSystem.Core.TacticsEngine.Instructions.Team.InTransition.GkDistributionInstructions
{
    public class GkDistribution
    {
        private List<GkDistributionAreaOption> _areas;
        private GkDistributionTypeOption? _type;
        private List<GkDistributionAreaOption> _unavailableAreas;
        private List<GkDistributionTypeOption> _unavailableTypes;

        public GkDistribution()
        {
            ZoneType = GkDistributionZoneTypeOption.AreaPlayer;
            // Areas
            _areas = new List<GkDistributionAreaOption>();
            _unavailableAreas = new List<GkDistributionAreaOption>();
            // Type
            _type = null;
            _unavailableTypes = new List<GkDistributionTypeOption>();
        }

        public GkDistributionZoneTypeOption ZoneType { get; set; }

        public IReadOnlyList<GkDistributionAreaOption> Areas => _areas.AsReadOnly();

        /*
        private List<GkDistributionAreaOption> _areas;
        public IReadOnlyCollection<GkDistributionAreaOption> GetGkDistributionAreasList()
        {
            return _areas.AsReadOnly();
        } */

        public GkDistributionTypeOption? Type
        {
            set
            {
                switch (value)
                {
                    case GkDistributionTypeOption.RollItOut:
                        // Areas
                        MakeAreaUnavailable(GkDistributionAreaOption.ToFlanks);
                        MakeAreaUnavailable(GkDistributionAreaOption.ToTargetMan);
                        MakeAreaUnavailable(GkDistributionAreaOption.ToOppositionDefense);
                        break;
                    case GkDistributionTypeOption.ThrowItLong:
                        // Areas
                        MakeAreaUnavailable(GkDistributionAreaOption.ToOppositionDefense);
                        MakeAreaUnavailable(GkDistributionAreaOption.ToTargetMan);
                        break;
                    case GkDistributionTypeOption.TakeShortKick:
                        // Areas
                        MakeAreaUnavailable(GkDistributionAreaOption.ToOppositionDefense);
                        MakeAreaUnavailable(GkDistributionAreaOption.ToTargetMan);
                        break;
                    case GkDistributionTypeOption.TakeLongKicks:
                        // Areas
                        MakeAreaUnavailable(GkDistributionAreaOption.ToCentreBacks);
                        MakeAreaUnavailable(GkDistributionAreaOption.ToFullBacks);
                        break;
                }

                _unavailableTypes.Clear();

                if (value != null)
                {
                    // To Address nullable compiler issues || Doesn't really do anything 
                    var nonNullableValue = value ?? GkDistributionTypeOption.RollItOut;
                    _unavailableTypes = GetAllTypesExcept(nonNullableValue).ToList();
                }

                _type = value;
            }
            get => _type;
        }

        // Unavailable Option
        public IReadOnlyList<GkDistributionAreaOption> UnavailableAreas => _unavailableAreas.AsReadOnly();

        public IReadOnlyList<GkDistributionTypeOption> UnavailableTypes => _unavailableTypes.AsReadOnly();

        /// <summary>
        ///     Add the specified area to the Distribution Areas list
        /// </summary>
        /// <param name="area"></param>
        public void AddArea(GkDistributionAreaOption area)
        {
            switch (area)
            {
                case GkDistributionAreaOption.ToCentreBacks:
                    AddDistributeToCentreBacks();
                    break;
                case GkDistributionAreaOption.ToFullBacks:
                    AddDistributeToFullBacks();
                    break;
                case GkDistributionAreaOption.ToPlaymakers:
                    AddDistributeToPlaymakers();
                    break;
                case GkDistributionAreaOption.ToFlanks:
                    AddDistributeToFlanks();
                    break;
                case GkDistributionAreaOption.ToTargetMan:
                    AddDistributeToTargetMan();
                    break;
                case GkDistributionAreaOption.ToOppositionDefense:
                    AddDistributeToOppositionDefense();
                    break;
            }
        }

        /// <summary>
        ///     Removes the specified area from the Distribution Areas list
        /// </summary>
        /// <param name="area"></param>
        public void RemoveArea(GkDistributionAreaOption area)
        {
            switch (area)
            {
                case GkDistributionAreaOption.ToCentreBacks:
                    RemoveDistributeToCentreBacks();
                    break;
                case GkDistributionAreaOption.ToFullBacks:
                    RemoveDistributeToFullBacks();
                    break;
                case GkDistributionAreaOption.ToPlaymakers:
                    RemoveDistributeToPlaymakers();
                    break;
                case GkDistributionAreaOption.ToFlanks:
                    RemoveDistributeToFlanks();
                    break;
                case GkDistributionAreaOption.ToTargetMan:
                    RemoveDistributeToTargetMan();
                    break;
                case GkDistributionAreaOption.ToOppositionDefense:
                    RemoveDistributeToOppositionDefense();
                    break;
            }
        }

        /// <summary>
        ///     Set specified type as the Distribution Type or removes if if it already exists
        /// </summary>
        /// <param name="type"></param>
        public void ToggleType(GkDistributionTypeOption type)
        {
            // Nullify and return if it's already been set as value
            if (Type == type)
            {
                Type = null;
                return;
            }

            Type = type;
        }

        /*
        public IReadOnlyCollection<GkDistributionTypeOption> GetAllTypesExcept(GkDistributionTypeOption type)
        {
            var typesList = Enum.GetValues(typeof(GkDistributionTypeOption))
                .Cast<GkDistributionTypeOption>()
                .ToList();
            typesList.Remove(type);
            return typesList.AsReadOnly();
        } */
        public List<GkDistributionTypeOption> GetAllTypesExcept(GkDistributionTypeOption type)
        {
            var typesList = Enum.GetValues(typeof(GkDistributionTypeOption))
                .Cast<GkDistributionTypeOption>()
                .ToList();
            typesList.Remove(type);
            return typesList;
        }

        #region ADD AREA METHODS

        private void AddDistributeToCentreBacks()
        {
            // Remove opposing distribution type
            if (_type == GkDistributionTypeOption.TakeLongKicks) _type = null;
            MakeTypeUnavailable(GkDistributionTypeOption.TakeLongKicks);
            // Add Area
            // To CB  & To FB can co-exist
            if (_areas.Contains(GkDistributionAreaOption.ToFullBacks))
                _areas.Add(GkDistributionAreaOption.ToCentreBacks);
            else
                _areas = new List<GkDistributionAreaOption> { GkDistributionAreaOption.ToCentreBacks };
            // Update Unavailable area options
            _unavailableAreas = new List<GkDistributionAreaOption>
            {
                GkDistributionAreaOption.ToTargetMan,
                GkDistributionAreaOption.ToFlanks,
                GkDistributionAreaOption.ToPlaymakers,
                GkDistributionAreaOption.ToOppositionDefense
            };
        }

        private void AddDistributeToFullBacks()
        {
            // Remove opposing distribution type
            if (_type == GkDistributionTypeOption.TakeLongKicks) _type = null;
            MakeTypeUnavailable(GkDistributionTypeOption.TakeLongKicks);
            // Add
            // To CB  & To FB can co-exist
            if (_areas.Contains(GkDistributionAreaOption.ToCentreBacks))
                _areas.Add(GkDistributionAreaOption.ToFullBacks);
            else
                _areas = new List<GkDistributionAreaOption> { GkDistributionAreaOption.ToFullBacks };
            // Update Unavailable area options
            _unavailableAreas = new List<GkDistributionAreaOption>
            {
                GkDistributionAreaOption.ToTargetMan,
                GkDistributionAreaOption.ToFlanks,
                GkDistributionAreaOption.ToPlaymakers,
                GkDistributionAreaOption.ToOppositionDefense
            };
        }

        private void AddDistributeToPlaymakers()
        {
            // Add
            _areas = new List<GkDistributionAreaOption> { GkDistributionAreaOption.ToPlaymakers };
            // Update Unavailable area options
            _unavailableAreas = new List<GkDistributionAreaOption>
            {
                GkDistributionAreaOption.ToTargetMan,
                GkDistributionAreaOption.ToFlanks,
                GkDistributionAreaOption.ToFullBacks,
                GkDistributionAreaOption.ToCentreBacks,
                GkDistributionAreaOption.ToOppositionDefense
            };
        }

        private void AddDistributeToTargetMan()
        {
            // Remove opposing distribution type
            if (_type == GkDistributionTypeOption.RollItOut ||
                _type == GkDistributionTypeOption.ThrowItLong ||
                _type == GkDistributionTypeOption.TakeShortKick)
                _type = null;
            // Make them Unavailable
            MakeTypeUnavailable(GkDistributionTypeOption.RollItOut);
            MakeTypeUnavailable(GkDistributionTypeOption.ThrowItLong);
            MakeTypeUnavailable(GkDistributionTypeOption.TakeShortKick);
            // Add
            _areas = new List<GkDistributionAreaOption> { GkDistributionAreaOption.ToTargetMan };
            // Update Unavailable area options
            _unavailableAreas = new List<GkDistributionAreaOption>
            {
                GkDistributionAreaOption.ToFlanks,
                GkDistributionAreaOption.ToFullBacks,
                GkDistributionAreaOption.ToCentreBacks,
                GkDistributionAreaOption.ToOppositionDefense,
                GkDistributionAreaOption.ToPlaymakers
            };
        }

        private void AddDistributeToFlanks()
        {
            // Remove opposing distribution type
            if (_type == GkDistributionTypeOption.RollItOut) _type = null;
            MakeTypeUnavailable(GkDistributionTypeOption.RollItOut);
            // Add
            _areas = new List<GkDistributionAreaOption> { GkDistributionAreaOption.ToFlanks };
            // Update Unavailable area options
            _unavailableAreas = new List<GkDistributionAreaOption>
            {
                GkDistributionAreaOption.ToFullBacks,
                GkDistributionAreaOption.ToCentreBacks,
                GkDistributionAreaOption.ToOppositionDefense,
                GkDistributionAreaOption.ToPlaymakers,
                GkDistributionAreaOption.ToTargetMan
            };
        }

        private void AddDistributeToOppositionDefense()
        {
            // Remove opposing distribution type
            if (_type == GkDistributionTypeOption.RollItOut ||
                _type == GkDistributionTypeOption.ThrowItLong ||
                _type == GkDistributionTypeOption.TakeShortKick)
                _type = null;
            // Make them Unavailable
            _unavailableTypes =
                new List<GkDistributionTypeOption>
                {
                    GkDistributionTypeOption.RollItOut,
                    GkDistributionTypeOption.ThrowItLong,
                    GkDistributionTypeOption.TakeShortKick
                };
            // Make areas unavailable
            _unavailableAreas = new List<GkDistributionAreaOption>
            {
                GkDistributionAreaOption.ToFlanks,
                GkDistributionAreaOption.ToFullBacks,
                GkDistributionAreaOption.ToCentreBacks,
                GkDistributionAreaOption.ToPlaymakers,
                GkDistributionAreaOption.ToTargetMan
            };

            // Add
            _areas = new List<GkDistributionAreaOption> { GkDistributionAreaOption.ToOppositionDefense };
        }

        #endregion

        #region REMOVE AREA METHODS

        private void RemoveDistributeToCentreBacks()
        {
            _areas.Remove(GkDistributionAreaOption.ToCentreBacks);

            // Make opposite areas available
            if (!_areas.Contains(GkDistributionAreaOption.ToFullBacks))
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToFlanks);

                if (_type != GkDistributionTypeOption.ThrowItLong &&
                    _type != GkDistributionTypeOption.TakeShortKick)
                {
                    MakeAreaAvailable(GkDistributionAreaOption.ToTargetMan);
                    MakeAreaAvailable(GkDistributionAreaOption.ToOppositionDefense);
                }
            }
            else if (!_areas.Contains(GkDistributionAreaOption.ToFlanks))
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToFullBacks);

                if (_type != GkDistributionTypeOption.ThrowItLong &&
                    _type != GkDistributionTypeOption.TakeShortKick)
                {
                    MakeAreaAvailable(GkDistributionAreaOption.ToTargetMan);
                    MakeAreaAvailable(GkDistributionAreaOption.ToOppositionDefense);
                }
            }

            // Make opposite types available
            if (!_areas.Contains(GkDistributionAreaOption.ToFlanks) &&
                !_areas.Contains(GkDistributionAreaOption.ToFullBacks))
            {
                if (_type == GkDistributionTypeOption.RollItOut)
                {
                    MakeTypeAvailable(GkDistributionTypeOption.ThrowItLong);
                    MakeTypeAvailable(GkDistributionTypeOption.TakeShortKick);
                }
                else if (_type == GkDistributionTypeOption.ThrowItLong)
                {
                    MakeTypeAvailable(GkDistributionTypeOption.TakeShortKick);
                    MakeTypeAvailable(GkDistributionTypeOption.TakeLongKicks);
                }
                else if (_type == GkDistributionTypeOption.TakeShortKick)
                {
                    MakeTypeAvailable(GkDistributionTypeOption.ThrowItLong);
                    MakeTypeAvailable(GkDistributionTypeOption.TakeLongKicks);
                }
            }
        }

        private void RemoveDistributeToFullBacks()
        {
            _areas.Remove(GkDistributionAreaOption.ToFullBacks);

            // Make opposite areas available
            if (!_areas.Contains(GkDistributionAreaOption.ToCentreBacks))
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToPlaymakers);
                MakeAreaAvailable(GkDistributionAreaOption.ToTargetMan);

                if (_type != GkDistributionTypeOption.RollItOut)
                {
                    MakeAreaAvailable(GkDistributionAreaOption.ToFlanks);
                    MakeAreaAvailable(GkDistributionAreaOption.ToOppositionDefense);
                }
            }
            else if (!_areas.Contains(GkDistributionAreaOption.ToFlanks))
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToCentreBacks);

                if (_type != GkDistributionTypeOption.ThrowItLong &&
                    _type != GkDistributionTypeOption.TakeShortKick)
                {
                    MakeAreaAvailable(GkDistributionAreaOption.ToTargetMan);
                    MakeAreaAvailable(GkDistributionAreaOption.ToOppositionDefense);
                }
            }

            // Make opposite types available
            if (!_areas.Contains(GkDistributionAreaOption.ToFlanks) &&
                !_areas.Contains(GkDistributionAreaOption.ToCentreBacks))
            {
                if (_type == GkDistributionTypeOption.RollItOut)
                {
                    MakeTypeAvailable(GkDistributionTypeOption.ThrowItLong);
                    MakeTypeAvailable(GkDistributionTypeOption.TakeShortKick);
                }
                else if (_type == GkDistributionTypeOption.ThrowItLong)
                {
                    MakeTypeAvailable(GkDistributionTypeOption.TakeShortKick);
                    MakeTypeAvailable(GkDistributionTypeOption.TakeLongKicks);
                }
                else if (_type == GkDistributionTypeOption.TakeShortKick)
                {
                    MakeTypeAvailable(GkDistributionTypeOption.ThrowItLong);
                    MakeTypeAvailable(GkDistributionTypeOption.TakeLongKicks);
                }
            }
            else if (!_areas.Contains(GkDistributionAreaOption.ToCentreBacks))
            {
                MakeTypeAvailable(GkDistributionTypeOption.TakeLongKicks);
            }
        }

        private void RemoveDistributeToPlaymakers()
        {
            _areas.Remove(GkDistributionAreaOption.ToPlaymakers);
            // Areas
            if (_type != GkDistributionTypeOption.RollItOut)
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToFlanks);
                MakeAreaAvailable(GkDistributionAreaOption.ToTargetMan);
                MakeAreaAvailable(GkDistributionAreaOption.ToOppositionDefense);
            }

            if (_type != GkDistributionTypeOption.ThrowItLong &&
                _type != GkDistributionTypeOption.TakeShortKick)
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToTargetMan);
                MakeAreaAvailable(GkDistributionAreaOption.ToOppositionDefense);
            }

            if (_type != GkDistributionTypeOption.TakeLongKicks)
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToCentreBacks);
                MakeAreaAvailable(GkDistributionAreaOption.ToFullBacks);
            }
        }

        private void RemoveDistributeToTargetMan()
        {
            _areas.Remove(GkDistributionAreaOption.ToTargetMan);
            // Areas
            if (_type != GkDistributionTypeOption.TakeLongKicks)
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToCentreBacks);
                MakeAreaAvailable(GkDistributionAreaOption.ToFullBacks);
            }

            // Types
            MakeTypeAvailable(GkDistributionTypeOption.RollItOut);
            MakeTypeAvailable(GkDistributionTypeOption.ThrowItLong);
            MakeTypeAvailable(GkDistributionTypeOption.TakeShortKick);
        }

        private void RemoveDistributeToFlanks()
        {
            _areas.Remove(GkDistributionAreaOption.ToFlanks);
            // Areas
            MakeAreaAvailable(GkDistributionAreaOption.ToPlaymakers);
            if (_type != GkDistributionTypeOption.ThrowItLong ||
                _type != GkDistributionTypeOption.TakeShortKick)
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToTargetMan);
                MakeAreaAvailable(GkDistributionAreaOption.ToOppositionDefense);
            }

            if (_type != GkDistributionTypeOption.TakeLongKicks)
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToCentreBacks);
                MakeAreaAvailable(GkDistributionAreaOption.ToFullBacks);
            }

            // Types
            MakeTypeAvailable(GkDistributionTypeOption.RollItOut);
        }

        private void RemoveDistributeToOppositionDefense()
        {
            _areas.Remove(GkDistributionAreaOption.ToOppositionDefense);
            // Areas
            MakeAreaAvailable(GkDistributionAreaOption.ToPlaymakers);
            if (_type != GkDistributionTypeOption.ThrowItLong && _type != GkDistributionTypeOption.TakeShortKick)
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToTargetMan);
                MakeAreaAvailable(GkDistributionAreaOption.ToOppositionDefense);
            }

            if (_type != GkDistributionTypeOption.TakeLongKicks)
            {
                MakeAreaAvailable(GkDistributionAreaOption.ToCentreBacks);
                MakeAreaAvailable(GkDistributionAreaOption.ToFullBacks);
            }

            // Types
            MakeTypeAvailable(GkDistributionTypeOption.RollItOut);
        }

        #endregion

        #region UTILITY METHODS

        private void AddDistributionArea(GkDistributionAreaOption area)
        {
            if (IsAreaUnavailable(area)) MakeAreaAvailable(area);
            _areas.Add(area);
        }

        private void MakeAreaAvailable(GkDistributionAreaOption area)
        {
            _unavailableAreas.Remove(area);
        }

        private void MakeTypeAvailable(GkDistributionTypeOption type)
        {
            _unavailableTypes.Remove(type);
        }

        private void MakeAreaUnavailable(GkDistributionAreaOption area)
        {
            // Remove from selected
            if (IsAreaSelected(area)) RemoveArea(area);
            // Then...
            // Add to unavailable if Not already there
            if (!IsAreaUnavailable(area)) _unavailableAreas.Add(area);
        }

        private void MakeTypeUnavailable(GkDistributionTypeOption type)
        {
            // Make unavailavle if option is not already
            if (!IsTypeUnavailable(type)) _unavailableTypes.Add(type);
        }

        private bool IsAreaSelected(GkDistributionAreaOption area)
        {
            if (_areas.Contains(area)) return true;
            return false;
        }

        private bool IsAreaUnavailable(GkDistributionAreaOption area)
        {
            if (_unavailableAreas.Contains(area)) return true;
            return false;
        }

        private bool IsTypeUnavailable(GkDistributionTypeOption area)
        {
            if (_unavailableTypes.Contains(area)) return true;
            return false;
        }

        #endregion
    }
}