using UnityEngine;
using System;
using System.Collections;
using UltimateFootballSystem.Gameplay.Common;
using UltimateFootballSystem.Gameplay.InputManagement;
using UnityEngine.InputSystem;

namespace UltimateFootballSystem.Gameplay.SideSelection
{
    public class SideSelectionRow : MonoBehaviour
    {
        [Header("Row Configuration")]
        public bool isGamerSideConfirmed;
        public GamerSide gamerSide;
        [SerializeField] private SideSelectionItem[] sideItems = new SideSelectionItem[3];
        [SerializeField] private float movementSpeed = 5f;

        [Header("Current State")]
        [SerializeField] private SideSelectionItemSide selectedItemSide = SideSelectionItemSide.Center;

        [Header("Gamer Assignment")]
        [SerializeField] private Gamer assignedGamer;

        [Header("Interactivity")]
        [SerializeField] private bool isInteractive = false;

        private SideSelectionIndicator indicator;
        private SideSelectionPanel parentPanel;
        private SideSelectionInputHandler inputHandler;
        private bool isMoving = false;

        public event Action<SideSelectionItemSide> OnStateChanged;
        public SideSelectionItemSide SelectedItemSide => selectedItemSide;
        public bool IsInteractive => isInteractive;

        void Start()
        {
            ValidateSetup();
            InitializeInputHandler();
            Debug.Log($"[SideSelectionRow] Row Start() called for {gamerSide} side");
        }

        void OnDestroy()
        {
            CleanupInputHandler();
        }

        public void SetParentPanel(SideSelectionPanel panel)
        {
            parentPanel = panel;
            Debug.Log($"[SideSelectionRow] Parent panel assigned to {gamerSide} row");

            // Initialize indicator immediately after panel is set
            InitializeIndicator();
            SetToSide(selectedItemSide, true);
        }

        public void AssignGamer(Gamer gamer)
        {
            assignedGamer = gamer;
            gamerSide = gamer.GamerSide;

            // Reinitialize input handler with new gamer
            CleanupInputHandler();
            InitializeInputHandler();

            // Enable interactivity when gamer is assigned
            EnableInteractivity();

            Debug.Log($"[SideSelectionRow] Gamer {gamer.GamerId} assigned to row and interactivity enabled");
        }

        public void UnassignGamer()
        {
            if (assignedGamer != null)
            {
                Debug.Log($"[SideSelectionRow] Gamer {assignedGamer.GamerId} unassigned from row");
            }

            // Cleanup input handler
            CleanupInputHandler();

            // Disable interactivity when gamer is unassigned
            DisableInteractivity();

            // Clear gamer reference
            assignedGamer = null;
            isGamerSideConfirmed = false;

            Debug.Log($"[SideSelectionRow] Gamer unassigned and interactivity disabled");
        }

        private void InitializeInputHandler()
        {
            if (assignedGamer == null)
            {
                Debug.LogWarning("[SideSelectionRow] Cannot initialize input - no gamer assigned");
                return;
            }

            if (assignedGamer.InputController == null)
            {
                Debug.LogWarning($"[SideSelectionRow] Cannot initialize input - Gamer {assignedGamer.GamerId} has no InputController");
                return;
            }

            // Ensure the input controller is in the correct state
            assignedGamer.InputController.SetState(GameState.SideSelection);

            inputHandler = assignedGamer.InputController.SideSelectionHandler;

            if (inputHandler == null)
            {
                Debug.LogWarning($"[SideSelectionRow] Cannot initialize input - InputController has no SideSelectionHandler");
                return;
            }

            // Subscribe to input events
            inputHandler.OnMoveLeft += HandleMoveLeft;
            inputHandler.OnMoveRight += HandleMoveRight;
            inputHandler.OnSubmitSelection += HandleSubmitSelection;
            inputHandler.OnCancelSelection += HandleCancelSelection;
            // inputHandler.OnChangeInputDevice += HandleChangeInputDevice;
            inputHandler.OnChangeInputDevice += HandleChangeInputDevice;
            
            Debug.Log($"[SideSelectionRow] Input handler successfully initialized for Gamer {assignedGamer.GamerId}");
        }

