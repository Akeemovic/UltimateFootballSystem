using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.Drag_and_Drop_Support;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.Options;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Compositions
{
    public class PlayerItemManager
    {
        private readonly TacticsBoardController _controller; // Reference to the controller

        // Constructor that takes a reference to the controller
        public PlayerItemManager(TacticsBoardController controller)
        {
            _controller = controller;
        }
    
        public void RemoveSubstitute(PlayerItemDragData data)
        {
            if (!data.IsValidPlayer()) return;

            Debug.Log("Removing Sub: " + data.Profile.Name);
            using var _ = _controller.substitutesPlayersItems.BeginUpdate();
            using var __ = _controller.reservePlayersItems.BeginUpdate();

            // Add removed player to reserves list
            _controller.reservePlayersItems.Add(data.Profile);

            // Then remove it from bench list
            _controller.substitutesPlayersItems.RemoveAt(data.BenchPlayersListIndex);

            // End Updates
            _controller.substitutesPlayersItems.EndUpdate();
            _controller.reservePlayersItems.EndUpdate();

            // Update UI
            _controller.InitializeSubstitutePlayers();
            _controller.InitializeReservePlayers();
        }
    
        public void SwapPlayers(PlayerItemDragData dragged, PlayerItemDragData dropTarget)
        {
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                using var _ = _controller.substitutesPlayersItems.BeginUpdate();
                using var __ = _controller.reservePlayersItems.BeginUpdate();

                Debug.Log(
                    $"Attempting to swap: dragged profile {dragged.Profile.Name} at {dragged.TacticalPositionOption} with drop target profile {dropTarget.Profile?.Name ?? "null"}");

                Debug.Log("DropTargetView.InUseForFormation?: " + dropTarget.InUseForFormation);

                // Perform the swap even if the drop target is null
                if (IsValidIndex(dropTarget.StartingPlayersListIndex, _controller.StartingPlayersViews))
                {
                    Debug.Log(
                        $"Swapping dragged profile {dragged.Profile.Name} to starting position {dropTarget.StartingPlayersListIndex}");
                    _controller.StartingPlayersViews[dropTarget.StartingPlayersListIndex].SetPlayerData(dragged.Profile);
                }
                else if (IsValidIndex(dropTarget.BenchPlayersListIndex, _controller.SubstitutesPlayersViews))
                {
                    Debug.Log(
                        $"Swapping dragged profile {dragged.Profile.Name} to bench position {dropTarget.BenchPlayersListIndex}");

                    if (dropTarget.IsValidPlayer())
                    {
                        _controller.SubstitutesPlayersViews[dropTarget.BenchPlayersListIndex].SetPlayerData(dragged.Profile);
                        _controller.substitutesPlayersItems[dropTarget.BenchPlayersListIndex] = dragged.Profile;
                    }
                    else
                    {
                        _controller.substitutesPlayersItems.Add(dragged.Profile);
                        _controller.InitializeSubstitutePlayers();
                    }

                }
                else if (IsValidIndex(dropTarget.ReservePlayersListIndex, _controller.ReservePlayersViews))
                {
                    Debug.Log(
                        $"Swapping dragged profile {dragged.Profile.Name} to reserve position {dropTarget.ReservePlayersListIndex}");
                    _controller.ReservePlayersViews[dropTarget.ReservePlayersListIndex].SetPlayerData(dragged.Profile);
                    _controller.reservePlayersItems[dropTarget.ReservePlayersListIndex] = dragged.Profile;
                }
                else
                {
                    Debug.LogError("Invalid drop target or uninitialized PlayerItemView");
                    return;
                }

                // Swap the drop target profile back to the original position of the dragged profile
                if (IsValidIndex(dragged.StartingPlayersListIndex, _controller.StartingPlayersViews))
                {
                    _controller.StartingPlayersViews[dragged.StartingPlayersListIndex].SetPlayerData(dropTarget.Profile);
                }
                else if (IsValidIndex(dragged.BenchPlayersListIndex, _controller.SubstitutesPlayersViews))
                {
                    if(dragged.DropTargetView.ViewOwnerOption != PlayerItemViewOwnerOption.BenchList && !dropTarget.IsValidPlayer()) 
                    {
                        _controller.substitutesPlayersItems.RemoveAt(dragged.BenchPlayersListIndex);
                        _controller.InitializeSubstitutePlayers();
                    }
                    else
                    {
                        _controller.SubstitutesPlayersViews[dragged.BenchPlayersListIndex].SetPlayerData(dropTarget.Profile);
                        _controller.substitutesPlayersItems[dragged.BenchPlayersListIndex] = dropTarget.Profile;
                    }
                }
                else if (IsValidIndex(dragged.ReservePlayersListIndex, _controller.ReservePlayersViews))
                {
                    if (!dropTarget.IsValidPlayer())
                    {
                        _controller.reservePlayersItems.RemoveAt(dragged.ReservePlayersListIndex);
                        _controller.InitializeReservePlayers();
                    }
                    else
                    {
                        _controller.ReservePlayersViews[dragged.ReservePlayersListIndex].SetPlayerData(dropTarget.Profile);
                        _controller.reservePlayersItems[dragged.ReservePlayersListIndex] = dropTarget.Profile;
                    }
                }
                else
                {
                    Debug.LogError("Invalid dragged target or uninitialized PlayerItemView");
                }

                _controller.substitutesPlayersItems.EndUpdate();
                _controller.reservePlayersItems.EndUpdate();
            }
        }
    
        private bool IsValidIndex(int index, PlayerItemView[] views)
        {
            return index >= 0 && index < views.Length && views[index] != null;
        }
    }
}
