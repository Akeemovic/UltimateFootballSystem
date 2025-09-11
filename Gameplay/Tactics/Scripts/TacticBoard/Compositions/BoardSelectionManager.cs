using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.TacticsEngine;
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

        public enum AutoPickCriteria
        {
            StartingPlayerPositionMapping,
            PositionalCompatibility,
            CurrentAbility, 
            PotentialAbility,
            Condition,
            Morale,
            MatchFitness
        }

        
        // Convenience methods
        public void AutoPickStartingLineupFromMapping() => AutoPickStartingLineup(AutoPickCriteria.StartingPlayerPositionMapping);
        public void AutoPickAllSelections(AutoPickCriteria criteria) => AutoPick(criteria);
        public void AutoPickStartingLineup(AutoPickCriteria criteria) => AutoPick(criteria, includeStarters: true);
        public void AutoPickSubstitutes(AutoPickCriteria criteria) => AutoPick(criteria, includeSubs: true);
        
        private void AutoPick(AutoPickCriteria criteria, bool includeStarters = false, bool includeSubs = false)
        {
            /*
             * if includeStarters then move all starting players items to reserve, auto pick by criteria,
             * then add the remaining to reserves, the init reserves - same for includeSubs
            */

            
            // Clear all
            // TODO: Implement autopick logic based on criteria
            switch (criteria)
            {
                case AutoPickCriteria.StartingPlayerPositionMapping:
                    FillPlayers(_controller.StartingPositionPlayerMapping);
                    break;
                case AutoPickCriteria.PositionalCompatibility:
                    // Sort Positional Compatibility and pick best players
                    break;
                case AutoPickCriteria.CurrentAbility:
                    // Sort by current ability and pick best players
                    break;
                case AutoPickCriteria.PotentialAbility:
                    // Sort by potential ability and pick best players
                    break;
                case AutoPickCriteria.Condition:
                    // Sort by condition and pick best players
                    break;
                case AutoPickCriteria.Morale:
                    // Sort by morale and pick best players
                    break;
                case AutoPickCriteria.MatchFitness:
                    // Sort by match fitness and pick best players
                    break;
            }
        }

        private void ClearSelection(bool clearStarting, bool clearSubs = false)
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
                //
                // Always refresh reserves since we added players
                // _viewRefreshManager.RefreshReserveViews(_controller.autoSortReserves); // Sort if auto-sort is enabled
                _controller.BoardInitializationManager.InitializeReservePlayers();
            }
        }

        // Convenience methods
        public void ClearAllSelections() => ClearSelection(true, true);
        public void ClearStartingLineup() => ClearSelection(true, false);
        public void ClearSubstitutes() => ClearSelection(false, true);
        
        private void FillPlayers(Dictionary<TacticalPositionOption, Player?> startingPositionPlayersMapping)
        {
            Debug.Log($"FillPlayers(Dictionary) called with {startingPositionPlayersMapping?.Count ?? 0} mappings");
            
            if (startingPositionPlayersMapping == null)
            {
                Debug.LogError("FillPlayers: startingPositionPlayersMapping is null");
                return;
            }
            
            if (_controller.zoneContainerViews == null)
            {
                Debug.LogError("FillPlayers: zoneContainerViews is null");
                return;
            }
            
            // Loop through each container of zone views on the board
            foreach (var zoneContainer in _controller.zoneContainerViews)
            {
                if (zoneContainer == null)
                {
                    Debug.LogWarning("FillPlayers: zoneContainer is null, skipping");
                    continue;
                }
                
                // Get all zone views (positions) within the current container
                var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);
                Debug.Log($"Found {zoneViews?.Length ?? 0} zone views in container {zoneContainer.name}");
                
                // Loop through each zone to clear old data and set new data
                foreach (var zoneView in zoneViews)
                {
                    if (zoneView == null || zoneView.childPlayerItemView == null)
                    {
                        Debug.LogWarning("FillPlayers: zoneView or childPlayerItemView is null, skipping");
                        continue;
                    }
                    
                    // Check if the zone is in use for the current formation
                    if (zoneView.InUseForFormation)
                    {
                        Debug.Log($"Processing IN-USE zone {zoneView.name} with position {zoneView.tacticalPositionOption}");
                        
                        // Find player for this position, set whatever is returned (even nulls)
                        if (startingPositionPlayersMapping.TryGetValue(zoneView.tacticalPositionOption, out var player))
                        {
                            Debug.Log($"Found mapping: {zoneView.tacticalPositionOption} -> {player?.Name ?? "null"}");
                            zoneView.childPlayerItemView.SetPlayerData(player);
                        }
                        else
                        {
                            Debug.LogWarning($"No mapping found for position {zoneView.tacticalPositionOption}, setting to null");
                            zoneView.childPlayerItemView.SetPlayerData(null);
                        }
                    }
                    else
                    {
                        // Clear zones that are NOT in use for current formation
                        Debug.Log($"Clearing NOT-IN-USE zone {zoneView.name}");
                        zoneView.childPlayerItemView.SetPlayerData(null);
                    }
                }
            }
        }
        
        public void FillPlayers(IEnumerable<Player?> players)
        {
            Debug.Log("LoadPlayersAutofill method called.");
    
            int playerIndex = 0;
    
            // Loop through each container of zone views on the board
            foreach (var zoneContainer in _controller.zoneContainerViews)
            {
                var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);
                // Loop through each zone tacticsPitch and assign players or set to null if not in use
                foreach (var zoneView in zoneViews)
                {
                    if (zoneView.InUseForFormation && playerIndex < players.Count())
                    {
                        var player = players.ElementAt(playerIndex);
                        
                        if (player != null)
                        {
                            Debug.Log($"LoadPlayersAutofill: Setting player {player.Name} in zone tacticsPitch {zoneView.name}.");
                            zoneView.childPlayerItemView.SetPlayerData(player);
                        }
                        else
                        {
                            Debug.Log($"LoadPlayersAutofill: No player to assign to zone tacticsPitch {zoneView.name}, setting data to null.");
                            zoneView.childPlayerItemView.SetPlayerData(null);
                        }

                        playerIndex++;
                    }
                    else
                    {
                        Debug.Log($"LoadPlayersAutofill: Zone tacticsPitch {zoneView.name} is not in use for formation, setting data to null.");
                        zoneView.childPlayerItemView.SetPlayerData(null);
                    }
                }
            }
            Debug.Log("LoadPlayersAutofill method completed.");
        }
    }
} 