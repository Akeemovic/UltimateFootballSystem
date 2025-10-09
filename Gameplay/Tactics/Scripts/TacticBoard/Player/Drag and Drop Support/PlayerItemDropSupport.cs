using System.Collections.Generic;
using UIWidgets;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Gameplay.Tactics.Tactics;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    /// <summary>
    /// Player Profile drop.
    /// </summary>
    [RequireComponent(typeof(PlayerItemView))]
    public class PlayerItemDropSupport : MonoBehaviour, IDropSupport<PlayerItemDragData>
    {
        /// <summary>
        /// Controller.
        /// </summary>
        public TacticsBoardController Controller;

        /// <summary>
        /// Target.
        /// </summary>
        protected PlayerItemView target;

        /// <summary>
        /// Target.
        /// </summary>
        public PlayerItemView Target
        {
            get
            {
                if (target == null || target.Profile == null)
                {
                    target = GetComponent<PlayerItemView>();
                }
                // Debug.Log("Target Target: " + target.Profile.Name);
                return target;
            }
        }

        private void Start()
        {
            // Controller = TacticsBoardController.Instance;
            Controller = Target.Controller;

            if (Controller == null)
            {
                Debug.LogError("TacticsBoardController is not initialized properly.", this);
            }
        }

        /// <summary>
        /// Determines whether this instance can receive drop with the specified data and eventData.
        /// </summary>
        /// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
        /// <param name="data">Data.</param>
        /// <param name="eventData">Event data.</param>
        public virtual bool CanReceiveDrop(PlayerItemDragData data, PointerEventData eventData)
        {
            if (Target == null)
            {
                // Debug.Log("Drop target is null or has no profile.");
                return false;
            }
        
            data.SetDropTargetViewReference(Target);
            var dropTargetData = Target.GetDragData();

            Debug.Log("Drop target vo : " + data.DropTargetView.ViewOwnerOption);
            Debug.Log("Drop target is valid?: " + dropTargetData.IsValidPlayer());
        
            // Null/Empty players cannot be dragged to another list.
            if (!data.DragSourceView.HasPlayerItem && data.DragSourceView.ViewOwnerOption != Target.ViewOwnerOption)
            {
                return false;
            }
        
            // CANNOT Drop on NULL 
            if (!dropTargetData.IsValidPlayer())
            {
                // if they DONT belong to starting or bench
                // if(data.DropTargetView.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList 
                //    &&
                //    data.DropTargetView.ViewOwnerOption != PlayerItemViewOwnerOption.BenchList
                //   )
                // {
                //     return false;
                // }
                //
            
                // if drop is either starting or bench
                if (data.DropTargetView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList
                    ||
                    data.DropTargetView.ViewOwnerOption == PlayerItemViewOwnerOption.BenchList
                   )
                {
                    // and dragSource from starting, but drop is not
                    if (data.DragSourceView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList &&
                        data.DropTargetView.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList)
                    {
                        return false;
                    }
                    // if (data.DragSourceView.ViewOwnerOption == PlayerItemViewOwnerOption.BenchList &&
                    //     data.DropTargetView.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList)
                    // {
                    //     return false;
                    // }
                }
            
                // if both drag and drop are bench
                if(data.DragSourceView.ViewOwnerOption == PlayerItemViewOwnerOption.BenchList 
                   && 
                   data.DropTargetView.ViewOwnerOption == PlayerItemViewOwnerOption.BenchList
                  )
                {
                    return false;
                }
            }

            // if (!data.IsValidPlayer())
            // {
            //     if(data.DropTargetView.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList 
            //        // && 
            //        // data.DropTargetView.ViewOwnerOption != PlayerItemViewOwnerOption.BenchList
            //        )
            //     {
            //         return false;
            //     }
            // }
    
            if (data.TacticalPositionOption == TacticalPositionOption.GK && !dropTargetData.IsValidPlayer() || 
                dropTargetData.TacticalPositionOption == TacticalPositionOption.GK && !data.IsValidPlayer())
            {
                Debug.Log("Cannot take GK out of formation");
                return false;
            }
        
            // data.SetDropTargetViewReference(Target);
            if (!CanSwap(data, dropTargetData))
            {
                // Debug.Log("Cannot swap items.");
                return false;
            }
        
            ShowDropIndicator(data);
    
            return true;
        }

        /// <summary>
        /// Handle dropped data.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="eventData">Event data.</param>
        public virtual void Drop(PlayerItemDragData data, PointerEventData eventData)
        {
            // Create target drag data and set the drop target reference
            var targetData = Target.GetDragData();
            targetData.SetDropTargetViewReference(Target);

            // Call unified swap logic in controller - handles formation changes, model updates, view refreshes
            Controller.SwapPlayers(data, targetData);

            // Play click sound on successful drop
            Controller.PlayClickSound();
        }

        /// <summary>
        /// Handle canceled drop.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <param name="eventData">Event data.</param>
        public virtual void DropCanceled(PlayerItemDragData data, PointerEventData eventData)
        {
            HideDropIndicator();
        }

        /// <summary>
        /// Shows the drop indicator.
        /// </summary>
        /// <param name="data">Data.</param>
        protected virtual void ShowDropIndicator(PlayerItemDragData data)
        {
            // Debug.Log("I can receive drop, I am" + data.Profile.Name);
            // TacticsBoardController.Instance.TacticsPitch.ShowUsablePlayerItemViews();
        }

        /// <summary>
        /// Hides the drop indicator.
        /// </summary>
        protected virtual void HideDropIndicator()
        {
            // TacticsBoardController.Instance.TacticsPitch.HideUnusedPlayerItemViews();
        }
    
        private bool CanSwap(PlayerItemDragData dragged, PlayerItemDragData dropTarget)
        {
            return (!dragged.IsSamePositionAs(dropTarget));
        
            if (!dragged.IsSamePositionAs(dropTarget))
            {
                return false;
            }
        
            return true;
        }
    }
}
