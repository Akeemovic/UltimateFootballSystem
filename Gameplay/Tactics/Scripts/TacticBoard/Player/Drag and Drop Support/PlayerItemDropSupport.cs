using UIWidgets;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.Options;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.Drag_and_Drop_Support
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
            // Debug.Log($"Dropping profile {data.Profile.Name} on target {Target.Profile.Name}");
            // HideDropIndicator();

            // data.SetDropTargetViewReference(Target);

            // Controller.SwapPlayers(data, Target.GetDragData());
            Controller.PlayerItemManager.SwapPlayers(data, Target.GetDragData());

            // Don't call SetInUseForFormation for views NOT OWNED by the StartingList
            if (Target.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList) return;
            // Make target available for formation and source not available for formation
            // if the source is not already in formation 
            if (!data.DragSourceView.InUseForFormation || Target.InUseForFormation) return;
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                data.DragSourceView.SetInUseForFormation(false);
                Target.SetInUseForFormation(true);
            }
            // HideDropIndicator();
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
