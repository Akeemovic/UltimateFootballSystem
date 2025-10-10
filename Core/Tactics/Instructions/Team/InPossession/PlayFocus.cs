using System.Collections.Generic;

namespace UltimateFootballSystem.Core.Tactics.Instructions.Team.InPossession
{
    public class PlayFocus
    {
        private readonly List<PlayFocusOption> _choices;
        private readonly List<PlayFocusOption> _unavailableChoices;

        public PlayFocus()
        {
            _choices = new List<PlayFocusOption>();
            _unavailableChoices = new List<PlayFocusOption>();
        }

        public IReadOnlyCollection<PlayFocusOption> Choices => _choices.AsReadOnly();

        public IReadOnlyCollection<PlayFocusOption> UnavailableChoices => _unavailableChoices.AsReadOnly();

        public void AddPlayOutOfDefense()
        {
            _choices.Add(PlayFocusOption.PlayOutOfDefense);
        }

        public void AddThroughTheMiddle()
        {
            // Remove the Direct Opposites
            _choices.Remove(PlayFocusOption.LeftFlank);
            _choices.Remove(PlayFocusOption.RightFlank);
            // Perfrom the Addition
            Add(PlayFocusOption.ThroughTheMiddle);
            // Update Unavailable
            MakeUnavailable(PlayFocusOption.LeftFlank);
            MakeUnavailable(PlayFocusOption.RightFlank);
        }

        public void AddLeftFlank()
        {
            // Remove the Direct Opposite
            _choices.Remove(PlayFocusOption.ThroughTheMiddle);
            // Perform the Addition
            Add(PlayFocusOption.LeftFlank);
            // Update Unavailable
            MakeAvailable(PlayFocusOption.RightFlank);
            MakeUnavailable(PlayFocusOption.ThroughTheMiddle);
        }

        public void AddRightFlank()
        {
            // Remove the Direct Opposite
            _choices.Remove(PlayFocusOption.ThroughTheMiddle);
            // Perform the Addition
            Add(PlayFocusOption.RightFlank);
            // Update Unavailable
            MakeAvailable(PlayFocusOption.LeftFlank);
            MakeUnavailable(PlayFocusOption.ThroughTheMiddle);
        }

        public void AddOverlapLeft()
        {
            // Remove the Direct Opposite
            _choices.Remove(PlayFocusOption.UnderlapLeft);
            // Perform the Addition
            Add(PlayFocusOption.OverlapLeft);
            // Update Unavailable
            MakeUnavailable(PlayFocusOption.UnderlapLeft);
        }

        public void AddUnderlapLeft()
        {
            // Remove the Direct Opposite
            _choices.Remove(PlayFocusOption.OverlapLeft);
            // Perform the Addition
            Add(PlayFocusOption.UnderlapLeft);
            // Update Unavailable
            MakeUnavailable(PlayFocusOption.OverlapLeft);
        }

        public void AddOverlapRight()
        {
            // Remove the Direct Opposite
            _choices.Remove(PlayFocusOption.UnderlapRight);
            // Perform the Addition
            Add(PlayFocusOption.OverlapRight);
            // Update Unavailable
            MakeUnavailable(PlayFocusOption.UnderlapRight);
        }

        public void AddUnderlapRight()
        {
            // Remove the Direct Opposite
            _choices.Remove(PlayFocusOption.OverlapRight);
            // Perform the Addition
            Add(PlayFocusOption.UnderlapRight);
            // Update Unavailable
            MakeUnavailable(PlayFocusOption.OverlapRight);
        }


        public void RemovePlayOutOfDefense()
        {
            _choices.Remove(PlayFocusOption.PlayOutOfDefense);
        }

        public void RemoveLeftFlank()
        {
            _choices.Remove(PlayFocusOption.LeftFlank);
            if (!_choices.Contains(PlayFocusOption.RightFlank))
                MakeAvailable(PlayFocusOption.ThroughTheMiddle);
            if (_unavailableChoices.Contains(PlayFocusOption.RightFlank))
                MakeAvailable(PlayFocusOption.RightFlank);
        }

        public void RemoveRightFlank()
        {
            _choices.Remove(PlayFocusOption.RightFlank);
            if (!_choices.Contains(PlayFocusOption.LeftFlank))
                MakeAvailable(PlayFocusOption.ThroughTheMiddle);
            if (_unavailableChoices.Contains(PlayFocusOption.LeftFlank))
                MakeAvailable(PlayFocusOption.LeftFlank);
        }

        public void RemoveThroughTheMiddle()
        {
            _choices.Remove(PlayFocusOption.ThroughTheMiddle);
            MakeAvailable(PlayFocusOption.LeftFlank);
            MakeAvailable(PlayFocusOption.RightFlank);
        }

        public void RemoveOverlapLeft()
        {
            _choices.Remove(PlayFocusOption.OverlapLeft);
            MakeAvailable(PlayFocusOption.UnderlapLeft);
        }

        public void RemoveUnderlapLeft()
        {
            _choices.Remove(PlayFocusOption.UnderlapLeft);
            MakeAvailable(PlayFocusOption.OverlapLeft);
        }

        public void RemoveOverlapRight()
        {
            _choices.Remove(PlayFocusOption.OverlapRight);
            MakeAvailable(PlayFocusOption.UnderlapRight);
        }

        public void RemoveUnderlapRight()
        {
            _choices.Remove(PlayFocusOption.UnderlapRight);
            MakeAvailable(PlayFocusOption.OverlapRight);
        }

        /*
         *  Utility Functions
         */
        private void Add(PlayFocusOption option)
        {
            if (IsUnavailable(option)) MakeAvailable(option);
            _choices.Add(option);
        }

        private void MakeAvailable(PlayFocusOption option)
        {
            _unavailableChoices.Remove(option);
        }

        //private void MakeAvailable(PlayFocusOption[] options) => _unavailableChoices.RemoveAll();
        private void MakeUnavailable(PlayFocusOption option)
        {
            // Make unavailavle if option is not already
            if (!IsUnavailable(option)) _unavailableChoices.Add(option);
        }

        private bool IsUnavailable(PlayFocusOption option)
        {
            if (_unavailableChoices.Contains(option)) return true;
            return false;
        }
    }
}