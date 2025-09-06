// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
//
// namespace UltimateFootballSystem.Gameplay.Tactics
// {
//     public class PlayerItemViewSelectionLayer : Selectable, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
//     {
//         [SerializeField] 
//         private PlayerItemView playerItemView;
//
//         private TacticsBoardController _controller;
//         private ScrollRect parentScrollRect;
//         private Vector2 pointerDownPos;
//         private float pointerDownTime;
//         private bool isDragging = false;
//         private bool isPointerClick = false;
//         private const float MAX_TAP_TIME = 0.2f;
//         private const float MAX_TAP_DISTANCE = 10f;
//
//         // Lazy initialization for controller
//         private TacticsBoardController controller
//         {
//             get
//             {
//                 if (_controller == null && playerItemView != null)
//                 {
//                     _controller = playerItemView.Controller;
//                 }
//                 return _controller;
//             }
//         }
//
//         protected override void Awake()
//         {
//             base.Awake();
//             
//             // Try to get playerItemView if not set in inspector
//             if (playerItemView == null)
//             {
//                 playerItemView = GetComponent<PlayerItemView>();
//             }
//             
//             parentScrollRect = GetComponentInParent<ScrollRect>();
//
//             if (Application.isMobilePlatform)
//             {
//                 transition = Transition.None;
//             }
//         }
//
//         public override void OnSelect(BaseEventData eventData)
//         {
//             base.OnSelect(eventData);
//             
//             // Only play selection sound if this wasn't triggered by a pointer click
//             if (!isPointerClick && eventData is not PointerEventData && controller != null)
//             {
//                 controller.PlaySelectSound();
//             }
//             
//             LogSelectionState("Selected");
//         }
//
//         public void OnBeginDrag(PointerEventData eventData)
//         {
//             isDragging = true;
//             if (parentScrollRect != null)
//             {
//                 parentScrollRect.OnBeginDrag(eventData);
//             }
//         }
//
//         public void OnDrag(PointerEventData eventData)
//         {
//             if (parentScrollRect != null)
//             {
//                 parentScrollRect.OnDrag(eventData);
//             }
//         }
//
//         public void OnEndDrag(PointerEventData eventData)
//         {
//             isDragging = false;
//             if (parentScrollRect != null)
//             {
//                 parentScrollRect.OnEndDrag(eventData);
//             }
//         }
//
//         public override void OnPointerDown(PointerEventData eventData)
//         {
//             base.OnPointerDown(eventData);
//             pointerDownPos = eventData.position;
//             pointerDownTime = Time.time;
//             isPointerClick = true;
//         }
//
//         public override void OnPointerUp(PointerEventData eventData)
//         {
//             base.OnPointerUp(eventData);
//             isPointerClick = false;
//         }
//
//         public void OnPointerClick(PointerEventData eventData)
//         {
//             // Check if this was a tap (not a drag)
//             if (Time.time - pointerDownTime <= MAX_TAP_TIME && 
//                 Vector2.Distance(eventData.position, pointerDownPos) <= MAX_TAP_DISTANCE &&
//                 !isDragging)
//             {
//                 HandleItemClick();
//             }
//         }
//
//         private void HandleItemClick()
//         {
//             // Check if controller is available
//             if (controller == null)
//             {
//                 Debug.LogWarning("Controller is not available yet");
//                 return;
//             }
//             
//             // Play click sound
//             controller.PlayClickSound();
//             
//             // Check if we should show role dialog
//             if (playerItemView != null && 
//                 playerItemView.mainView != null &&
//                 playerItemView.mainView.ViewMode == PlayerItemViewModeOption.Roles && 
//                 playerItemView.HasPlayerItem)
//             {
//                 if (controller.roleSelectorDialog != null)
//                 {
//                     var dialog = controller.roleSelectorDialog.Clone();
//                     dialog.Show();
//                     Debug.Log("Showing role dialog", dialog);
//                 }
//                 else
//                 {
//                     Debug.LogWarning("Role selector dialog is not set");
//                 }
//             }
//             else
//             {
//                 // Handle selection/swap
//                 controller.HandleItemClicked(playerItemView);
//             }
//         }
//
//         private void LogSelectionState(string state)
//         {
//             if (playerItemView != null && controller != null)
//             {
//                 int teamId = controller.teamId;
//                 Debug.Log($"{state}: TeamID: {teamId}, Player: {playerItemView.Profile?.Name ?? "Empty"}");
//             }
//         }
//     }
// }

// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
//
// namespace UltimateFootballSystem.Gameplay.Tactics
// {
//     public class PlayerItemViewSelectionLayer : Selectable, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
//     {
//         [SerializeField] 
//         private PlayerItemView playerItemView;
//
//         private TacticsBoardController _controller;
//         private ScrollRect parentScrollRect;
//         private Vector2 pointerDownPos;
//         private float pointerDownTime;
//         private bool isDragging = false;
//         private bool isPointerClick = false;
//         private bool hasStartedDrag = false; // Track if we've started a drag
//         private const float MAX_TAP_TIME = 0.2f;
//         private const float MAX_TAP_DISTANCE = 10f;
//
//         // Lazy initialization for controller
//         private TacticsBoardController controller
//         {
//             get
//             {
//                 if (_controller == null && playerItemView != null)
//                 {
//                     _controller = playerItemView.Controller;
//                 }
//                 return _controller;
//             }
//         }
//
//         protected override void Awake()
//         {
//             base.Awake();
//             
//             // Try to get playerItemView if not set in inspector
//             if (playerItemView == null)
//             {
//                 playerItemView = GetComponent<PlayerItemView>();
//             }
//             
//             parentScrollRect = GetComponentInParent<ScrollRect>();
//
//             if (Application.isMobilePlatform)
//             {
//                 transition = Transition.None;
//             }
//         }
//
//         public override void OnSelect(BaseEventData eventData)
//         {
//             base.OnSelect(eventData);
//             
//             // Only play selection sound if this wasn't triggered by a pointer click
//             if (!isPointerClick && eventData is not PointerEventData && controller != null)
//             {
//                 controller.PlaySelectSound();
//             }
//             
//             LogSelectionState("Selected");
//         }
//
//         public void OnBeginDrag(PointerEventData eventData)
//         {
//             isDragging = true;
//             hasStartedDrag = true;
//             
//             // Pass through to scroll rect
//             if (parentScrollRect != null)
//             {
//                 parentScrollRect.OnBeginDrag(eventData);
//             }
//             
//             // Let the drag support handle the drag naturally
//             // It will receive this event through Unity's event system
//         }
//
//         public void OnDrag(PointerEventData eventData)
//         {
//             // Pass through to scroll rect
//             if (parentScrollRect != null)
//             {
//                 parentScrollRect.OnDrag(eventData);
//             }
//             
//             // Let the drag support handle the drag naturally
//         }
//
//         public void OnEndDrag(PointerEventData eventData)
//         {
//             isDragging = false;
//             
//             // Pass through to scroll rect
//             if (parentScrollRect != null)
//             {
//                 parentScrollRect.OnEndDrag(eventData);
//             }
//             
//             // Let the drag support handle the drag naturally
//         }
//
//         public override void OnPointerDown(PointerEventData eventData)
//         {
//             base.OnPointerDown(eventData);
//             pointerDownPos = eventData.position;
//             pointerDownTime = Time.time;
//             isPointerClick = true;
//             hasStartedDrag = false;
//         }
//
//         public override void OnPointerUp(PointerEventData eventData)
//         {
//             base.OnPointerUp(eventData);
//             isPointerClick = false;
//         }
//
//         public void OnPointerClick(PointerEventData eventData)
//         {
//             // Only handle click if we haven't started dragging
//             if (!hasStartedDrag &&
//                 Time.time - pointerDownTime <= MAX_TAP_TIME && 
//                 Vector2.Distance(eventData.position, pointerDownPos) <= MAX_TAP_DISTANCE)
//             {
//                 HandleItemClick();
//             }
//         }
//
//         private void HandleItemClick()
//         {
//             // Check if controller is available
//             if (controller == null)
//             {
//                 Debug.LogWarning("Controller is not available yet");
//                 return;
//             }
//             
//             // Play click sound
//             controller.PlayClickSound();
//             
//             // Check if we should show role dialog
//             if (playerItemView != null && 
//                 playerItemView.mainView != null &&
//                 playerItemView.mainView.ViewMode == PlayerItemViewModeOption.Roles && 
//                 playerItemView.HasPlayerItem)
//             {
//                 if (controller.roleSelectorDialog != null)
//                 {
//                     var dialog = controller.roleSelectorDialog.Clone();
//                     dialog.Show();
//                     Debug.Log("Showing role dialog", dialog);
//                 }
//                 else
//                 {
//                     Debug.LogWarning("Role selector dialog is not set");
//                 }
//             }
//             else
//             {
//                 // Handle selection/swap
//                 controller.HandleItemClicked(playerItemView);
//             }
//         }
//
//         private void LogSelectionState(string state)
//         {
//             if (playerItemView != null && controller != null)
//             {
//                 int teamId = controller.teamId;
//                 Debug.Log($"{state}: TeamID: {teamId}, Player: {playerItemView.Profile?.Name ?? "Empty"}");
//             }
//         }
//     }
// }

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
        }

        public override void OnSelect(BaseEventData eventData)
        {
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
            if (!isDragging)
            {
                HandleItemSelection();
            }
        }

        private void HandleItemSelection()
        {
            if (playerItemView?.mainView?.ViewMode == PlayerItemViewModeOption.Roles)
            {
                if (playerItemView.Controller?.roleSelectorDialog != null)
                {
                    var dialog = playerItemView.Controller.roleSelectorDialog.Clone();
                    dialog.Show();
                    Debug.Log("dialog", dialog);
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

    
        // ... (any other existing methods)
    }
}
