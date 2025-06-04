using System;
using UltimateFootballSystem.Gameplay.Tactics.Tactics;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class BoardPlayerItemManager
    {
        private readonly TacticsBoardController _controller; // Reference to the controller

        // Constructor that takes a reference to the controller
        public BoardPlayerItemManager(TacticsBoardController controller)
        {
            _controller = controller;
        }
    
        public void RemoveSubstitute(PlayerItemDragData data)
        {
            if (!data.IsValidPlayer()) return;

            Debug.Log("Removing Sub: " + data.Profile.Name);
            using var _ = _controller.SubstitutesPlayersItems.BeginUpdate();
            using var __ = _controller.ReservePlayersItems.BeginUpdate();

            // Add removed player to reserves list
            _controller.ReservePlayersItems.Add(data.Profile);

            // Then remove it from bench list
            _controller.SubstitutesPlayersItems.RemoveAt(data.BenchPlayersListIndex);

            // End Updates
            _controller.SubstitutesPlayersItems.EndUpdate();
            _controller.ReservePlayersItems.EndUpdate();

            // Update UI
            _controller.InitializeSubstitutePlayers();
            _controller.InitializeReservePlayers();
        }
    
        // Swapping for DnD (Drag and Drop)
        public void SwapPlayersDropped(PlayerItemDragData dragged, PlayerItemDragData dropTarget)
        {
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                using var _ = _controller.SubstitutesPlayersItems.BeginUpdate();
                using var __ = _controller.ReservePlayersItems.BeginUpdate();

                Debug.Log(
                    $"Attempting to swap: dragged profile {dragged.Profile.Name} at {dragged.TacticalPositionOption} with drop target profile {dropTarget.Profile?.Name ?? "null"}");

                Debug.Log("DropTargetView.InUseForFormation?: " + dropTarget.InUseForFormation);

                // Perform the swap even if the drop target is null
                if (IsValidIndex(dropTarget.StartingPlayersListIndex, _controller.startingPlayersViews))
                {
                    Debug.Log(
                        $"Swapping dragged profile {dragged.Profile.Name} to starting position {dropTarget.StartingPlayersListIndex}");
                    _controller.startingPlayersViews[dropTarget.StartingPlayersListIndex].SetPlayerData(dragged.Profile);
                }
                else if (IsValidIndex(dropTarget.BenchPlayersListIndex, _controller.substitutesPlayersViews))
                {
                    Debug.Log(
                        $"Swapping dragged profile {dragged.Profile.Name} to bench position {dropTarget.BenchPlayersListIndex}");

                    if (dropTarget.IsValidPlayer())
                    {
                        _controller.substitutesPlayersViews[dropTarget.BenchPlayersListIndex].SetPlayerData(dragged.Profile);
                        _controller.SubstitutesPlayersItems[dropTarget.BenchPlayersListIndex] = dragged.Profile;
                    }
                    else
                    {
                        _controller.SubstitutesPlayersItems.Add(dragged.Profile);
                        _controller.InitializeSubstitutePlayers();
                    }

                }
                else if (IsValidIndex(dropTarget.ReservePlayersListIndex, _controller.reservePlayersViews))
                {
                    Debug.Log(
                        $"Swapping dragged profile {dragged.Profile.Name} to reserve position {dropTarget.ReservePlayersListIndex}");
                    _controller.reservePlayersViews[dropTarget.ReservePlayersListIndex].SetPlayerData(dragged.Profile);
                    _controller.ReservePlayersItems[dropTarget.ReservePlayersListIndex] = dragged.Profile;
                }
                else
                {
                    Debug.LogError("Invalid drop target or uninitialized PlayerItemView");
                    return;
                }

                // Swap the drop target profile back to the original position of the dragged profile
                if (IsValidIndex(dragged.StartingPlayersListIndex, _controller.startingPlayersViews))
                {
                    _controller.startingPlayersViews[dragged.StartingPlayersListIndex].SetPlayerData(dropTarget.Profile);
                }
                else if (IsValidIndex(dragged.BenchPlayersListIndex, _controller.substitutesPlayersViews))
                {
                    if(dragged.DropTargetView.ViewOwnerOption != PlayerItemViewOwnerOption.BenchList && !dropTarget.IsValidPlayer()) 
                    {
                        _controller.SubstitutesPlayersItems.RemoveAt(dragged.BenchPlayersListIndex);
                        _controller.InitializeSubstitutePlayers();
                    }
                    else
                    {
                        _controller.substitutesPlayersViews[dragged.BenchPlayersListIndex].SetPlayerData(dropTarget.Profile);
                        _controller.SubstitutesPlayersItems[dragged.BenchPlayersListIndex] = dropTarget.Profile;
                    }
                }
                else if (IsValidIndex(dragged.ReservePlayersListIndex, _controller.reservePlayersViews))
                {
                    if (!dropTarget.IsValidPlayer())
                    {
                        _controller.ReservePlayersItems.RemoveAt(dragged.ReservePlayersListIndex);
                        _controller.InitializeReservePlayers();
                    }
                    else
                    {
                        _controller.reservePlayersViews[dragged.ReservePlayersListIndex].SetPlayerData(dropTarget.Profile);
                        _controller.ReservePlayersItems[dragged.ReservePlayersListIndex] = dropTarget.Profile;
                    }
                }
                else
                {
                    Debug.LogError("Invalid dragged target or uninitialized PlayerItemView");
                }

                _controller.SubstitutesPlayersItems.EndUpdate();
                _controller.ReservePlayersItems.EndUpdate();
            }
        }

        // Swapping for DnD (Drag and Drop)
        public void SwapPlayersSelected(PlayerItemDragData dragged, PlayerItemDragData dropTarget)
        {
            throw new NotImplementedException();
        }

        private bool IsValidIndex(int index, PlayerItemView[] views)
        {
            return index >= 0 && index < views.Length && views[index] != null;
        }
    }
}
