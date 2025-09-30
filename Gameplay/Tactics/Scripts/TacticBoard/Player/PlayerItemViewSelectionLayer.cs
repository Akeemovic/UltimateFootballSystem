using System;
using System.Linq;
using UIWidgets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PlayerItemViewSelectionLayer : Selectable, IPointerClickHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] 
        private PlayerItemView playerItemView;

        private ScrollRect parentScrollRect;
        private Vector2 pointerDownPos;
        private float pointerDownTime;
        private bool isDragging = false;
        private bool isClickHandled = false;
        private const float MAX_TAP_TIME = 0.2f;
        // private const float MAX_TAP_DISTANCE = 10f;
        private const float MAX_TAP_DISTANCE = 20f; // or 25f
        
        protected override void Awake()
        {
            base.Awake();
            
            // Try to get playerItemView if not set in inspector
            if (playerItemView == null)
            {
                playerItemView = GetComponent<PlayerItemView>();
            }
            
            parentScrollRect = GetComponentInParent<ScrollRect>();

            if (Application.isMobilePlatform)
            {
                transition = Transition.None;
            }
            
            // For Inital RUN
            // Remove selectability for non-starting list items with no valid player
            RemoveSelectabilityForNonStartingListItemsWithNoValidPlayer();

            // Subscribe to OnDataChanged event
            playerItemView.OnDataChanged += (player) =>
            {
                // Update selectability for non-starting list items based on player presence
                RemoveSelectabilityForNonStartingListItemsWithNoValidPlayer();
            };
        }

        private void RemoveSelectabilityForNonStartingListItemsWithNoValidPlayer()
        {
            // Ensure non-starting list items are selectable only if they have a player
            // ie. only non-starting list items should be selectable whether they have a player or not
            // Because all starting item view are needed for formation changes
            if (playerItemView.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList)
            {
                SetSelectability(playerItemView.HasPlayerItem);
                // SetSelectability(player == null || player.Id <= 0); 
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (!interactable) return;

            base.OnSelect(eventData);
            LogSelectionState("Selected");

            // Playing selection sound should only be applicable for when Gamepad or keyboard inputs
            // are active - more on this later
            if (!isClickHandled && !Application.isMobilePlatform && playerItemView?.Controller != null)
            {
                playerItemView.Controller.PlaySelectSound();
            }
            isClickHandled = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            if (parentScrollRect != null)
            {
                parentScrollRect.OnBeginDrag(eventData);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            if (parentScrollRect != null)
            {
                parentScrollRect.OnEndDrag(eventData);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!interactable) return;

            base.OnPointerDown(eventData);
            pointerDownPos = eventData.position;
            pointerDownTime = Time.time;
        }

        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     if (Time.time - pointerDownTime <= MAX_TAP_TIME && 
        //         Vector2.Distance(eventData.position, pointerDownPos) <= MAX_TAP_DISTANCE &&
        //         !isDragging)
        //     {
        //         isClickHandled = true;
        //         HandleItemSelection();
        //         PlayClickSound();
        //     }
        // }
        
        // public void OnPointerClick(PointerEventData eventData)
        // {
        //     Debug.Log($"OnPointerClick called for {playerItemView.Profile?.Name}");
        //     Debug.Log($"Time check: {Time.time - pointerDownTime} <= {MAX_TAP_TIME}");
        //     Debug.Log($"Distance check: {Vector2.Distance(eventData.position, pointerDownPos)} <= {MAX_TAP_DISTANCE}");
        //     Debug.Log($"isDragging: {isDragging}");
        //
        //     if (Time.time - pointerDownTime <= MAX_TAP_TIME && 
        //         Vector2.Distance(eventData.position, pointerDownPos) <= MAX_TAP_DISTANCE &&
        //         !isDragging)
        //     {
        //         isClickHandled = true;
        //         HandleItemSelection();
        //         PlayClickSound();
        //     }
        // }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable || isDragging) return;

            HandleItemSelection();
        }

        private void HandleItemSelection()
        {
            if (!interactable)
            {
                Debug.Log($"[PlayerItemViewSelectionLayer] Selection blocked - item not selectable: {playerItemView?.Profile?.Name ?? "Unknown Player"}");
                return;
            }

            // Select player item  - OLD: Handle selection/swap
            // if (playerItemView?.Controller != null)
            // {
            //     playerItemView.Controller.HandleItemClicked(playerItemView);
            // }
            // else
            // {
            //     Debug.LogWarning("PlayerItemView Controller is not available");
            //     return;
            // }
            
            if (playerItemView?.mainView?.ViewMode == PlayerItemViewModeOption.Roles)
            {
                if (playerItemView.Controller?.roleSelectorDialog != null)
                // if (playerItemView.Controller?.roleSelectorDialogContainer != null)
                {
                    playerItemView.Controller.SelectedPlayerItemView = playerItemView;
                    
                    var dialog = playerItemView.Controller.roleSelectorDialog.Clone();
                    dialog.Show(modal: true,  onClose: () =>
                    {
                        Destroy(dialog.gameObject);
                    });
                    Debug.Log("dialog", dialog);
                    
                    // Method 2
                    // var dialog = playerItemView.Controller.roleSelectorDialog;
                    // dialog.gameObject.SetActive(!dialog.gameObject.activeInHierarchy);
                    // if (!dialog.gameObject.activeInHierarchy)
                    // {
                    //     dialog.GetComponent<TacticalRoleSelector>().CleanupDialog();
                    // }
                    
                    
                    // var pos = playerItemView.Controller.SelectedPlayerItemView.TacticalPosition;
                    // var availableRoleOptions = playerItemView.Controller.SelectedPlayerItemView.TacticalPosition.AvailableRoles
                    //     .Select(r => r.RoleOption).ToList();
                    // Debug.Log("roles available: " + availableRoleOptions.Count + " pos: " + pos);
                    
                    // // Method 3
                    // var roleSelectorDialog = playerItemView.Controller.roleSelectorDialogContainer.GetComponentInChildren<TacticalRoleSelector>();
                    // if (roleSelectorDialog == null)
                    // {
                    //     // spawn dialog
                    //     Instantiate(playerItemView.Controller.roleSelectorDialogPrefab, playerItemView.Controller.roleSelectorDialogContainer.transform);
                    // }
                    // else
                    // {
                    //     // despawn dialog
                    //     Destroy(roleSelectorDialog.gameObject);
                    // }
                }
                else
                {
                    Debug.LogWarning("Role selector dialog is not available");
                }
            }
            else 
            {
                string playerName = playerItemView?.Profile?.Name ?? "Unknown Player";
                Debug.Log(playerName + " clicked!");
                
                // Handle selection/swap
                if (playerItemView?.Controller != null)
                {
                    playerItemView.Controller.HandleItemClicked(playerItemView);
                }
                else
                {
                    Debug.LogWarning("PlayerItemView Controller is not available");
                }
            }
        }

        private void LogSelectionState(string state)
        {
            if (playerItemView != null && playerItemView.Controller != null)
            {
                int teamId = playerItemView.Controller.teamId;
                string playerName = playerItemView.Profile?.Name ?? "Unknown Player";
                Debug.Log($"{state}: TeamID: {teamId} Player: {playerName}");
            }
            else
            {
                Debug.LogWarning("PlayerItemView or Controller is not assigned.");
            }
        }

        #region Selectability Control

        /// <summary>
        /// Sets the selectability state of this player item
        /// </summary>
        /// <param name="selectable">True to enable selectability, false to disable</param>
        public void SetSelectability(bool selectable)
        {
            interactable = selectable;

            if (selectable)
            {
                // Restore normal visual state
                var colors = this.colors;
                colors.disabledColor = colors.normalColor;
                this.colors = colors;
            }
            else
            {
                // Apply disabled visual state (semi-transparent)
                var colors = this.colors;
                colors.disabledColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, 0.5f);
                this.colors = colors;
            }

            string playerName = playerItemView?.Profile?.Name ?? "Unknown Player";
            Debug.Log($"[PlayerItemViewSelectionLayer] Selectability {(selectable ? "enabled" : "disabled")} for {playerName}");
        }

        /// <summary>
        /// Enables selectability for this player item
        /// </summary>
        public void EnableSelectability() => SetSelectability(true);

        /// <summary>
        /// Disables selectability for this player item
        /// </summary>
        public void DisableSelectability() => SetSelectability(false);

        /// <summary>
        /// Toggles the selectability state
        /// </summary>
        /// <param name="forceState">Optional: force a specific state instead of toggling</param>
        public void ToggleSelectability(bool? forceState = null)
        {
            bool newState = forceState ?? !interactable;
            SetSelectability(newState);
        }

        /// <summary>
        /// Gets the current selectability state
        /// </summary>
        public bool IsSelectable => interactable;

        #endregion
    }
}
