using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.Entities;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class BoardSelectionManager
    {
        private readonly TacticsBoardController _controller;
        private readonly BoardViewRefreshManager _viewRefreshManager;

        public BoardSelectionManager(TacticsBoardController controller, BoardViewRefreshManager viewRefreshManager)
        {
            _controller = controller;
            _viewRefreshManager = viewRefreshManager;
        }

        public enum AutopickCriteria
        {
            PositionalCompatibility,
            CurrentAbility, 
            PotentialAbility,
            Condition,
            Morale,
            MatchFitness
        }

        public void Autopick(AutopickCriteria criteria)
        {
            // TODO: Implement autopick logic based on criteria
            switch (criteria)
            {
                case AutopickCriteria.PositionalCompatibility:
                    // Sort Positional Compatibility and pick best players
                    break;
                case AutopickCriteria.CurrentAbility:
                    // Sort by current ability and pick best players
                    break;
                case AutopickCriteria.PotentialAbility:
                    // Sort by potential ability and pick best players
                    break;
                case AutopickCriteria.Condition:
                    // Sort by condition and pick best players
                    break;
                case AutopickCriteria.Morale:
                    // Sort by morale and pick best players
                    break;
                case AutopickCriteria.MatchFitness:
                    // Sort by match fitness and pick best players
                    break;
            }
        }

        public void ClearSelection(bool clearStarting, bool clearSubs = false)
        {
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                using var _ = _controller.SubstitutesPlayersItems.BeginUpdate();
                using var __ = _controller.ReservePlayersItems.BeginUpdate();
                
                var playersToMoveToReserves = new List<Core.Entities.Player>();
                
                // Clear starting players if requested
                if (clearStarting)
                {
                    foreach (var view in _controller.startingPlayersViews)
                    {
                        if (view != null && view.HasPlayerItem && view.Profile != null)
                        {
                            // Add to list to move to reserves
                            playersToMoveToReserves.Add(view.Profile);
                            
                            // Clear the view
                            view.SetPlayerData(null);
                        }
                    }
                    
                    // Also update the StartingPositionPlayerMapping
                    var keys = _controller.StartingPositionPlayerMapping.Keys.ToList();
                    foreach (var key in keys)
                    {
                        _controller.StartingPositionPlayerMapping[key] = null;
                    }
                }
                
                // Clear substitute players if requested
                if (clearSubs)
                {
                    // Collect non-null substitute players
                    var subsToMove = _controller.SubstitutesPlayersItems.Where(p => p != null).ToList();
                    playersToMoveToReserves.AddRange(subsToMove);
                    
                    // Clear substitutes list and fill with nulls
                    _controller.SubstitutesPlayersItems.Clear();
                    for (int i = 0; i < _controller.allowedSubstitutes; i++)
                    {
                        _controller.SubstitutesPlayersItems.Add(null);
                    }
                }
                
                // Add all cleared players to reserves (avoiding duplicates)
                foreach (var player in playersToMoveToReserves)
                {
                    if (!_controller.ReservePlayersItems.Contains(player))
                    {
                        _controller.ReservePlayersItems.Add(player);
                    }
                }
                
                _controller.SubstitutesPlayersItems.EndUpdate();
                _controller.ReservePlayersItems.EndUpdate();
                
                // Refresh affected views
                if (clearSubs)
                {
                    _viewRefreshManager.RefreshSubstituteViews();
                }
                
                // Always refresh reserves since we added players
                // _viewRefreshManager.RefreshReserveViews(_controller.autoSortReserves); // Sort if auto-sort is enabled
                _controller.BoardInitializationManager.InitializeReservePlayers();
            }
        }

        // Convenience methods
        public void ClearAllSelections() => ClearSelection(true, true);
        public void ClearStartingLineup() => ClearSelection(true, false);
        public void ClearSubstitutes() => ClearSelection(false, true);
    }
} 