        private void CleanupInputHandler()
        {
            if (inputHandler != null)
            {
                inputHandler.OnMoveLeft -= HandleMoveLeft;
                inputHandler.OnMoveRight -= HandleMoveRight;
                inputHandler.OnSubmitSelection -= HandleSubmitSelection;
                inputHandler.OnCancelSelection -= HandleCancelSelection;
            }
        }

        private void HandleMoveLeft()
        {
            if (!isInteractive)
            {
                Debug.Log($"[SideSelectionRow] Movement blocked - row not interactive for Gamer {assignedGamer?.GamerId}");
                return;
            }

            Debug.Log($"[SideSelectionRow] HandleMoveLeft called for Gamer {assignedGamer?.GamerId}");
            MoveLeft();
        }

        private void HandleMoveRight()
        {
            if (!isInteractive)
            {
                Debug.Log($"[SideSelectionRow] Movement blocked - row not interactive for Gamer {assignedGamer?.GamerId}");
                return;
            }

            Debug.Log($"[SideSelectionRow] HandleMoveRight called for Gamer {assignedGamer?.GamerId}");
            MoveRight();
        }

        private void HandleSubmitSelection()
        {
            if (!isInteractive)
            {
                Debug.Log($"[SideSelectionRow] Submit blocked - row not interactive for Gamer {assignedGamer?.GamerId}");
                return;
            }

            ConfirmSideSelection();
        }

        private void HandleCancelSelection()
        {
            if (!isInteractive)
            {
                Debug.Log($"[SideSelectionRow] Cancel blocked - row not interactive for Gamer {assignedGamer?.GamerId}");
                return;
            }

            CancelSideSelection();
        }

        // private void HandleChangeInputDevice(InputDevice obj)
        // { 
        //     Debug.Log($"[SideSelectionRow] HandleChangeInputDevice called for Gamer {assignedGamer?.GamerId}");
        //     if (obj is Keyboard || obj is Mouse)
        //     {
        //         indicator.ChangeDeviceTypeToKeyboardMouse();
        //     }
        //     else if (obj is Gamepad)
        //     {
        //         indicator.ChangeDeviceTypeToGamepad();
        //     }
        // }
        
        private void HandleChangeInputDevice()
        { 
            if (assignedGamer == null)
            {
                Debug.LogWarning("[SideSelectionRow] HandleChangeInputDevice called but no gamer assigned");
                return;
            }
            
            GamerManager.Instance.HandleChangeInputDevice(assignedGamer);
            Debug.Log($"[SideSelectionRow] HandleChangeInputDevice called for Gamer {assignedGamer?.GamerId}");
            if (assignedGamer.Input.devices[0] is Keyboard || assignedGamer.Input.devices[0] is Mouse)
            {
                indicator.ChangeDeviceTypeToKeyboardMouse();
            }
            else if (assignedGamer.Input.devices[0] is Gamepad)
            {
                indicator.ChangeDeviceTypeToGamepad();
            }
        }

        public void ConfirmSideSelection()
        {
            if (assignedGamer != null && !isGamerSideConfirmed)
            {
                assignedGamer.SetGamerSide(gamerSide);
                isGamerSideConfirmed = true;

                // Stop any ongoing movement
                StopAllCoroutines();
                isMoving = false;

                // Change visual to green and disable movement
                if (indicator != null)
                {
                    indicator.SetConfirmedColor();
                }

                // Keep indicator visible but disable movement
                Debug.Log($"[SideSelectionRow] Gamer {assignedGamer.GamerId} CONFIRMED side: {gamerSide}");
            }
        }

