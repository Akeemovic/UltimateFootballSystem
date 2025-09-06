// using UltimateFootballSystem.Core.TacticsEngine;
// using UnityEngine;
// using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
//
// namespace UltimateFootballSystem.Gameplay.Tactics
// {
//     public class SelectionSwapManager
//     {
//         private readonly TacticsBoardController _controller;
//         private PlayerItemView _selectedItem;
//         private PlayerItemDragData _selectedData;
//
//         public bool HasSelection => _selectedItem != null && _selectedItem.HasPlayerItem;
//
//         public SelectionSwapManager(TacticsBoardController controller)
//         {
//             _controller = controller;
//         }
//
//         public void HandleItemClicked(PlayerItemView clickedItem)
//         {
//             if (clickedItem == null)
//             {
//                 Debug.LogWarning("Clicked item is null");
//                 return;
//             }
//
//             // Don't process null players unless they're in starting lineup
//             if (!clickedItem.HasPlayerItem && clickedItem.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList)
//             {
//                 ClearSelection();
//                 return;
//             }
//
//             // If we don't have a selection, select this item
//             if (_selectedItem == null)
//             {
//                 SelectItem(clickedItem);
//             }
//             // If we clicked the same item, deselect it
//             else if (_selectedItem == clickedItem)
//             {
//                 ClearSelection();
//             }
//             // Otherwise, perform the swap
//             else
//             {
//                 PerformSwap(clickedItem);
//             }
//         }
//
//         private void SelectItem(PlayerItemView item)
//         {
//             _selectedItem = item;
//             _selectedData = item.GetDragData();
//             _selectedData.SetDragSourceViewReference(item);
//             
//             // Add visual feedback
//             AddSelectionHighlight(item);
//             
//             Debug.Log($"Selected: {item.Profile?.Name ?? "Empty"} at position {item.TacticalPositionOption}");
//         }
//
//         private void PerformSwap(PlayerItemView targetItem)
//         {
//             if (targetItem == null || _selectedData == null)
//             {
//                 Debug.LogWarning("Cannot perform swap - target or selected data is null");
//                 ClearSelection();
//                 return;
//             }
//
//             var targetData = targetItem.GetDragData();
//             targetData.SetDropTargetViewReference(targetItem);
//
//             // Check if swap is valid (similar to drop validation)
//             if (!CanSwap(_selectedData, targetData))
//             {
//                 Debug.Log("Cannot swap these items");
//                 if (_controller.errorAudioClip != null)
//                 {
//                     _controller.PlayErrorSound();
//                 }
//                 return;
//             }
//
//             // Log before clearing (if needed for debugging)
//             Debug.Log($"Swapping: {_selectedData.Profile?.Name ?? "Empty"} with {targetData.Profile?.Name ?? "Empty"}");
//
//             // Store the data before clearing selection
//             var sourceData = _selectedData;
//             
//             // Clear selection to remove visual feedback
//             ClearSelection();
//
//             // Perform the swap with the stored data
//             _controller.BoardPlayerItemManager.SwapPlayersSelected(sourceData, targetData);
//             
//             // Play completion sound
//             _controller.PlayClickSound();
//         }
//
//         private bool CanSwap(PlayerItemDragData source, PlayerItemDragData target)
//         {
//             // Null checks
//             if (source == null || target == null)
//                 return false;
//
//             // Can't swap with self
//             if (source.DragSourceView == target.DropTargetView)
//                 return false;
//
//             // Additional validation for empty slots
//             bool sourceIsEmpty = !source.IsValidPlayer();
//             bool targetIsEmpty = !target.IsValidPlayer();
//
//             // Can't swap two empty slots
//             if (sourceIsEmpty && targetIsEmpty)
//                 return false;
//
//             // Handle empty slot swaps based on view owner
//             if (sourceIsEmpty || targetIsEmpty)
//             {
//                 // Check view owner compatibility
//                 var sourceOwner = source.DragSourceView.ViewOwnerOption;
//                 var targetOwner = target.DropTargetView.ViewOwnerOption;
//
//                 // Can't move empty slots between different lists (except starting list)
//                 if (sourceIsEmpty && sourceOwner != PlayerItemViewOwnerOption.StartingList)
//                     return false;
//                 
//                 if (targetIsEmpty)
//                 {
//                     // Can't drop on empty bench/reserve from a different list type
//                     if (targetOwner == PlayerItemViewOwnerOption.BenchList && 
//                         sourceOwner != PlayerItemViewOwnerOption.BenchList &&
//                         sourceOwner != PlayerItemViewOwnerOption.ReserveList)
//                         return false;
//                 }
//             }
//
//             // Can't remove GK from formation
//             if ((source.TacticalPositionOption == TacticalPositionOption.GK && targetIsEmpty) ||
//                 (target.TacticalPositionOption == TacticalPositionOption.GK && sourceIsEmpty))
//             {
//                 return false;
//             }
//
//             return true;
//         }
//
//         public void ClearSelection()
//         {
//             if (_selectedItem != null)
//             {
//                 RemoveSelectionHighlight(_selectedItem);
//                 _selectedItem = null;
//                 _selectedData = null;
//             }
//         }
//
//         private void AddSelectionHighlight(PlayerItemView item)
//         {
//             if (item == null || item.gameObject == null)
//                 return;
//
//             // Add visual highlight - you can customize this
//             var outline = item.GetComponent<UnityEngine.UI.Outline>();
//             if (outline == null)
//             {
//                 outline = item.gameObject.AddComponent<UnityEngine.UI.Outline>();
//             }
//             outline.effectColor = Color.yellow;
//             outline.effectDistance = new Vector2(3, 3);
//             outline.enabled = true;
//         }
//
//         private void RemoveSelectionHighlight(PlayerItemView item)
//         {
//             if (item == null || item.gameObject == null)
//                 return;
//
//             var outline = item.GetComponent<UnityEngine.UI.Outline>();
//             if (outline != null)
//             {
//                 outline.enabled = false;
//             }
//         }
//     }
// }

