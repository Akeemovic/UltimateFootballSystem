using UnityEngine;
using System;
using System.Collections;

namespace UltimateFootballSystem.Gameplay
{
    public class SideSelectionRow : MonoBehaviour
    {
        [Header("Row Configuration")]
        public GamerSide gamerSide;
        [SerializeField] private SideSelectionItem[] sideItems = new SideSelectionItem[3];
        [SerializeField] private float movementSpeed = 5f;

        [Header("Current State")]
        [SerializeField] private SideSelectionItemSide selectedItemSide = SideSelectionItemSide.Center;

        private SideSelectionIndicator indicator;
        private SideSelectionPanel parentPanel;
        private bool isMoving = false;

        public event Action<SideSelectionItemSide> OnStateChanged;
        public SideSelectionItemSide SelectedItemSide => selectedItemSide;

        void Start()
        {
            ValidateSetup();
            Debug.Log($"[SideSelectionRow] Row Start() called for {gamerSide} side");
        }

        public void SetParentPanel(SideSelectionPanel panel)
        {
            parentPanel = panel;
            Debug.Log($"[SideSelectionRow] Parent panel assigned to {gamerSide} row");

            // Initialize indicator immediately after panel is set
            InitializeIndicator();
            SetToSide(selectedItemSide, true);
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

        public void MoveLeft()
        {
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

        public void MoveRight()
        {
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

        public void MoveToSide(SideSelectionItemSide targetSide)
        {
            if (isMoving || targetSide == selectedItemSide || indicator == null) return;

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

            UpdateItemSelection(targetSide);

            float journey = 0f;
            while (journey <= 1f)
            {
                journey += Time.deltaTime * movementSpeed;
                indicator.transform.position = Vector3.Lerp(startPosition, targetPosition, journey);
                yield return null;
            }

            ParentIndicatorToItem(targetItem);
            selectedItemSide = targetSide;
            UpdateGamerSideFromItemSide();
            OnStateChanged?.Invoke(selectedItemSide);
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
    }
}