        public void CancelSideSelection()
        {
            if (assignedGamer != null && isGamerSideConfirmed)
            {
                isGamerSideConfirmed = false;

                // Restore normal visual
                if (indicator != null)
                {
                    indicator.SetInitialColor();
                }

                Debug.Log($"[SideSelectionRow] Gamer {assignedGamer.GamerId} CANCELED - can change side");
            }
        }

        // Enable row interactivity: allow movement and input. Usually called when selection is not confirmed or when row is assigned
        public void EnableInteractivity()
        {
            isInteractive = true;

            // Show indicator
            if (indicator != null)
            {
                indicator.SetActive(true);
            }

            // Restore full opacity for side items
            foreach (var item in sideItems)
            {
                if (item != null)
                {
                    var itemImage = item.GetComponent<UnityEngine.UI.Image>();
                    if (itemImage != null)
                    {
                        var color = itemImage.color;
                        color.a = 1f; // Full opacity
                        itemImage.color = color;
                    }
                }
            }

            Debug.Log($"[SideSelectionRow] Interactivity ENABLED for Gamer {assignedGamer?.GamerId}");
        }

        // Disable row interactivity: prevent movement and input. Usually called when selection is confirmed or when row is unassigned
        public void DisableInteractivity()
        {
            isInteractive = false;

            // Hide indicator
            if (indicator != null)
            {
                indicator.SetActive(false);
            }

            // Dim the side items to show they're not interactive
            foreach (var item in sideItems)
            {
                if (item != null)
                {
                    var itemImage = item.GetComponent<UnityEngine.UI.Image>();
                    if (itemImage != null)
                    {
                        var color = itemImage.color;
                        color.a = 0.3f; // Make semi-transparent
                        itemImage.color = color;
                    }
                }
            }

            Debug.Log($"[SideSelectionRow] Interactivity DISABLED for Gamer {assignedGamer?.GamerId}");
        }

        // Deactivate row: disable interactivity and unassign gamer. Usually called when row is unassigned to a gamer
        public void DeactivateSideSelection()
        {
            DisableInteractivity();
            HideIndicator();
        }

        // Activate row: enable interactivity. Usually called when row is assigned to a gamer
        public void ActivateSideSelection()
        {
            EnableInteractivity();
            ShowIndicator();
        }

        private void InitializeIndicator()
        {
            if (parentPanel != null && parentPanel.IndicatorPrefab != null)
            {
                GameObject indicatorObj = Instantiate(parentPanel.IndicatorPrefab, transform);
                indicator = indicatorObj.GetComponent<SideSelectionIndicator>();
                Debug.Log($"[SideSelectionRow] Indicator instantiated for {gamerSide} row");
            }
            else
            {
                Debug.LogError($"[SideSelectionRow] Cannot initialize indicator - parentPanel: {parentPanel != null}, prefab: {parentPanel?.IndicatorPrefab != null}");
            }
        }
        
        private void ShowIndicator ()  
        {
            if (indicator != null) indicator.SetActive(true);
        }
        
        private void HideIndicator ()
        {
            if (indicator != null) indicator.SetActive(false);
        }

        private void MoveLeft()
        {
            if (isGamerSideConfirmed)
            {
                Debug.Log($"[SideSelectionRow] Movement blocked - side already confirmed");
                return;
            }

            SideSelectionItemSide newSide = selectedItemSide switch
            {
                SideSelectionItemSide.Right => SideSelectionItemSide.Center,
                SideSelectionItemSide.Center => SideSelectionItemSide.Left,
                SideSelectionItemSide.Left => SideSelectionItemSide.Left,
                _ => SideSelectionItemSide.Left
            };
            Debug.Log($"[SideSelectionRow] {gamerSide} moving left: {selectedItemSide} -> {newSide}");
            MoveToSide(newSide);
        }

