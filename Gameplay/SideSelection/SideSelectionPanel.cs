using System;
using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Gameplay.Common;
using UnityEngine;
using UnityEngine.Events;
using UltimateFootballSystem.Gameplay.SideSelection;

namespace UltimateFootballSystem.Gameplay.SideSelection
{
    public class SideSelectionPanel : MonoBehaviour
    {
        [Header("Prefab References")]
        [SerializeField] private GameObject indicatorPrefab;
        [SerializeField] private GameObject rowPrefab;

        [Header("Container")]
        [SerializeField] private Transform rowContainer;

        [SerializeField] private List<SideSelectionRow> rows = new List<SideSelectionRow>();
        public GameObject IndicatorPrefab => indicatorPrefab;
        
        // TODO: Check if all gamers side selections are confirmed

        private void Awake()
        {
            // Subscribe to GameStateManager events;
            
            // Change GameState to SideSelection 
            GameStateManager.Instance.SetState(GameState.SideSelection);
        }

        void Start()
        {
            // Ensure we always have exactly 4 rows for the 4 gamers
            EnsureExactly4Rows();

            AssignGamersToRowsChronologically();
            SubscribeToGamerEvents();

            // Check for already active gamers (in case GamerManager initialized first)
            CheckForActiveGamers();
        }

        private void CheckForActiveGamers()
        {
            if (GamerManager.Instance == null) return;

            var activeGamers = GamerManager.Instance.GetActiveGamers();
            Debug.Log($"[SideSelectionPanel] Found {activeGamers.Count} active gamers");
            foreach (var gamer in activeGamers)
            {
                Debug.Log($"[SideSelectionPanel] Found already active Gamer {gamer.GamerId}");
                ActivateRowForGamer(gamer);
            }
        }

        void OnDestroy()
        {
            UnsubscribeFromGamerEvents();
            CleanupRows();
        }

        private void SubscribeToGamerEvents()
        {
            GamerManager.OnGamerActivated += OnGamerActivated;
            GamerManager.OnGamerDeactivated += OnGamerDeactivated;
            Debug.Log("[SideSelectionPanel] Subscribed to GamerManager events");
        }

        private void UnsubscribeFromGamerEvents()
        {
            GamerManager.OnGamerActivated -= OnGamerActivated;
            GamerManager.OnGamerDeactivated -= OnGamerDeactivated;
        }

        // private void OnGamerActivated(Gamer gamer)
        // {
        //     Debug.Log($"[SideSelectionPanel] Gamer {gamer.GamerId} activated - activating row");
        //     ActivateRowForGamer(gamer);
        // }
        private void OnGamerActivated(Gamer gamer)
        {
            Debug.Log($"[SideSelectionPanel] Gamer {gamer.GamerId} activated - activating row");
            Debug.Log($"[SideSelectionPanel] active gamers: {GamerManager.Instance.GetActiveGamers().Count}");
            
            // Ensure we have enough rows
            EnsureEnoughRows();
            
            // Find the correct row for this gamer based on chronological order (GamerId)
            int rowIndex = (int)gamer.GamerId;

            if (rowIndex >= 0 && rowIndex < rows.Count)
            {
                SideSelectionRow row = rows[rowIndex];

                // If row doesn't have this gamer assigned, assign it
                if (row.GetAssignedGamer() != gamer)
                {
                    row.AssignGamer(gamer);
                    Debug.Log($"[SideSelectionPanel] Assigned Gamer {gamer.GamerId} to row {rowIndex}");
                }

                row.gameObject.SetActive(true);
                row.ActivateSideSelection();
                Debug.Log($"[SideSelectionPanel] Row {rowIndex} activated for Gamer {gamer.GamerId}");
            }
            else
            {
                Debug.LogError($"[SideSelectionPanel] Invalid row index {rowIndex} for Gamer {gamer.GamerId}. Available rows: {rows.Count}");
                return;
            }
        }

        private void OnGamerDeactivated(Gamer gamer)
        {
            Debug.Log($"[SideSelectionPanel] Gamer {gamer.GamerId} deactivated - deactivating row");
            DeactivateRowForGamer(gamer);
        }

        private void AssignGamersToRowsChronologically()
        {
            if (GamerManager.Instance == null)
            {
                Debug.LogError("[SideSelectionPanel] GamerManager not found!");
                return;
            }

            var allGamers = GamerManager.Instance.GetGamerList();
            Debug.Log($"[SideSelectionPanel] Assigning {allGamers.Count} gamers to {rows.Count} rows chronologically");

            // Assign gamers to rows based on their GamerId (chronological order)
            for (int i = 0; i < 4; i++)
            {
                if (i < rows.Count)
                {
                    var row = rows[i];
                    row.SetParentPanel(this);
                    row.OnStateChanged += OnRowStateChanged;

                    // Find gamer by ID (Player1=0, Player2=1, etc.)
                    var gamer = allGamers.FirstOrDefault(g => (int)g.GamerId == i);

                    if (gamer != null)
                    {
                        row.AssignGamer(gamer);
                        // Start inactive - will be activated when gamer gets input device
                        bool shouldBeActive = gamer.gameObject.activeSelf;
                        row.gameObject.SetActive(shouldBeActive);
                        Debug.Log($"[SideSelectionPanel] Assigned Gamer {gamer.GamerId} to row {i} (active: {shouldBeActive})");
                    }
                    else
                    {
                        row.gameObject.SetActive(false);
                        Debug.Log($"[SideSelectionPanel] Row {i} initialized without gamer (will be assigned when gamer becomes available)");
                    }
                }
            }
        }