// using UltimateFootballSystem.Core.TacticsEngine;
// using UnityEngine;
// using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
// using System.Collections.Generic;
//
// namespace UltimateFootballSystem.Gameplay.Tactics
// {
//     public class SelectionSwapManager
//     {
//         private readonly TacticsBoardController _controller;
//         private PlayerItemView _selectedItem;
//         private PlayerItemDragData _selectedData;
//         private Dictionary<PlayerItemView, UnityEngine.UI.Outline> _activeOutlines = new Dictionary<PlayerItemView, UnityEngine.UI.Outline>();
//
//         public bool HasSelection => _selectedItem != null && _selectedItem.HasPlayerItem;
//
//         public SelectionSwapManager(TacticsBoardController controller)
//         {
//             _controller = controller;
//         }
//
//         public void HandleItemClicked(PlayerItemView clickedItem)
//         {
//             if (clickedItem == null)
//             {
//                 Debug.LogWarning("Clicked item is null");
//                 return;
//             }
//
//             // Don't process null players unless they're in starting lineup
//             if (!clickedItem.HasPlayerItem && clickedItem.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList)
//             {
//                 ClearSelection();
//                 return;
//             }
//
//             // If we don't have a selection, select this item
//             if (_selectedItem == null)
//             {
//                 SelectItem(clickedItem);
//             }
//             // If we clicked the same item, deselect it
//             else if (_selectedItem == clickedItem)
//             {
//                 ClearSelection();
//             }
//             // Otherwise, perform the swap
//             else
//             {
//                 PerformSwap(clickedItem);
//             }
//         }
//
//         private void SelectItem(PlayerItemView item)
//         {
//             _selectedItem = item;
//             _selectedData = item.GetDragData();
//             _selectedData.SetDragSourceViewReference(item);
//             
//             // Add visual feedback
//             AddSelectionHighlight(item);
//             
//             Debug.Log($"Selected: {item.Profile?.Name ?? "Empty"} at position {item.TacticalPositionOption}");
//         }
//
//         private void PerformSwap(PlayerItemView targetItem)
//         {
//             if (targetItem == null || _selectedData == null)
//             {
//                 Debug.LogWarning("Cannot perform swap - target or selected data is null");
//                 ClearSelection();
//                 return;
//             }
//
//             var targetData = targetItem.GetDragData();
//             targetData.SetDropTargetViewReference(targetItem);
//
//             // Check if swap is valid (similar to drop validation)
//             if (!CanSwap(_selectedData, targetData))
//             {
//                 Debug.Log("Cannot swap these items");
//                 if (_controller.errorAudioClip != null)
//                 {
//                     _controller.PlayErrorSound();
//                 }
//                 return;
//             }
//
//             // Log before clearing (if needed for debugging)
//             Debug.Log($"Swapping: {_selectedData.Profile?.Name ?? "Empty"} with {targetData.Profile?.Name ?? "Empty"}");
//
//             // Check if this involves reserves
//             bool involvesReserves = _selectedData.ReservePlayersListIndex >= 0 || targetData.ReservePlayersListIndex >= 0;
//             
//             // Store the data before clearing selection
//             var sourceData = _selectedData;
//             
//             // Clear selection to remove visual feedback
//             ClearSelection();
//
//             // If reserves are involved, we need to handle this differently
//             if (involvesReserves)
//             {
//                 // Delay the swap to next frame to avoid UI recreation issues
//                 _controller.StartCoroutine(DelayedSwap(sourceData, targetData));
//             }
//             else
//             {
//                 // Perform the swap immediately for non-reserve swaps
//                 _controller.BoardPlayerItemManager.SwapPlayersSelected(sourceData, targetData);
//                 _controller.PlayClickSound();
//             }
//         }
//
//         private System.Collections.IEnumerator DelayedSwap(PlayerItemDragData sourceData, PlayerItemDragData targetData)
//         {
//             // Wait one frame for UI to stabilize
//             yield return null;
//             
//             // Perform the swap
//             _controller.BoardPlayerItemManager.SwapPlayersSelected(sourceData, targetData);
//             
//             // Play completion sound
//             _controller.PlayClickSound();
//         }
//
//         private bool CanSwap(PlayerItemDragData source, PlayerItemDragData target)
//         {
//             // Null checks
//             if (source == null || target == null)
//                 return false;
//
//             // Can't swap with self
//             if (source.DragSourceView == target.DropTargetView)
//                 return false;
//
//             // Additional validation for empty slots
//             bool sourceIsEmpty = !source.IsValidPlayer();
//             bool targetIsEmpty = !target.IsValidPlayer();
//
//             // Can't swap two empty slots
//             if (sourceIsEmpty && targetIsEmpty)
//                 return false;
//
//             // Handle empty slot swaps based on view owner
//             if (sourceIsEmpty || targetIsEmpty)
//             {
//                 // Check view owner compatibility
//                 var sourceOwner = source.DragSourceView.ViewOwnerOption;
//                 var targetOwner = target.DropTargetView.ViewOwnerOption;
//
//                 // Can't move empty slots between different lists (except starting list)
//                 if (sourceIsEmpty && sourceOwner != PlayerItemViewOwnerOption.StartingList)
//                     return false;
//                 
//                 if (targetIsEmpty)
//                 {
//                     // Can't drop on empty bench/reserve from a different list type
//                     if (targetOwner == PlayerItemViewOwnerOption.BenchList && 
//                         sourceOwner != PlayerItemViewOwnerOption.BenchList &&
//                         sourceOwner != PlayerItemViewOwnerOption.ReserveList)
//                         return false;
//                 }
//             }
//
//             // Can't remove GK from formation
//             if ((source.TacticalPositionOption == TacticalPositionOption.GK && targetIsEmpty) ||
//                 (target.TacticalPositionOption == TacticalPositionOption.GK && sourceIsEmpty))
//             {
//                 return false;
//             }
//
//             return true;
//         }
//
//         public void ClearSelection()
//         {
//             if (_selectedItem != null)
//             {
//                 RemoveSelectionHighlight(_selectedItem);
//                 _selectedItem = null;
//                 _selectedData = null;
//             }
//         }
//
//         public void ClearAllHighlights()
//         {
//             // Clear all stored outlines
//             foreach (var kvp in _activeOutlines)
//             {
//                 if (kvp.Value != null)
//                 {
//                     kvp.Value.enabled = false;
//                 }
//             }
//             _activeOutlines.Clear();
//         }
//
//         private void AddSelectionHighlight(PlayerItemView item)
//         {
//             if (item == null || item.gameObject == null)
//                 return;
//
//             // Add visual highlight - you can customize this
//             var outline = item.GetComponent<UnityEngine.UI.Outline>();
//             if (outline == null)
//             {
//                 outline = item.gameObject.AddComponent<UnityEngine.UI.Outline>();
//             }
//             outline.effectColor = Color.yellow;
//             outline.effectDistance = new Vector2(3, 3);
//             outline.enabled = true;
//             
//             // Track the outline
//             _activeOutlines[item] = outline;
//         }
//
//         private void RemoveSelectionHighlight(PlayerItemView item)
//         {
//             if (item == null || item.gameObject == null)
//                 return;
//
//             var outline = item.GetComponent<UnityEngine.UI.Outline>();
//             if (outline != null)
//             {
//                 outline.enabled = false;
//             }
//             
//             // Remove from tracking
//             _activeOutlines.Remove(item);
//         }
//     }
// }

