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

            // Use controller as mediator
            _controller.boardViewRefreshManager.RefreshSubstituteViews();
            // _controller.boardViewRefreshManager.RefreshReserveViews();
            _controller.BoardInitializationManager.InitializeReservePlayers();
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

                bool involvedSubstitutes = false;
                bool involvedReserves = false;

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
                        // Adding to an empty substitute slot
                        if (dropTarget.BenchPlayersListIndex < _controller.SubstitutesPlayersItems.Count)
                        {
                            _controller.SubstitutesPlayersItems[dropTarget.BenchPlayersListIndex] = dragged.Profile;
                        }
                        else
                        {
                            _controller.SubstitutesPlayersItems.Add(dragged.Profile);
                        }
                    }
                    involvedSubstitutes = true;
                }
                else if (IsValidIndex(dropTarget.ReservePlayersListIndex, _controller.reservePlayersViews))
                {
                    Debug.Log(
                        $"Swapping dragged profile {dragged.Profile.Name} to reserve position {dropTarget.ReservePlayersListIndex}");
                    _controller.reservePlayersViews[dropTarget.ReservePlayersListIndex].SetPlayerData(dragged.Profile);
                    _controller.ReservePlayersItems[dropTarget.ReservePlayersListIndex] = dragged.Profile;
                    involvedReserves = true;
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
                    if (dragged.DropTargetView.ViewOwnerOption != PlayerItemViewOwnerOption.BenchList && !dropTarget.IsValidPlayer()) 
                    {
                        // Moving from bench to empty slot - set original position to null
                        if (dragged.BenchPlayersListIndex < _controller.SubstitutesPlayersItems.Count)
                        {
                            _controller.SubstitutesPlayersItems[dragged.BenchPlayersListIndex] = null;
                        }
                    }
                    else
                    {
                        _controller.substitutesPlayersViews[dragged.BenchPlayersListIndex].SetPlayerData(dropTarget.Profile);
                        if (dragged.BenchPlayersListIndex < _controller.SubstitutesPlayersItems.Count)
                        {
                            _controller.SubstitutesPlayersItems[dragged.BenchPlayersListIndex] = dropTarget.Profile;
                        }
                    }
                    involvedSubstitutes = true;
                }
                else if (IsValidIndex(dragged.ReservePlayersListIndex, _controller.reservePlayersViews))
                {
                    if (!dropTarget.IsValidPlayer())
                    {
                        // Remove from reserves when swapping to empty position
                        _controller.ReservePlayersItems.RemoveAt(dragged.ReservePlayersListIndex);
                        involvedReserves = true;
                    }
                    else
                    {
                        _controller.reservePlayersViews[dragged.ReservePlayersListIndex].SetPlayerData(dropTarget.Profile);
                        _controller.ReservePlayersItems[dragged.ReservePlayersListIndex] = dropTarget.Profile;
                        involvedReserves = true;
                    }
                }
                else
                {
                    Debug.LogError("Invalid dragged target or uninitialized PlayerItemView");
                }

                _controller.SubstitutesPlayersItems.EndUpdate();
                _controller.ReservePlayersItems.EndUpdate();

                // Use controller as mediator
                if (involvedSubstitutes)
                {
                    _controller.boardViewRefreshManager.CompactSubstitutes();
                    _controller.boardViewRefreshManager.RefreshSubstituteViews();
                }
                
                if (involvedReserves)
                {
                    // _controller.boardViewRefreshManager.RefreshReserveViews();
                    _controller.BoardInitializationManager.InitializeReservePlayers();
                }
            }
        }

        // Swapping for Select-and-swap
        public void SwapPlayersSelected(PlayerItemDragData selected1, PlayerItemDragData selected2)
        {
            // using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            // {
                using var _ = _controller.SubstitutesPlayersItems.BeginUpdate();
                using var __ = _controller.ReservePlayersItems.BeginUpdate();
        
                bool involvedSubstitutes = false;
                bool involvedReserves = false;
        
                // Store the players temporarily
                var player1 = selected1.Profile;
                var player2 = selected2.Profile;
        
                // Swap player 1 to position 2
                if (IsValidIndex(selected2.StartingPlayersListIndex, _controller.startingPlayersViews))
                {
                    _controller.startingPlayersViews[selected2.StartingPlayersListIndex].SetPlayerData(player1);
                }
                else if (IsValidIndex(selected2.BenchPlayersListIndex, _controller.substitutesPlayersViews))
                {
                    _controller.substitutesPlayersViews[selected2.BenchPlayersListIndex].SetPlayerData(player1);
                    if (selected2.BenchPlayersListIndex < _controller.SubstitutesPlayersItems.Count)
                    {
                        _controller.SubstitutesPlayersItems[selected2.BenchPlayersListIndex] = player1;
                    }
                    involvedSubstitutes = true;
                }
                else if (IsValidIndex(selected2.ReservePlayersListIndex, _controller.reservePlayersViews))
                {
                    _controller.reservePlayersViews[selected2.ReservePlayersListIndex].SetPlayerData(player1);
                    _controller.ReservePlayersItems[selected2.ReservePlayersListIndex] = player1;
                    involvedReserves = true;
                }
        
                // Swap player 2 to position 1
                if (IsValidIndex(selected1.StartingPlayersListIndex, _controller.startingPlayersViews))
                {
                    _controller.startingPlayersViews[selected1.StartingPlayersListIndex].SetPlayerData(player2);
                }
                else if (IsValidIndex(selected1.BenchPlayersListIndex, _controller.substitutesPlayersViews))
                {
                    _controller.substitutesPlayersViews[selected1.BenchPlayersListIndex].SetPlayerData(player2);
                    if (selected1.BenchPlayersListIndex < _controller.SubstitutesPlayersItems.Count)
                    {
                        _controller.SubstitutesPlayersItems[selected1.BenchPlayersListIndex] = player2;
                    }
                    involvedSubstitutes = true;
                }
                else if (IsValidIndex(selected1.ReservePlayersListIndex, _controller.reservePlayersViews))
                {
                    _controller.reservePlayersViews[selected1.ReservePlayersListIndex].SetPlayerData(player2);
                    _controller.ReservePlayersItems[selected1.ReservePlayersListIndex] = player2;  
                    involvedReserves = true;
                }
        
                _controller.SubstitutesPlayersItems.EndUpdate();
                _controller.ReservePlayersItems.EndUpdate();
        
                // Use controller as mediator
                if (involvedSubstitutes)
                {
                    _controller.boardViewRefreshManager.RefreshSubstituteViews();
                }
                
                if (involvedReserves)
                {
                    // _controller.boardViewRefreshManager.RefreshReserveViews();
                    _controller.BoardInitializationManager.InitializeReservePlayers();
                }
            // }
        }
        
        // Swapping for Select-and-swap
        // public void SwapPlayersSelected(PlayerItemDragData selected1, PlayerItemDragData selected2)
        // {
        //     // using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
        //     // {
        //         using var _ = _controller.SubstitutesPlayersItems.BeginUpdate();
        //         using var __ = _controller.ReservePlayersItems.BeginUpdate();
        //
        //         bool involvedSubstitutes = false;
        //         bool involvedReserves = false;
        //
        //         // Store the players temporarily
        //         var player1 = selected1.Profile;
        //         var player2 = selected2.Profile;
        //         
        //         // Check if either player is null/empty
        //         bool player1IsEmpty = !selected1.IsValidPlayer();
        //         bool player2IsEmpty = !selected2.IsValidPlayer();
        //
        //         // Handle player 1 moving to position 2
        //         if (IsValidIndex(selected2.StartingPlayersListIndex, _controller.startingPlayersViews))
        //         {
        //             _controller.startingPlayersViews[selected2.StartingPlayersListIndex].SetPlayerData(player1);
        //         }
        //         else if (IsValidIndex(selected2.BenchPlayersListIndex, _controller.substitutesPlayersViews))
        //         {
        //             if (!player2IsEmpty || player1IsEmpty)
        //             {
        //                 // Normal swap or empty-to-empty
        //                 _controller.substitutesPlayersViews[selected2.BenchPlayersListIndex].SetPlayerData(player1);
        //                 if (selected2.BenchPlayersListIndex < _controller.SubstitutesPlayersItems.Count)
        //                 {
        //                     _controller.SubstitutesPlayersItems[selected2.BenchPlayersListIndex] = player1;
        //                 }
        //             }
        //             else
        //             {
        //                 // Moving to an empty substitute slot
        //                 if (selected2.BenchPlayersListIndex < _controller.SubstitutesPlayersItems.Count)
        //                 {
        //                     _controller.SubstitutesPlayersItems[selected2.BenchPlayersListIndex] = player1;
        //                 }
        //                 else
        //                 {
        //                     _controller.SubstitutesPlayersItems.Add(player1);
        //                 }
        //             }
        //             involvedSubstitutes = true;
        //         }
        //         else if (IsValidIndex(selected2.ReservePlayersListIndex, _controller.reservePlayersViews))
        //         {
        //             _controller.reservePlayersViews[selected2.ReservePlayersListIndex].SetPlayerData(player1);
        //             _controller.ReservePlayersItems[selected2.ReservePlayersListIndex] = player1;
        //             involvedReserves = true;
        //         }
        //
        //         // Handle player 2 moving to position 1
        //         if (IsValidIndex(selected1.StartingPlayersListIndex, _controller.startingPlayersViews))
        //         {
        //             _controller.startingPlayersViews[selected1.StartingPlayersListIndex].SetPlayerData(player2);
        //         }
        //         else if (IsValidIndex(selected1.BenchPlayersListIndex, _controller.substitutesPlayersViews))
        //         {
        //             // Check if we're moving from a non-bench position to an empty bench slot
        //             if (selected2.DragSourceView.ViewOwnerOption != PlayerItemViewOwnerOption.BenchList && player2IsEmpty)
        //             {
        //                 // Moving to empty slot - set original position to null
        //                 if (selected1.BenchPlayersListIndex < _controller.SubstitutesPlayersItems.Count)
        //                 {
        //                     _controller.SubstitutesPlayersItems[selected1.BenchPlayersListIndex] = null;
        //                 }
        //             }
        //             else
        //             {
        //                 // Normal swap
        //                 _controller.substitutesPlayersViews[selected1.BenchPlayersListIndex].SetPlayerData(player2);
        //                 if (selected1.BenchPlayersListIndex < _controller.SubstitutesPlayersItems.Count)
        //                 {
        //                     _controller.SubstitutesPlayersItems[selected1.BenchPlayersListIndex] = player2;
        //                 }
        //             }
        //             involvedSubstitutes = true;
        //         }
        //         else if (IsValidIndex(selected1.ReservePlayersListIndex, _controller.reservePlayersViews))
        //         {
        //             if (player2IsEmpty)
        //             {
        //                 // Remove from reserves when swapping to empty position
        //                 _controller.ReservePlayersItems.RemoveAt(selected1.ReservePlayersListIndex);
        //                 involvedReserves = true;
        //             }
        //             else
        //             {
        //                 // Normal swap
        //                 _controller.reservePlayersViews[selected1.ReservePlayersListIndex].SetPlayerData(player2);
        //                 _controller.ReservePlayersItems[selected1.ReservePlayersListIndex] = player2;
        //                 involvedReserves = true;
        //             }
        //         }
        //
        //         _controller.SubstitutesPlayersItems.EndUpdate();
        //         _controller.ReservePlayersItems.EndUpdate();
        //
        //         // Use controller as mediator
        //         if (involvedSubstitutes)
        //         {
        //             _controller.boardViewRefreshManager.CompactSubstitutes();
        //             _controller.boardViewRefreshManager.RefreshSubstituteViews();
        //         }
        //         
        //         if (involvedReserves)
        //         {
        //             _controller.BoardInitializationManager.InitializeReservePlayers();
        //         }
        //     // }
        // }

        private bool IsValidIndex(int index, PlayerItemView[] views)
        {
            return index >= 0 && index < views.Length && views[index] != null;
        }
    }
}