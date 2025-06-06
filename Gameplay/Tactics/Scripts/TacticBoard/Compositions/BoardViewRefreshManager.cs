using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UltimateFootballSystem.Core.Entities;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class BoardViewRefreshManager
    {
        private readonly TacticsBoardController _controller;
        private readonly Queue<PlayerItemView> _reserveViewPool = new Queue<PlayerItemView>();
        private const int MaxPoolSize = 10;

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
                
                if (player != null)
                {
                    view.placeholderView.Hide();
                    view.mainView.Show();
                }
                else
                {
                    view.placeholderView.Show();
                    view.mainView.Hide();
                    view.placeholderView.UpdatePositionText();
                }
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
                    // Try to get from pool first
                    if (_reserveViewPool.Count > 0)
                    {
                        view = _reserveViewPool.Dequeue();
                        view.gameObject.SetActive(true);
                    }
                    else
                    {
                        // Create new view
                        GameObject playerItemViewObject = Object.Instantiate(_controller.playerItemViewPrefab.gameObject, _controller.reserveListSection.viewsContainer);
                        view = playerItemViewObject.GetComponent<PlayerItemView>();
                        
                        view.Controller = _controller;
                        view.ViewOwnerOption = PlayerItemViewOwnerOption.ReserveList;
                    }
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
                // Pool or destroy excess views
                var excessViews = viewsList.Skip(_controller.ReservePlayersItems.Count).ToList();
                _controller.StartCoroutine(PoolOrDestroyViewsAsync(excessViews));
                
                // Remove from list
                viewsList.RemoveRange(_controller.ReservePlayersItems.Count, viewsList.Count - _controller.ReservePlayersItems.Count);
            }
            
            // Update the array reference
            _controller.reservePlayersViews = viewsList.ToArray();
            
            // Update header count
            _controller.reserveListSection.UpdateFormattedHeaderText(_controller.ReservePlayersItems.Count.ToString());
        }

        private IEnumerator PoolOrDestroyViewsAsync(List<PlayerItemView> viewsToRemove)
        {
            foreach (var view in viewsToRemove)
            {
                if (view != null && view.gameObject != null)
                {
                    if (_reserveViewPool.Count < MaxPoolSize)
                    {
                        // Pool the view for reuse
                        view.gameObject.SetActive(false);
                        view.SetPlayerData(null); // Clear data
                        _reserveViewPool.Enqueue(view);
                    }
                    else
                    {
                        // Pool is full, destroy the view
                        Object.Destroy(view.gameObject);
                    }
                    yield return null; // Wait one frame between operations
                }
            }
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
            // Clean up view pool
            while (_reserveViewPool.Count > 0)
            {
                var view = _reserveViewPool.Dequeue();
                if (view != null && view.gameObject != null)
                {
                    Object.Destroy(view.gameObject);
                }
            }
        }
    }
} 