using System.Collections.Generic;

namespace UltimateFootballSystem.Core.Tactics.Instructions.Team.InPossession
{
    public class ChanceCreation
    {
        private readonly List<ChanceCreationOption> _choices;
        private readonly List<ChanceCreationOption> _unavailableChoices;

        public ChanceCreation()
        {
            _choices = new List<ChanceCreationOption>();
            _unavailableChoices = new List<ChanceCreationOption>();
        }

        public IReadOnlyCollection<ChanceCreationOption> Choices => _choices.AsReadOnly();

        public IReadOnlyCollection<ChanceCreationOption> UnavailableChoices => _unavailableChoices.AsReadOnly();

        /*
         *  Add
         */
        public void AddWorkIntoBox()
        {
            // Remove the Direct Opposite (Remove Everything Else)
            _choices.Clear();
            // Perform the Addition
            _choices.Add(ChanceCreationOption.WorkIntoBox);
            // Update Unavailable
            _unavailableChoices.Add(ChanceCreationOption.HitEarlyCrosses);
            _unavailableChoices.Add(ChanceCreationOption.ShootOnSight);
        }

        public void AddShootOnSight()
        {
            // Remove the Direct Opposite
            RemoveWorkIntoBox();
            // Perform the Addition
            _choices.Add(ChanceCreationOption.ShootOnSight);
            // Update Unavailable
            MakeWorkIntoBoxUnavailable();
        }

        public void AddHitEarlyCrosses()
        {
            // Remove the Direct Opposites
            RemoveWorkIntoBox();
            // Perfrom the Addition
            _choices.Add(ChanceCreationOption.HitEarlyCrosses);
            // Update Unavailable
            MakeWorkIntoBoxUnavailable();
        }

        /*
         *  Remove
         */
        public void RemoveWorkIntoBox()
        {
            _choices.Remove(ChanceCreationOption.WorkIntoBox);
            // Update Unavailable
            _unavailableChoices.Remove(ChanceCreationOption.HitEarlyCrosses);
            _unavailableChoices.Remove(ChanceCreationOption.ShootOnSight);
        }

        public void RemoveShootOnSight()
        {
            _choices.Remove(ChanceCreationOption.ShootOnSight);
            if (!_choices.Contains(ChanceCreationOption.HitEarlyCrosses))
                _unavailableChoices.Remove(ChanceCreationOption.WorkIntoBox);
        }

        public void RemoveHitEarlyCrosses()
        {
            _choices.Remove(ChanceCreationOption.HitEarlyCrosses);
            if (!_choices.Contains(ChanceCreationOption.ShootOnSight))
                _unavailableChoices.Remove(ChanceCreationOption.WorkIntoBox);
        }


        /*
         *  Utility Functions
         */
        private void MakeWorkIntoBoxUnavailable()
        {
            if (!_unavailableChoices.Contains(ChanceCreationOption.WorkIntoBox))
                _unavailableChoices.Add(ChanceCreationOption.WorkIntoBox);
        }
    }
}