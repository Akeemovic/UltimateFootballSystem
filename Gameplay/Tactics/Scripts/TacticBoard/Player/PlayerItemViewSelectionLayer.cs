using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PlayerItemViewSelectionLayer : Selectable, IPointerClickHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] 
        private PlayerItemView playerItemView;

        [SerializeField] 
        private AudioClip selectAudioClip;

        [SerializeField] 
        private AudioClip clickAudioClip;

        private AudioSource audioSource;
        private ScrollRect parentScrollRect;
        private Vector2 pointerDownPos;
        private float pointerDownTime;
        private bool isDragging = false;
        private bool isClickHandled = false;
        private const float MAX_TAP_TIME = 0.2f;
        private const float MAX_TAP_DISTANCE = 10f;

        protected override void Awake()
        {
            base.Awake();

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
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
        
            if (!isClickHandled && !Application.isMobilePlatform)
            {
                PlaySelectionSound();
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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.time - pointerDownTime <= MAX_TAP_TIME && 
                Vector2.Distance(eventData.position, pointerDownPos) <= MAX_TAP_DISTANCE &&
                !isDragging)
            {
                isClickHandled = true;
                HandleItemSelection();
                PlayClickSound();
            }
        }

        private void HandleItemSelection()
        {
            if (playerItemView.mainView.ViewMode == PlayerItemViewModeOption.Roles)
            {
                var dialog = playerItemView.Controller.roleSelectorDialog.Clone();
                dialog.Show();
                Debug.Log("dialog", dialog);
            }
            else 
            {
                Debug.Log(playerItemView.Profile.Name + " clicked!");
                if (playerItemView.ViewOwnerOption == PlayerItemViewOwnerOption.BenchList &&
                    playerItemView.HasPlayerItem)
                {
                    var dragData = playerItemView.GetDragData();
                    dragData.SetDragSourceViewReference(playerItemView);
                    // Implement your logic for removing substitute here
                }
            }
        }

        private void PlaySelectionSound()
        {
            if (selectAudioClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(selectAudioClip);
            }
        }

        private void PlayClickSound()
        {
            if (clickAudioClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clickAudioClip);
            }
        }

        private void LogSelectionState(string state)
        {
            if (playerItemView != null && playerItemView.Controller != null)
            {
                int teamId = playerItemView.Controller.teamId;
                Debug.Log($"{state}: TeamID: {teamId}");
            }
            else
            {
                Debug.LogWarning("PlayerItemView or Controller is not assigned.");
            }
        }

    
        // ... (any other existing methods)
    }
}