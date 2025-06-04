using UIWidgets;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    /// <summary>
    /// Player Profile drag.
    /// </summary>
    [RequireComponent(typeof(PlayerItemView))]
    public class PlayerItemDragSupport :  DragSupport<PlayerItemDragData>
    {
        /// <summary>
        /// The drag info. IMPORTANT: Must NOT be a Raycast target.
        /// </summary>
        [SerializeField]
        public PlayerItemView DragInfoView;

        /// <summary>
        /// DragInfoView offset.
        /// </summary>
        [SerializeField]
        // public Vector3 DragInfoViewOffset = new Vector3(-5, 5, 0);
        public Vector3 DragInfoViewOffset = new Vector3(-10, 5, 0);
    
        PlayerItemView source;

        /// <summary>
        /// Source component.
        /// </summary>
        public PlayerItemView Source
        {
            get
            {
                if (source == null)
                {
                    source = GetComponent<PlayerItemView>();
                }

                return source;
            }
        }

        // Start is called before the first frame update
        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();
        
            // DragInfoView = TacticsBoardController.Instance.DragInfoView;
            DragInfoView = Source.Controller.dragInfoView;
            // Disable raycast target on tacticsPitch
            // to prevent it from blocking pointer raycast 
            DragInfoView.GetComponent<Image>().raycastTarget = false;

            // using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            // {
            if (DragInfoView != null)
            {
                if (DragInfoView.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                {
                    DragInfoView = null;
                    Debug.LogWarning("DragInfoView cannot be same gameobject as DragSupport.", this);
                }
                else
                {
                    DragInfoView.gameObject.SetActive(false);
                }
            }
            // }

            if (Source.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList)
            {
                DragDelay = 0.1f;
            }
            RedirectDragToScrollRect = true;
        }
    
        // Update is called once per frame
        void Update()
        {
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnMouseDown()
        {
            Debug.Log("OnMouseDown");
        }

        /// <inheritdoc/>
        protected override void InitDrag(PointerEventData eventData)
        {
            // Source.DisableRecycling = true;
            Data = Source.GetDragData();
            // Data.DragSourceViewReference = Source;
            Data.SetDragSourceViewReference(Source);

            // Null players cannot be moved around unless owned starting list
            if (!Data.IsValidPlayer() && Data.DragSourceView.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList) { return; }
        
            // Debug.Log(
            //     "SetDragSourceViewReference position is: " + Data.DragSourceView.ParentPositionZoneView.tacticalPositionOption);
            // Debug.Log(
            //     "Source position is: " + Source.TacticalPositionOption);
        
            // TacticalPosition = Source.TacticalPosition;
            // Debug.Log("Drag");
            // Debug.Log(Data.Profile.Name);

            // Apply fade effect
            source.FadeMainView();
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                ShowDragInfo();

                // Don't show unless startingList views are being dragged
                if (Source.ViewOwnerOption != PlayerItemViewOwnerOption.StartingList) return;
                // TacticsBoardController.Instance.View.ShowUsablePlayerItemViews();
                Source.Controller.tacticsPitch.ShowUsablePlayerItemViews();
                // TacticsPitch.ShowUsablePlayerItemViews(TacticsBoardController.Instance.zoneContainerViews);
            }
        }



        /// <summary>
        /// Shows the drag info.
        /// </summary>
        protected virtual void ShowDragInfo()
        {
            if (DragInfoView == null)
            {
                Debug.Log("DragInfoView is null");
                return;
            }
    
            DragInfoView.transform.SetParent(DragPoint, false);
            DragInfoView.transform.localPosition = DragInfoViewOffset;
    
    
            DragInfoView.SetPlayerData(Data.Profile);
            DragInfoView.gameObject.SetActive(true);
        }
    
        /// <summary>
        /// Hides the drag info.
        /// </summary>
        protected virtual void HideDragInfo()
        {
            if (DragInfoView == null)
            {
                return;
            }

            DragInfoView.gameObject.SetActive(false);
        }

        /// <inheritdoc/>
        public override void Dropped(bool success)
        {
            HideDragInfo();

            // Source.DisableRecycling = false;

            Data = PlayerItemDragData.Empty();
        
            // Remove fade effect
            source.BrightenMainView();

            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                // TacticsBoardController.Instance.View.HideUnusedPlayerItemViews();
                Source.Controller.tacticsPitch.HideUnusedPlayerItemViews();
                // TacticsPitch.HideUnusedPlayerItemViews(TacticsBoardController.Instance.zoneContainerViews);
            }
        }
    }
}