using UltimateFootballSystem.Core.TacticsEngine;
using UnityEngine;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
using System.Collections.Generic;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class SelectionSwapManager
    {
        private readonly TacticsBoardController _controller;
        private PlayerItemView _selectedItem;
        private PlayerItemDragData _selectedData;
        private Dictionary<PlayerItemView, UnityEngine.UI.Outline> _activeOutlines = new Dictionary<PlayerItemView, UnityEngine.UI.Outline>();

        public bool HasSelection => _selectedItem != null && _selectedItem.HasPlayerItem;
        public bool HasAnySelection => _selectedItem != null;

        public SelectionSwapManager(TacticsBoardController controller)
        {
            _controller = controller;
        }

        public void HandleItemClicked(PlayerItemView clickedItem)
        {
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                if (clickedItem == null)
                {
                    Debug.LogWarning("Clicked item is null");
                    return;
                }

                // Don't process null players unless they're in starting lineup
                if (!clickedItem.HasPlayerItem && clickedItem.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList)
                {
                    ClearSelection();
                    return;
                }

                // If we don't have a selection, select this item
                if (_selectedItem == null)
                {
                    SelectItem(clickedItem);
                }
                // If we clicked the same item, deselect it
                else if (_selectedItem == clickedItem)
                {
                    // if (_selectedItem.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList &&
                    //     _controller.tacticsPitch.AreUsableViewsShown)
                    // {
                        _controller.tacticsPitch.HideUnusedPlayerItemViews();
                    // }
                    ClearSelection();
                }
                // Otherwise, perform the swap
                else
                {
                    PerformSwap(clickedItem);
                }
            }
        }

        private void SelectItem(PlayerItemView item)
        {
            _selectedItem = item;
            _selectedData = item.GetDragData();
            _selectedData.SetDragSourceViewReference(item);

            // using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            // {
                // for formation change
                if (_selectedItem.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    _controller.tacticsPitch.ShowUsablePlayerItemViews();
                }
            // }

            // Add visual feedback
            AddSelectionHighlight(item);
            
            Debug.Log($"Selected: {item.Profile?.Name ?? "Empty"} at position {item.TacticalPositionOption}");
        }

        private void PerformSwap(PlayerItemView targetItem)
        {
            if (targetItem == null || _selectedData == null)
            {
                Debug.LogWarning("Cannot perform swap - target or selected data is null");
                ClearSelection();
                return;
            }

            var targetData = targetItem.GetDragData();
            targetData.SetDropTargetViewReference(targetItem);

            // Check if swap is valid (similar to drop validation)
            if (!CanSwap(_selectedData, targetData))
            {
                Debug.Log("Cannot swap these items");
                if (_controller.errorAudioClip != null)
                {
                    _controller.PlayErrorSound();
                }
                return;
            }

            // Log before clearing (if needed for debugging)
            Debug.Log($"Swapping: {_selectedData.Profile?.Name ?? "Empty"} with {targetData.Profile?.Name ?? "Empty"}");

            // Store the data before clearing selection
            var sourceData = _selectedData;
            
            // Store the views and their states before clearing
            var sourceView = sourceData.DragSourceView;
            var targetView = targetData.DropTargetView;
            bool sourceWasInFormation = sourceView.InUseForFormation;
            bool targetWasInFormation = targetView.InUseForFormation;
            
            // Clear selection to remove visual feedback
            ClearSelection();

            // Perform the swap
            _controller.BoardPlayerItemManager.SwapPlayersSelected(sourceData, targetData);
            _controller.PlayClickSound();
            
            // Handle formation status updates for starting list positions
            if (sourceView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList || 
                targetView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                // Update source view formation status
                if (sourceView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    // Source now has target's player
                    bool sourceNowHasPlayer = targetData.IsValidPlayer();
                    if (sourceView.InUseForFormation != sourceNowHasPlayer)
                    {
                        sourceView.SetInUseForFormation(sourceNowHasPlayer);
                    }
                }
                
                // Update target view formation status
                if (targetView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    // Target now has source's player
                    bool targetNowHasPlayer = sourceData.IsValidPlayer();
                    if (targetView.InUseForFormation != targetNowHasPlayer)
                    {
                        targetView.SetInUseForFormation(targetNowHasPlayer);
                    }
                }
                
                // Hide unused views if we moved from starting list
                if (sourceView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    _controller.tacticsPitch.HideUnusedPlayerItemViews();
                }
            }
        }

        private System.Collections.IEnumerator DelayedSwap(PlayerItemDragData sourceData, PlayerItemDragData targetData)
        {
            // Wait one frame for UI to stabilize
            yield return null;
            
            // Perform the swap
            _controller.BoardPlayerItemManager.SwapPlayersSelected(sourceData, targetData);
            // _controller.BoardPlayerItemManager.SwapPlayersDropped(sourceData, targetData);
            
            // Play completion sound
            _controller.PlayClickSound();
            
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                if (sourceData.DragSourceView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList &&
                    targetData.DropTargetView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {

                    _controller.tacticsPitch.HideUnusedPlayerItemViews();
                }
            }
        }

        private bool CanSwap(PlayerItemDragData source, PlayerItemDragData target)
        {
            // Null checks
            if (source == null || target == null)
                return false;

            // Can't swap with self
            if (source.DragSourceView == target.DropTargetView)
                return false;

            // Additional validation for empty slots
            bool sourceIsEmpty = !source.IsValidPlayer();
            bool targetIsEmpty = !target.IsValidPlayer();

            // Can't swap two empty slots
            if (sourceIsEmpty && targetIsEmpty)
                return false;

            // Handle empty slot swaps based on view owner
            if (sourceIsEmpty || targetIsEmpty)
            {
                // Check view owner compatibility
                var sourceOwner = source.DragSourceView.ViewOwnerOption;
                var targetOwner = target.DropTargetView.ViewOwnerOption;

                // Can't move empty slots between different lists (except starting list)
                if (sourceIsEmpty && sourceOwner != PlayerItemViewOwnerOption.StartingList)
                    return false;
                
                if (targetIsEmpty)
                {
                    // Can't drop on empty bench/reserve from a different list type
                    if (targetOwner == PlayerItemViewOwnerOption.BenchList && 
                        sourceOwner != PlayerItemViewOwnerOption.BenchList &&
                        sourceOwner != PlayerItemViewOwnerOption.ReserveList)
                        return false;
                }
            }

            // Can't remove GK from formation
            if ((source.TacticalPositionOption == TacticalPositionOption.GK && targetIsEmpty) ||
                (target.TacticalPositionOption == TacticalPositionOption.GK && sourceIsEmpty))
            {
                return false;
            }

            return true;
        }

        public void ClearSelection()
        {
            if (_selectedItem != null)
            {
                RemoveSelectionHighlight(_selectedItem);
                _selectedItem = null;
                _selectedData = null;
            }
        }

        public void ClearAllHighlights()
        {
            // Clear all stored outlines
            foreach (var kvp in _activeOutlines)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.enabled = false;
                }
            }
            _activeOutlines.Clear();
        }

        private void AddSelectionHighlight(PlayerItemView item)
        {
            if (item == null || item.gameObject == null)
                return;

            // Add visual highlight - you can customize this
            var outline = item.GetComponent<UnityEngine.UI.Outline>();
            if (outline == null)
            {
                outline = item.gameObject.AddComponent<UnityEngine.UI.Outline>();
            }
            outline.effectColor = Color.yellow;
            outline.effectDistance = new Vector2(3, 3);
            outline.enabled = true;
            
            // Track the outline
            _activeOutlines[item] = outline;
        }

        private void RemoveSelectionHighlight(PlayerItemView item)
        {
            if (item == null || item.gameObject == null)
                return;

            var outline = item.GetComponent<UnityEngine.UI.Outline>();
            if (outline != null)
            {
                outline.enabled = false;
            }
            
            // Remove from tracking
            _activeOutlines.Remove(item);
        }
    }
}