        private void SpawnRowForGamer(Gamer gamer)
        {
            if (rowPrefab == null || rowContainer == null)
            {
                Debug.LogError("[SideSelectionPanel] RowPrefab or RowContainer is null");
                return;
            }

            GameObject rowObj = Instantiate(rowPrefab, rowContainer);
            SideSelectionRow row = rowObj.GetComponent<SideSelectionRow>();

            if (row != null)
            {
                row.SetParentPanel(this);
                row.AssignGamer(gamer);
                row.OnStateChanged += OnRowStateChanged;
                rows.Add(row);

                // Start inactive - will be activated when gamer gets input device
                bool shouldBeActive = gamer != null && gamer.gameObject.activeSelf;
                rowObj.SetActive(shouldBeActive);

                Debug.Log($"[SideSelectionPanel] Row created for Gamer {gamer.GamerId} (active: {shouldBeActive}, gamer active: {gamer.gameObject.activeSelf})");
            }
            else
            {
                Debug.LogError("[SideSelectionPanel] RowPrefab doesn't have SideSelectionRow component");
                Destroy(rowObj);
            }
        }

        private void ActivateRowForGamer(Gamer gamer)
        {
            SideSelectionRow row = rows.Find(r => r.GetAssignedGamer() == gamer);
            if (row != null)
            {
                row.gameObject.SetActive(true);
                row.ActivateSideSelection();
                Debug.Log($"[SideSelectionPanel] Row activated for Gamer {gamer.GamerId}");
            }
        }

        private void DeactivateRowForGamer(Gamer gamer)
        {
            SideSelectionRow row = rows.Find(r => r.GetAssignedGamer() == gamer);
            if (row != null)
            {
                row.DeactivateSideSelection();
                row.gameObject.SetActive(false);
                Debug.Log($"[SideSelectionPanel] Row deactivated for Gamer {gamer.GamerId}");
            }
        }

        private void CleanupRows()
        {
            foreach (var row in rows)
            {
                if (row != null)
                {
                    row.OnStateChanged -= OnRowStateChanged;
                }
            }
        }



        private void OnRowStateChanged(SideSelectionItemSide newSide)
        {
            Debug.Log($"[SideSelectionPanel] Row side changed to: {newSide}");
        }

        public List<SideSelectionRow> GetAllRows() => rows;

        // Method to manually trigger row assignment if GamerManager wasn't ready at Start
        public void RefreshRows()
        {
            CleanupRows();
            AssignGamersToRowsChronologically();
            Debug.Log($"[SideSelectionPanel] Rows refreshed - now have {rows.Count} rows");
        }
        
        // Ensure we have exactly 4 rows for the 4 gamers
        private void EnsureExactly4Rows()
        {
            const int REQUIRED_ROWS = 4;

            // If we have fewer than 4 rows, create them
            while (rows.Count < REQUIRED_ROWS)
            {
                SpawnNewRow();
            }

            // If we have more than 4 rows, warn (shouldn't happen but handle gracefully)
            if (rows.Count > REQUIRED_ROWS)
            {
                Debug.LogWarning($"[SideSelectionPanel] Found {rows.Count} rows, but only 4 are needed. Extra rows will be ignored.");
            }

            Debug.Log($"[SideSelectionPanel] Ensured exactly {Math.Min(rows.Count, REQUIRED_ROWS)} rows are available");
        }

        private void SpawnNewRow()
        {
            if (rowPrefab == null || rowContainer == null)
            {
                Debug.LogError("[SideSelectionPanel] RowPrefab or RowContainer is null - cannot spawn row");
                return;
            }

            GameObject rowObj = Instantiate(rowPrefab, rowContainer);
            SideSelectionRow row = rowObj.GetComponent<SideSelectionRow>();

            if (row != null)
            {
                rows.Add(row);
                rowObj.SetActive(false); // Start inactive
                Debug.Log($"[SideSelectionPanel] Spawned new row (total: {rows.Count})");
            }
            else
            {
                Debug.LogError("[SideSelectionPanel] RowPrefab doesn't have SideSelectionRow component");
                Destroy(rowObj);
            }
        }

        // Legacy method name for compatibility
        private void EnsureEnoughRows()
        {
            EnsureExactly4Rows();
        }
    }
}