using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UltimateFootballSystem.Core.Entities;
using UnityEngine;
using Lean.Pool;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class BoardViewRefreshManager
    {
        private readonly TacticsBoardController _controller;

        public BoardViewRefreshManager(TacticsBoardController controller, BoardInitializationManager boardInitializationManager)
        {
            _controller = controller;
        }

        public void RefreshSubstituteViews(bool forceSort = false)
        {
            // Sort if requested and auto-sort is enabled
            if (forceSort && _controller.autoSortSubstitutes)
            {
                SortSubstitutes();
            }
            
            // Ensure we have the correct number of views
            if (_controller.substitutesPlayersViews == null || _controller.substitutesPlayersViews.Length != _controller.allowedSubstitutes)
            {
                // Only recreate if array size is wrong
                _controller.BoardInitializationManager.InitializeSubstitutePlayers();
                return;
            }
            
            // Update existing views without destroying them
            for (int i = 0; i < _controller.substitutesPlayersViews.Length; i++)
            {
                var view = _controller.substitutesPlayersViews[i];
                if (view == null) continue;
                
                Core.Entities.Player player = null;
                if (i < _controller.SubstitutesPlayersItems.Count)
                {
                    player = _controller.SubstitutesPlayersItems[i];
                }
                
                // Update the view with player data (or null)
                view.SetPlayerData(player);
                view.BenchPlayersListIndex = i; // Update index
                
                // if (player != null)
                // {
                //     view.placeholderView.Hide();
                //     view.mainView.Show();
                // }
                // else
                // {
                //     view.placeholderView.Show();
                //     view.mainView.Hide();
                //     view.placeholderView.UpdatePositionText();
                // }
            }
            
            // Update header count (only count non-null players)
            _controller.substitutesListSection.UpdateFormattedHeaderText(
                _controller.SubstitutesPlayersItems.Count(p => p != null).ToString()
            );
        }

        // DO NOT USE: State inconsistent. View shows more or less player than expected in reserves
        public void RefreshReserveViews(bool forceSort = false)
        {
            // TODO: Implement better state management 
            // Remove any null entries from reserves (they shouldn't exist)
            var validReserves = _controller.ReservePlayersItems.Where(p => p != null).ToList();
            
            // Sort if requested and auto-sort is enabled
            if (forceSort && _controller.autoSortReserves)
            {
                validReserves = validReserves
                    .OrderBy(p => p.CurrentAbility) // Or your preferred sorting criteria
                    .ThenBy(p => p.Name)
                    .ToList();
            }
            
            // Update the list only if there are changes
            if (!validReserves.SequenceEqual(_controller.ReservePlayersItems))
            {
                _controller.ReservePlayersItems.Clear();
                foreach (var player in validReserves)
                {
                    _controller.ReservePlayersItems.Add(player);
                }
            }
            
            // Initialize views array if null
            if (_controller.reservePlayersViews == null)
            {
                _controller.reservePlayersViews = new PlayerItemView[0];
            }
            
            // Convert to list for easier manipulation
            var viewsList = _controller.reservePlayersViews.ToList();
            
            // Update existing views from top
            int viewIndex = 0;
            for (int i = 0; i < _controller.ReservePlayersItems.Count; i++)
            {
                PlayerItemView view = null;
                
                // Try to reuse existing view
                if (viewIndex < viewsList.Count)
                {
                    view = viewsList[viewIndex];
                    viewIndex++;
                }
                else
                {
                    // Create new view using LeanPool
                    view = PlayerItemViewPoolManager.SpawnPlayerItemView(_controller.playerItemViewPrefab.GetComponent<PlayerItemView>(), _controller.reserveListSection.viewsContainer);
                    
                    view.Controller = _controller;
                    view.ViewOwnerOption = PlayerItemViewOwnerOption.ReserveList;
                    viewsList.Add(view);
                }
                
                // Update view data
                view.SetPlayerData(_controller.ReservePlayersItems[i]);
                view.ReservePlayersListIndex = i;
                view.gameObject.SetActive(true);
                
                // Ensure proper ordering in hierarchy
                view.transform.SetSiblingIndex(i);
            }
            
            // Handle excess views
            if (viewsList.Count > _controller.ReservePlayersItems.Count)
            {
                // Despawn excess views using LeanPool
                var excessViews = viewsList.Skip(_controller.ReservePlayersItems.Count).ToList();
                foreach (var view in excessViews)
                {
                    if (view != null)
                    {
                        PlayerItemViewPoolManager.DespawnPlayerItemView(view);
                    }
                }
                
                // Remove from list
                viewsList.RemoveRange(_controller.ReservePlayersItems.Count, viewsList.Count - _controller.ReservePlayersItems.Count);
            }
            
            // Update the array reference
            _controller.reservePlayersViews = viewsList.ToArray();
            
            // Update header count
            _controller.reserveListSection.UpdateFormattedHeaderText(_controller.ReservePlayersItems.Count.ToString());
        }


        private void SortSubstitutes()
        {
            // Separate players and nulls
            var players = _controller.SubstitutesPlayersItems.Where(p => p != null).ToList();
            var nullCount = _controller.allowedSubstitutes - players.Count;
            
            // Sort players
            players = players
                .OrderBy(p => p.CurrentAbility)
                .ThenBy(p => p.Name)
                .ToList();
            
            // Clear and repopulate
            _controller.SubstitutesPlayersItems.Clear();
            foreach (var player in players)
            {
                _controller.SubstitutesPlayersItems.Add(player);
            }
            
            // Fill remaining slots with nulls
            for (int i = 0; i < nullCount; i++)
            {
                _controller.SubstitutesPlayersItems.Add(null);
            }
        }

        public void CompactSubstitutes()
        {
            var players = _controller.SubstitutesPlayersItems.Where(p => p != null).ToList();
            var nullCount = _controller.allowedSubstitutes - players.Count;
            
            _controller.SubstitutesPlayersItems.Clear();
            foreach (var player in players)
            {
                _controller.SubstitutesPlayersItems.Add(player);
            }
            
            for (int i = 0; i < nullCount; i++)
            {
                _controller.SubstitutesPlayersItems.Add(null);
            }
            
            RefreshSubstituteViews(false);
        }

        public void Cleanup()
        {
            // LeanPool handles cleanup automatically, no manual cleanup needed
        }
    }
} 