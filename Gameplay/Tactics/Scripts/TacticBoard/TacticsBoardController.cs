using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UltimateFootballSystem.Core.Utils;
using UnityEngine;
using Lean.Pool;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    [RequireComponent(typeof(AudioSource))]
    public class TacticsBoardController : MonoBehaviour
    {
        public Canvas canvas;
        
        [HideInInspector]
        public PlayerItemView dragInfoView;
        
        [SerializeField] 
        public Transform playerItemViewPrefab;
        
        public TacticsPitch tacticsPitch;

        [HideInInspector] 
        public PositionZonesContainerView[] zoneContainerViews = new PositionZonesContainerView[6];
        
        [SerializeField] 
        public ListSection substitutesListSection;
        
        [SerializeField] 
        public ListSection reserveListSection;
        
        // [SerializeField]
        public PlayerItemView[] startingPlayersViews = new PlayerItemView[24];
        
        // [SerializeField]
        public PlayerItemView[] substitutesPlayersViews;
        
        // [SerializeField]
        public PlayerItemView[] reservePlayersViews;

        // [SerializeField] 
        public Dialog roleSelectorDialog;
        
        // [SerializeField] 
        // public GameObject roleSelectorDialog;
        //
        // [SerializeField] 
        // public GameObject roleSelectorDialogContainer;
        // [SerializeField] 
        // public GameObject roleSelectorDialogPrefab;

        public PlayerItemView SelectedPlayerItemView;
        
        public PlayerDataManager PlayerDataManager;
        public int teamId = 419;
        
        private SelectionSwapManager selectionSwapManager;
        public SelectionSwapManager SelectionSwapManager => selectionSwapManager;
    
        // Audio management
        [Header("Audio Settings")]
        [SerializeField] public AudioSource audioSource;
        [SerializeField] public AudioClip errorAudioClip; // Optional error sound
        [SerializeField] public AudioClip selectAudioClip;
        [SerializeField] public AudioClip clickAudioClip;
        
        // EVENTS
        public event Action<PlayerItemViewModeOption> OnViewModeChanged;
        
        // Mapping tactical positions to player IDs
        public Dictionary<TacticalPositionOption, int?> StartingPositionIdMapping;
        public List<int?> BenchPlayersIds;
        public List<int> reservePlayersIds;

        // Populated based on player data from PlayerDataManager
        public Dictionary<TacticalPositionOption, Player?> StartingPositionPlayerMapping;
        public ObservableList<Player> SubstitutesPlayersItems;
        public ObservableList<Player> ReservePlayersItems;

        private bool _isFirstLoad = true;

        [SerializeField] private bool autosaveApply = true;

        public int allowedSubstitutes = 9;
        public bool autoSortSubstitutes = false;  // Changed default to false as per requirement
        public bool autoSortReserves = true;      // New field for reserve sorting
        
        public BoardPlayerItemManager BoardPlayerItemManager;
        public BoardInitializationManager BoardInitializationManager;
        public BoardViewRefreshManager boardViewRefreshManager;
        private BoardSelectionManager boardSelectionManager;
        private BoardTacticManager boardTacticManager;
        
        private Team _team;
        
        private void Awake()
        {
            if (tacticsPitch == null)
            {
                Debug.LogError("TacticsPitch is not set. Please assign it in the inspector.");
            }

            _team = new Team(419, "Nitrocent United");
            _team.Players = new List<int>() { 
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 
                11, 101, 110, 107, 108, 201, 210, 207, 202, 203, 
                204, 205, 206, 208, 209, 211, 212, 213, 214, 215, 
                216, 217, 218, 219, 220, 221, 222, 223, 224, 225
            };
            
            Debug.Log("before");
            var tactic = new Tactic();
            _team.AddTactic(tactic);
            _team.ActiveTactic.ChangeFormation(FormationsPositions.F343);
            var starting11 = new List<int?>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 225, 10};
            _team.ActiveTactic.AssignPlayersToPosition(starting11);
            _team.ActiveTactic.Substitutes = new List<int?>() { 101, 110, 107, 108 };
            
            Debug.Log("Active positions count" + _team.ActiveTactic.ActivePositions.Count);
            
            StartingPositionPlayerMapping = new Dictionary<TacticalPositionOption, Core.Entities.Player?>();
            SubstitutesPlayersItems = new ObservableList<Core.Entities.Player>();
            ReservePlayersItems = new ObservableList<Core.Entities.Player>();
            
            var canvas = FindObjectOfType<Canvas>();
            dragInfoView = PlayerItemViewPoolManager.SpawnPlayerItemView(playerItemViewPrefab.GetComponent<PlayerItemView>(), canvas.gameObject.transform);
            dragInfoView.ViewOwnerOption = PlayerItemViewOwnerOption.DragAndDrop;
            dragInfoView.Controller = this;
            dragInfoView.gameObject.SetActive(false);
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            zoneContainerViews = tacticsPitch.zoneContainerViews;
            
            Debug.Log("zoneContainerViews count" + zoneContainerViews.Length);

            BoardPlayerItemManager = new BoardPlayerItemManager(this);
            boardTacticManager = new BoardTacticManager(this);
            boardViewRefreshManager = new BoardViewRefreshManager(this, BoardInitializationManager);
            BoardInitializationManager = new BoardInitializationManager(this, boardTacticManager);
            boardSelectionManager = new BoardSelectionManager(this, boardViewRefreshManager);
            // Initialize selection swap manager
            selectionSwapManager = new SelectionSwapManager(this);
        }

        private void Start()
        {
            InitData();
            InitViews();

            RegisterDropdownListeners();
            
            SubstitutesPlayersItems.OnCollectionChange += () =>
            {
                substitutesListSection.UpdateFormattedHeaderText(SubstitutesPlayersItems.Count.ToString());
                Debug.Log("Substitutes Count: " + SubstitutesPlayersItems.Count + "*** Players: " + string.Join(" ", SubstitutesPlayersItems.Where(item => item != null).Select(item => item.Id)));
            };

            ReservePlayersItems.OnCollectionChange += () =>
            {
                reserveListSection.UpdateFormattedHeaderText(ReservePlayersItems.Count.ToString());
                Debug.Log("Reserves Count: " + ReservePlayersItems.Count + "*** Players: " + string.Join(" ", ReservePlayersItems.Where(item => item != null).Select(item => item.Id)));
            };
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                selectionSwapManager.ClearSelection();
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                boardTacticManager.SaveTacticWithTimestamp();
            }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                boardSelectionManager.ClearAllSelections();
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                boardSelectionManager.ClearStartingLineup();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                boardSelectionManager.ClearSubstitutes();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    SetFormation(FormationsPositions.F442);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    SetFormation(FormationsPositions.F433_DM_Wide);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    SetFormation(FormationsPositions.F4141);

                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    SetFormation(FormationsPositions.F4231_Wide);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    SetFormation(FormationsPositions.F3232_352);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    SetFormation(FormationsPositions.F343);
                }
            }
        }

        private void InitViews()
        {
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                BoardInitializationManager.InitializeAndSetupBoard();
                BoardInitializationManager.InitializeSubstitutePlayers();
                BoardInitializationManager.InitializeReservePlayers();
            }
        }
        
        private void InitData()
        {
            Debug.Log("Active positions count" + _team.ActiveTactic.ActivePositions.Count);
            
            // Initialize the mapping dictionary
            StartingPositionPlayerMapping = new Dictionary<TacticalPositionOption, Core.Entities.Player?>();

            // Example bench player IDs (these would come from your PlayerDataManager in a real scenario)
            BenchPlayersIds = _team.ActiveTactic.Substitutes;

            // Step 1: Get the current squad from the PlayerDataManager
            List<Player> currentSquad = _team.Players
                .Select(id => PlayerDataManager.GetPlayerById(id))
                .Where(player => player != null)
                .ToList();

            // Step 2: Populate starting position mapping
            foreach (var position in _team.ActiveTactic.ActivePositions)
            {
                Debug.Log(position.ToString());
                TacticalPositionOption posOption = position.Position;
                var playerId = position.AssignedPlayerId;

                // Find the player profile with matching Id or set to null if not found
                var playerProfile = currentSquad.FirstOrDefault(p => p.Id == playerId);
                StartingPositionPlayerMapping[posOption] = playerProfile;
            }

            SubstitutesPlayersItems.Clear();
            ReservePlayersItems.Clear();

            int benchCount = 0;

            // Distribute bench players according to the AllowedSubstitutes limit
            for (int i = 0; i < BenchPlayersIds.Count; i++)
            {
                var playerId = BenchPlayersIds[i];
                var playerProfile = currentSquad.FirstOrDefault(p => p.Id == playerId);
    
                if (playerProfile != null)
                {
                    if (benchCount < allowedSubstitutes)
                    {
                        // Add to bench until the AllowedSubstitutes limit is reached
                        SubstitutesPlayersItems.Add(playerProfile);
                        benchCount++;
                    }
                    else
                    {
                        // Once bench is full, add remaining players to reserves
                        ReservePlayersItems.Add(playerProfile);
                    }
                }
                
                Debug.Log("bench player count: " + SubstitutesPlayersItems.Count);
                Debug.Log("bench players: " + string.Join(", ", SubstitutesPlayersItems.Select(player => player.Name)));
            }

            // Add any remaining players who aren't in startingPositionPlayerMapping, substitutesPlayersItems, or reservePlayersItems to reserves
            foreach (var player in currentSquad)
            {
                if (!StartingPositionPlayerMapping.ContainsValue(player) &&
                    !SubstitutesPlayersItems.Contains(player) &&
                    !ReservePlayersItems.Contains(player))
                {
                    ReservePlayersItems.Add(player);
                }
            }
        }

        #region Effects

        // Add these public methods for audio
        public void PlayClickSound()
        {
            if (clickAudioClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clickAudioClip);
            }
        }
    
        public void PlaySelectSound()
        {
            if (selectAudioClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(selectAudioClip);
            }
        }
    
        public void PlayErrorSound()
        {
            if (errorAudioClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(errorAudioClip);
            }
        }
        #endregion
        
        #region Item Click
        // Add method to handle item clicks
        public void HandleItemClicked(PlayerItemView clickedItem)
        {
            string playerName = clickedItem?.Profile?.Name ?? "Empty Position";
            Debug.Log($"{playerName} data Reached");
            
            SelectedPlayerItemView = clickedItem;
            
            if (selectionSwapManager != null)
            {
                selectionSwapManager.HandleItemClicked(clickedItem);
            }
            else
            {
                Debug.LogWarning("SelectionSwapManager is not initialized");
            }
        }
    
        // Add method to clear selections (useful for keyboard shortcuts)
        public void ClearClickSelections()
        {
            selectionSwapManager.ClearSelection();
        }
        #endregion
        
        /// <summary>
        /// Change formation preserving existing players with key remapping
        /// </summary>
        public void SetFormation(TacticalPositionOption[] newFormation, bool initCall = false)
        {
            // using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            // {
                // 1. Preserve existing players before formation change
                RemapStartingPositionPlayerMapping(newFormation);
                
                // 2. Update formation structure (shows/hides zones)
                boardTacticManager.SetFormationViews(newFormation, initCall);
                
                // 3. Update UI to reflect the remapped players
                // UpdateUIFromStartingPositionPlayerMapping();
                boardSelectionManager.AutoPickStartingLineupFromMapping();
            // }
        }

        /// <summary>
        /// Remap existing players to new formation keys, preserving player assignments
        /// </summary>
        private void RemapStartingPositionPlayerMapping(TacticalPositionOption[] newFormation)
        {
            // Preserve existing players (non-null values only)
            var existingPlayers = StartingPositionPlayerMapping.Values
                .Where(player => player != null)
                .ToList();
            
            // Clear old mapping with old keys
            StartingPositionPlayerMapping.Clear();
            
            // Create new mapping with new formation keys
            for (int i = 0; i < newFormation.Length; i++)
            {
                var position = newFormation[i];
                var player = i < existingPlayers.Count ? existingPlayers[i] : null;
                StartingPositionPlayerMapping[position] = player;
            }
        }

        /// <summary>
        /// Update UI views to reflect current StartingPositionPlayerMapping
        /// </summary>
        private void UpdateUIFromStartingPositionPlayerMapping()
        {
            foreach (var zoneContainer in zoneContainerViews)
            {
                if (zoneContainer == null) continue;

                foreach (var zoneView in zoneContainer.ZoneViews)
                {
                    if (zoneView == null || !zoneView.InUseForFormation) continue;

                    // Get player from mapping for this position
                    var position = zoneView.tacticalPositionOption;
                    if (StartingPositionPlayerMapping.TryGetValue(position, out var player))
                    {
                        // Update UI view with mapped player (or null)
                        zoneView.childPlayerItemView.SetPlayerData(player);
                    }
                }
            }
        }

        public void RegisterDropdownListeners()
        {
            tacticsPitch.viewModesDropDown.onValueChanged.AddListener((int selectedIndex) =>
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    // Handle the tacticsPitch mode change based on the selected index
                    PlayerItemViewModeOption selectedMode = (PlayerItemViewModeOption)selectedIndex;

                    // Implement your logic for the selected mode
                    Debug.Log("Selected View Mode: " + selectedMode);

                    // Publish event only; views will react individually
                    OnViewModeChanged?.Invoke(selectedMode);
                }
            });
        }

        public void OnDisable()
        {
            foreach (var playerItemView in startingPlayersViews)
            {
                if (playerItemView != null)
                {
                    playerItemView.OnFormationStatusChanged -= boardTacticManager.HandleFormationStatusChanged;
                    Debug.Log($"Unsubscribed from {playerItemView.gameObject.name} OnFormationStatusChanged");
                }
            }
            
            foreach (var playerItemView in substitutesPlayersViews)
            {
                if (playerItemView != null)
                {
                    playerItemView.OnFormationStatusChanged -= boardTacticManager.HandleFormationStatusChanged;
                    Debug.Log($"Unsubscribed from {playerItemView.gameObject.name} OnFormationStatusChanged");
                }
            }
            
            boardViewRefreshManager.Cleanup();
        }
    }
}