        private void MoveRight()
        {
            if (isGamerSideConfirmed)
            {
                Debug.Log($"[SideSelectionRow] Movement blocked - side already confirmed");
                return;
            }

            SideSelectionItemSide newSide = selectedItemSide switch
            {
                SideSelectionItemSide.Left => SideSelectionItemSide.Center,
                SideSelectionItemSide.Center => SideSelectionItemSide.Right,
                SideSelectionItemSide.Right => SideSelectionItemSide.Right,
                _ => SideSelectionItemSide.Right
            };
            Debug.Log($"[SideSelectionRow] {gamerSide} moving right: {selectedItemSide} -> {newSide}");
            MoveToSide(newSide);
        }

        private void MoveToSide(SideSelectionItemSide targetSide)
        {
            if (!isInteractive || isMoving || targetSide == selectedItemSide || indicator == null || isGamerSideConfirmed) return;

            Debug.Log($"[SideSelectionRow] {gamerSide} moving to side: {targetSide}");
            StartCoroutine(MoveIndicatorCoroutine(targetSide));
        }

        private void SetToSide(SideSelectionItemSide side, bool immediate = false)
        {
            if (sideItems[(int)side] == null || indicator == null) return;

            UpdateItemSelection(side);

            if (immediate)
            {
                ParentIndicatorToItem(sideItems[(int)side]);
            }

            selectedItemSide = side;
            UpdateGamerSideFromItemSide();
            OnStateChanged?.Invoke(selectedItemSide);
            Debug.Log($"[SideSelectionRow] {gamerSide} side set to: {side}, GamerSide: {gamerSide}");
        }

        private void ParentIndicatorToItem(SideSelectionItem targetItem)
        {
            indicator.transform.SetParent(targetItem.transform);
            (indicator.transform as RectTransform).anchoredPosition = Vector2.zero;
            Debug.Log($"[SideSelectionRow] Indicator parented to {targetItem.ItemSide} item");
        }

        private void UpdateItemSelection(SideSelectionItemSide selectedSide)
        {
            for (int i = 0; i < sideItems.Length; i++)
            {
                if (sideItems[i] != null)
                {
                    sideItems[i].SetSelected(i == (int)selectedSide);
                }
            }
        }

        private void UpdateGamerSideFromItemSide()
        {
            gamerSide = selectedItemSide switch
            {
                SideSelectionItemSide.Left => GamerSide.Home,
                SideSelectionItemSide.Center => GamerSide.Neutral,
                SideSelectionItemSide.Right => GamerSide.Away,
                _ => GamerSide.Neutral
            };
        }

        private IEnumerator MoveIndicatorCoroutine(SideSelectionItemSide targetSide)
        {
            isMoving = true;
            SideSelectionItem targetItem = sideItems[(int)targetSide];

            Vector3 startPosition = indicator.transform.position;
            Vector3 targetPosition = targetItem.transform.position;

            // Update selection state immediately when movement starts
            selectedItemSide = targetSide;
            UpdateGamerSideFromItemSide();

            // Update gamer side immediately during movement
            if (assignedGamer != null)
            {
                assignedGamer.SetGamerSide(gamerSide);
            }

            OnStateChanged?.Invoke(selectedItemSide);

            float journey = 0f;
            while (journey <= 1f)
            {
                journey += Time.deltaTime * movementSpeed;
                indicator.transform.position = Vector3.Lerp(startPosition, targetPosition, journey);
                yield return null;
            }

            // Update visual selection only after indicator arrives
            UpdateItemSelection(targetSide);
            ParentIndicatorToItem(targetItem);
            isMoving = false;

            Debug.Log($"[SideSelectionRow] {gamerSide} movement completed to {targetSide}, GamerSide: {gamerSide}");
        }

        private void ValidateSetup()
        {
            for (int i = 0; i < sideItems.Length; i++)
            {
                if (sideItems[i] == null)
                {
                    Debug.LogWarning($"[SideSelectionRow] Side item {i} is null in {gamerSide} row!");
                }
            }
        }

        public SideSelectionItemSide GetSelectedItemSide() => selectedItemSide;
        public GamerSide GetGamerSide() => gamerSide;
        public SideSelectionIndicator GetIndicator() => indicator;
        public Gamer GetAssignedGamer() => assignedGamer;
    }
}