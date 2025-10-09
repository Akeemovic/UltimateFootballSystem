using UltimateFootballSystem.Core.TacticsEngine;
using UnityEngine;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
using System.Collections.Generic;
using UltimateFootballSystem.Core.Entities;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    /// <summary>
    /// Manages click-to-swap selection functionality.
    /// Works with model for data operations and view for visual feedback.
    /// </summary>
    public class SelectionSwapManager
    {
        private readonly TacticsBoardController _controller;
        private readonly TacticBoardModel _model;
        private readonly TacticsPitchView _view;

        private PlayerItemView _selectedItem;
        private PlayerItemDragData _selectedData;
        private Dictionary<PlayerItemView, UnityEngine.UI.Outline> _activeOutlines = new Dictionary<PlayerItemView, UnityEngine.UI.Outline>();

        public bool HasSelection => _selectedItem != null && _selectedItem.HasPlayerItem;
        public bool HasAnySelection => _selectedItem != null;

        public SelectionSwapManager(TacticsBoardController controller, TacticBoardModel model, TacticsPitchView view)
        {
            _controller = controller;
            _model = model;
            _view = view;
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
                    _view.HideUnusedPlayerItemViews();
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

            // Show all usable views if selecting from starting list
            if (_selectedItem.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                _view.ShowUsablePlayerItemViews();
            }

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

            // Check if swap is valid
            if (!CanSwap(_selectedData, targetData))
            {
                Debug.Log("Cannot swap these items");
                _controller.PlayErrorSound();
                return;
            }

            Debug.Log($"Swapping: {_selectedData.Profile?.Name ?? "Empty"} with {targetData.Profile?.Name ?? "Empty"}");

            // Store data before clearing selection
            var sourceData = _selectedData;

            // Clear selection to remove visual feedback
            ClearSelection();

            // Call unified swap logic in controller - handles everything!
            _controller.SwapPlayers(sourceData, targetData);

            // Play click sound
            _controller.PlayClickSound();

            // Hide unused views if we swapped from starting list
            if (sourceData.DragSourceView?.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList ||
                targetData.DropTargetView?.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                _view.HideUnusedPlayerItemViews();
            }
        }

        // Note: SwapInModel, UpdateViewAfterSwap, SyncModelFormationFromViews all removed
        // They're now handled by the unified Controller.SwapPlayers() method
        private bool CanSwap(PlayerItemDragData source, PlayerItemDragData target)
        {
            // Null checks
            if (source == null || target == null)
                return false;

            // Can't swap with self
            if (source.DragSourceView == target.DropTargetView)
                return false;

            bool sourceIsEmpty = !source.IsValidPlayer();
            bool targetIsEmpty = !target.IsValidPlayer();

            // Can't swap two empty slots unless between starting list items
            if (sourceIsEmpty && targetIsEmpty &&
                source.DragSourceView.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList &&
                target.DragSourceView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                return false;
            }

            // Handle empty slot swaps
            if (sourceIsEmpty || targetIsEmpty)
            {
                var sourceOwner = source.DragSourceView.ViewOwnerOption;
                var targetOwner = target.DropTargetView.ViewOwnerOption;

                if (sourceIsEmpty && sourceOwner != PlayerItemViewOwnerOption.StartingList)
                    return false;

                if (targetIsEmpty)
                {
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

            var outline = item.GetComponent<UnityEngine.UI.Outline>();
            if (outline == null)
            {
                outline = item.gameObject.AddComponent<UnityEngine.UI.Outline>();
            }
            outline.effectColor = Color.yellow;
            outline.effectDistance = new Vector2(3, 3);
            outline.enabled = true;

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

            _activeOutlines.Remove(item);
        }
    }
}
