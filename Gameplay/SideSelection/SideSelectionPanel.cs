using UnityEngine;
using UnityEngine.Events;

namespace UltimateFootballSystem.Gameplay
{
    public class SideSelectionPanel : MonoBehaviour
    {
        [Header("Prefab References")]
        [SerializeField] private GameObject indicatorPrefab;

        [Header("Rows")]
        [SerializeField] private SideSelectionRow[] rows;

        [Header("Current Active Row")]
        [SerializeField] private int activeRowIndex = 0;

        [Header("Events")]
        public UnityEvent<SideSelectionRow> OnRowSelectionChanged;

        public GameObject IndicatorPrefab => indicatorPrefab;
        private SideSelectionRow ActiveRow => (rows != null && activeRowIndex >= 0 && activeRowIndex < rows.Length) ? rows[activeRowIndex] : null;

        void Start()
        {
            InitializePanel();
            Debug.Log($"[SideSelectionPanel] Panel initialized with {rows?.Length ?? 0} rows");
        }

        void Update()
        {
            HandleInput();
        }

        private void InitializePanel()
        {
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    if (row != null)
                    {
                        row.SetParentPanel(this);
                        row.OnStateChanged += OnRowStateChanged;
                    }
                }
            }

            SetActiveRow(activeRowIndex);
        }

        private void HandleInput()
        {
            if (ActiveRow == null) return;

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log($"[SideSelectionPanel] Left input detected");
                ActiveRow.MoveLeft();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log($"[SideSelectionPanel] Right input detected");
                ActiveRow.MoveRight();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                SwitchToRow(activeRowIndex - 1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                SwitchToRow(activeRowIndex + 1);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                SelectCurrentRow();
            }
        }

        private void SwitchToRow(int newRowIndex)
        {
            if (rows == null || newRowIndex < 0 || newRowIndex >= rows.Length) return;

            Debug.Log($"[SideSelectionPanel] Switching from row {activeRowIndex} to row {newRowIndex}");
            SetActiveRow(newRowIndex);
        }

        private void SetActiveRow(int rowIndex)
        {
            if (rows == null || rowIndex < 0 || rowIndex >= rows.Length) return;

            activeRowIndex = rowIndex;
            Debug.Log($"[SideSelectionPanel] Active row set to: {activeRowIndex} ({rows[activeRowIndex]?.gamerSide})");
        }

        private void OnRowStateChanged(SideSelectionItemSide newSide)
        {
            Debug.Log($"[SideSelectionPanel] Row {activeRowIndex} side changed to: {newSide}");
        }

        private void SelectCurrentRow()
        {
            if (ActiveRow != null)
            {
                Debug.Log($"[SideSelectionPanel] Row selected: {ActiveRow.gamerSide} at side {ActiveRow.SelectedItemSide}");
                OnRowSelectionChanged?.Invoke(ActiveRow);
            }
        }

        public SideSelectionRow GetActiveRow() => ActiveRow;
        public SideSelectionRow[] GetAllRows() => rows;
        public SideSelectionItemSide GetActiveRowSide() => ActiveRow?.SelectedItemSide ?? SideSelectionItemSide.Center;

        void OnDestroy()
        {
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    if (row != null)
                    {
                        row.OnStateChanged -= OnRowStateChanged;
                    }
                }
            }
        }
    }
}