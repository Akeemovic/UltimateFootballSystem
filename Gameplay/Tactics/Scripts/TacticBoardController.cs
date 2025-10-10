using System;
using System.Collections.Generic;
using System.Linq;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UltimateFootballSystem.Core.Utils;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    /// <summary>
    /// Thin mediator/controller between TacticBoardModel and TacticBoardView.
    /// Handles user input and coordinates model-view updates.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class TacticBoardController : MonoBehaviour
    {
        [Header("References")]
        public TacticBoardView view;
        public PlayerDataManager playerDataManager;
        public int teamId = 419;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip errorAudioClip;
        [SerializeField] private AudioClip selectAudioClip;
        [SerializeField] private AudioClip clickAudioClip;

        [Header("Configuration")]
        [SerializeField] private int allowedSubstitutes = 9;
        [SerializeField] private bool autoSortSubstitutes = false;
        [SerializeField] private bool autoSortReserves = true;

        // Model and managers
        private TacticBoardModel _model;
        private SelectionSwapManager _selectionSwapManager;
        private Team _team;

        // Events
        public event Action<PlayerItemViewModeOption> OnViewModeChanged;

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeAudio();
            InitializeTeamAndModel();
            InitializeManagers();
            InitializeView();
        }

        private void Start()
        {
            LoadDataFromTeam();
            InitializeAllViews();
            RegisterListeners();
        }

        private void Update()
        {
            HandleKeyboardInput();
        }

        private void OnDisable()
        {
            UnregisterListeners();
            view?.Cleanup();
        }

        #endregion

        #region Initialization

        private void InitializeAudio()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void InitializeTeamAndModel()
        {
            // Create temp team (replace with real team loading)
            _team = new Team(teamId, "Nitrocent United");
            _team.Players = new List<int>() {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                11, 101, 110, 107, 108, 201, 210, 207, 202, 203,
                204, 205, 206, 208, 209, 211, 212, 213, 214, 215,
                216, 217, 218, 219, 220, 221, 222, 223, 224, 225
            };

            var tactic = new Tactic();
            _team.AddTactic(tactic);
            _team.ActiveTactic.ChangeFormation(FormationsPositions.F343);

            // var starting11 = new List<int?>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 225, 10};
            var starting11 = new List<int?>() { 1, 2, 3, 4, 5, 6, null, null, null, null, null};
            _team.ActiveTactic.AssignPlayersToPosition(starting11);
            _team.ActiveTactic.Substitutes = new List<int?>() { 101, 110, 107, 108 };

            // Create model
            _model = new TacticBoardModel(_team);
            _model.AllowedSubstitutes = allowedSubstitutes;
            _model.AutoSortSubstitutes = autoSortSubstitutes;
            _model.AutoSortReserves = autoSortReserves;

            // Subscribe to model events
            _model.OnSubstitutesCountChanged += (count) => view.UpdateSubstitutesHeaderCount(count);
            _model.OnReservesCountChanged += (count) => view.UpdateReservesHeaderCount(count);
        }

        private void InitializeManagers()
        {
            _selectionSwapManager = new SelectionSwapManager(this, _model, view);
        }

        private void InitializeView()
        {
            if (view == null)
            {
                Debug.LogError("TacticBoardView is not assigned!");
                return;
            }

            view.Initialize(this);
        }

        private void LoadDataFromTeam()
        {
            _model.InitializeFromTeam(playerDataManager);
        }

        private void InitializeAllViews()
        {
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                // Initialize starting players
                view.InitializeStartingPlayerViews(this, HandleFormationStatusChanged);

                // Set initial formation
                SetFormation(_model.StartingPositionPlayerMapping.Keys.ToArray(), initCall: true);

                // Initialize subs and reserves
                view.InitializeSubstituteViews(this, _model.SubstitutesPlayersItems, _model.AllowedSubstitutes, HandleFormationStatusChanged);
                view.InitializeReserveViews(this, _model.ReservePlayersItems);
            }
        }

        private void RegisterListeners()
        {
            view.OnViewModeChanged += HandleViewModeChanged;
            view.OnPlayerItemClicked += HandlePlayerItemClicked;
        }

        private void UnregisterListeners()
        {
            if (view != null)
            {
                view.OnViewModeChanged -= HandleViewModeChanged;
                view.OnPlayerItemClicked -= HandlePlayerItemClicked;
            }
        }

        #endregion

        #region User Input Handlers

        private void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _selectionSwapManager?.ClearSelection();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveTacticWithTimestamp();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearAllSelections();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                ClearStartingLineup();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                ClearSubstitutes();
            }

            // Formation shortcuts
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetFormation(FormationsPositions.F442);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetFormation(FormationsPositions.F433_DM_Wide);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetFormation(FormationsPositions.F4141);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SetFormation(FormationsPositions.F4231_Wide);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SetFormation(FormationsPositions.F3232_352);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SetFormation(FormationsPositions.F343);
            }
        }

        private void HandlePlayerItemClicked(PlayerItemView clickedItem)
        {
            _selectionSwapManager?.HandleItemClicked(clickedItem);
        }

        private void HandleViewModeChanged(int selectedIndex)
        {
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                PlayerItemViewModeOption selectedMode = (PlayerItemViewModeOption)selectedIndex;
                Debug.Log("Selected View Mode: " + selectedMode);
                OnViewModeChanged?.Invoke(selectedMode);
            }
        }

        private void HandleFormationStatusChanged(bool inUseForFormation)
        {
            Debug.Log("Formation status changed. Updating tactic based on active zones.");
            // Update model's CurrentTactic if needed
        }

        #endregion

        #region Formation Management

        public void SetFormation(TacticalPositionOption[] newFormation, bool initCall = false)
        {
            // 1. Update model (preserves existing player assignments)
            _model.SetFormation(newFormation);

            // 2. Update view zones
            view.UpdateFormationZones(newFormation, initCall);

            // 3. Update view to show players from model
            view.UpdateStartingPlayers(_model.StartingPositionPlayerMapping);

            // 4. Show only formation zones if not in selection mode
            if (!_selectionSwapManager.HasAnySelection)
            {
                view.ShowFormationZonesOnly();
            }
        }

        #endregion

        #region Player Management

        public void ClearAllSelections()
        {
            _model.ClearStartingLineup();
            _model.ClearSubstitutes();

            RefreshAllViews();
        }

        public void ClearStartingLineup()
        {
            _model.ClearStartingLineup();
            view.UpdateStartingPlayers(_model.StartingPositionPlayerMapping);
            view.InitializeReserveViews(this, _model.ReservePlayersItems);
        }

        public void ClearSubstitutes()
        {
            _model.ClearSubstitutes();
            view.RefreshSubstituteViews(_model.SubstitutesPlayersItems, _model.AllowedSubstitutes);
            view.InitializeReserveViews(this, _model.ReservePlayersItems);
        }

        /// <summary>
        /// Auto-pick starting lineup from model's mapping
        /// </summary>
        public void AutoPickStartingLineupFromMapping()
        {
            view.UpdateStartingPlayers(_model.StartingPositionPlayerMapping);
        }

        #endregion

        #region View Refresh

        public void RefreshAllViews()
        {
            view.UpdateStartingPlayers(_model.StartingPositionPlayerMapping);
            view.RefreshSubstituteViews(_model.SubstitutesPlayersItems, _model.AllowedSubstitutes);
            view.InitializeReserveViews(this, _model.ReservePlayersItems);
        }

        public void RefreshSubstituteViews()
        {
            Debug.Log($"[REFRESH] Refreshing substitute views - {_model.SubstitutesPlayersItems.Count} subs, allowed: {_model.AllowedSubstitutes}");
            view.RefreshSubstituteViews(_model.SubstitutesPlayersItems, _model.AllowedSubstitutes);
        }

        public void RefreshReserveViews()
        {
            Debug.Log($"[REFRESH] Refreshing reserve views - {_model.ReservePlayersItems.Count} reserves");
            view.InitializeReserveViews(this, _model.ReservePlayersItems);
        }

        #endregion

        #region Audio

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

        #region Tactic Saving

        private void SaveTacticWithTimestamp()
        {
            // Update current tactic from model
            _model.CurrentTactic.ActivePositions.Clear();

            // Only save positions that are InUseForFormation (the active formation)
            foreach (var playerView in view.startingPlayersViews)
            {
                if (playerView.InUseForFormation && playerView.ParentPositionZoneView != null)
                {
                    var position = playerView.ParentPositionZoneView.tacticalPositionOption;
                    var player = _model.StartingPositionPlayerMapping.TryGetValue(position, out var p) ? p : null;

                    var positionGroup = TacticalPositionUtils.GetGroupForPosition(position);
                    var tacticalPos = new TacticalPosition(positionGroup, position, new List<TacticalRole>());

                    if (player != null)
                    {
                        tacticalPos.AssignPlayer(player);
                    }

                    _model.CurrentTactic.ActivePositions.Add(tacticalPos);
                }
            }

            _model.CurrentTactic.Substitutes.Clear();
            foreach (var sub in _model.SubstitutesPlayersItems.Where(p => p != null))
            {
                _model.CurrentTactic.Substitutes.Add(sub.Id);
            }

            var formationString = _model.CurrentTactic.FormationToString();
            var positionsString = string.Join("-", _model.CurrentTactic.ActivePositions
                .OrderBy(p => p.Position)
                .Select(p => p.Position.ToString()));

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"Tactic_{positionsString}_{timestamp}.json";

            Debug.Log($"Saving tactic: {fileName}");
            // Implement actual save logic here
        }

        #endregion

        #region Public Accessors (for compatibility)

        public TacticBoardModel Model => _model;
        public PlayerItemView[] StartingPlayersViews => view.startingPlayersViews;
        public PlayerItemView[] SubstitutesPlayersViews => view.substitutesPlayersViews;
        public PlayerItemView[] ReservePlayersViews => view.reservePlayersViews;
        public SelectionSwapManager SelectionSwapManager => _selectionSwapManager;

        // View accessors
        public PlayerItemView dragInfoView => view.dragInfoView;
        public PositionZonesContainerView[] zoneContainerViews => view.zoneContainerViews;

        // Selected player
        public PlayerItemView SelectedPlayerItemView { get; set; }

        // Dialog
        public UIWidgets.Dialog roleSelectorDialog; // TODO: Move to view

        // Methods that other components expect
        public void HandleItemClicked(PlayerItemView clickedItem)
        {
            HandlePlayerItemClicked(clickedItem);
        }

        public void ClearClickSelections()
        {
            _selectionSwapManager?.ClearSelection();
        }

        /// <summary>
        /// UNIFIED swap logic - called by BOTH drag-and-drop AND click-to-swap.
        /// Handles formation changes, model updates, and view refreshes.
        /// </summary>
        public void SwapPlayers(PlayerItemDragData source, PlayerItemDragData target)
        {
            if (source == null || target == null)
            {
                Debug.LogWarning("[SWAP] Source or target data is null");
                return;
            }

            Debug.Log("========== SWAP START ==========");
            LogModelState("BEFORE SWAP");

            var sourceView = source.DragSourceView;
            var targetView = target.DropTargetView;

            // Step 1: Detect if this is a formation change
            bool isFormationChange = false;
            if (sourceView?.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList &&
                targetView?.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                bool sourceInFormation = sourceView.InUseForFormation;
                bool targetInFormation = targetView.InUseForFormation;

                if ((sourceInFormation && !targetInFormation) || (!sourceInFormation && targetInFormation))
                {
                    isFormationChange = true;
                    Debug.Log("[SWAP] Formation change detected - updating formation first");

                    // Step 2: Change formation FIRST
                    if (sourceInFormation && !targetInFormation)
                    {
                        sourceView.SetInUseForFormation(false);
                        targetView.SetInUseForFormation(true);
                    }
                    else
                    {
                        sourceView.SetInUseForFormation(true);
                        targetView.SetInUseForFormation(false);
                    }

                    // Step 3: Sync model dictionary to match new formation
                    SyncModelFormationFromViews();
                }
            }

            // Step 4: Extract data for swap using the new properties
            var sourcePos = source.SourceTacticalPosition;
            var targetPos = target.TargetTacticalPosition;

            int? sourceBenchIndex = source.BenchPlayersListIndex >= 0 ? source.BenchPlayersListIndex : (int?)null;
            int? sourceReserveIndex = source.ReservePlayersListIndex >= 0 ? source.ReservePlayersListIndex : (int?)null;
            int? targetBenchIndex = target.BenchPlayersListIndex >= 0 ? target.BenchPlayersListIndex : (int?)null;
            int? targetReserveIndex = target.ReservePlayersListIndex >= 0 ? target.ReservePlayersListIndex : (int?)null;

            var sourcePlayer = source.Profile;
            var targetPlayer = target.Profile;

            Debug.Log($"[SWAP] Source: {sourcePos?.ToString() ?? "NULL"} (bench:{sourceBenchIndex}, reserve:{sourceReserveIndex}) = {sourcePlayer?.Name ?? "NULL"}, Target: {targetPos?.ToString() ?? "NULL"} (bench:{targetBenchIndex}, reserve:{targetReserveIndex}) = {targetPlayer?.Name ?? "NULL"}");

            // Step 5: Perform swap in model (dictionary now has correct positions if formation changed)
            _model.SwapPlayers(
                sourcePos, sourceBenchIndex, sourceReserveIndex, sourcePlayer,
                targetPos, targetBenchIndex, targetReserveIndex, targetPlayer
            );

            LogModelState("AFTER MODEL SWAP");

            // Step 6: Compact substitutes if needed
            if (sourceBenchIndex.HasValue || targetBenchIndex.HasValue)
            {
                Debug.Log($"[SWAP] Compacting substitutes - source bench: {sourceBenchIndex}, target bench: {targetBenchIndex}");
                _model.CompactSubstitutes();
                LogModelState("AFTER COMPACT SUBS");
            }

            // Step 6b: Compact reserves if needed (remove any null entries)
            Debug.Log($"[SWAP] Checking if reserves compacting needed - sourceReserveIndex.HasValue: {sourceReserveIndex.HasValue}, targetReserveIndex.HasValue: {targetReserveIndex.HasValue}");
            if (sourceReserveIndex.HasValue || targetReserveIndex.HasValue)
            {
                Debug.Log($"[SWAP] YES - Compacting reserves - source reserve: {sourceReserveIndex}, target reserve: {targetReserveIndex}");
                var nullCountBefore = _model.ReservePlayersItems.Count(p => p == null);
                Debug.Log($"[SWAP] Null count BEFORE compact: {nullCountBefore}");
                _model.CompactReserves();
                var nullCountAfter = _model.ReservePlayersItems.Count(p => p == null);
                Debug.Log($"[SWAP] Null count AFTER compact: {nullCountAfter}");
                LogModelState("AFTER COMPACT RESERVES");
            }
            else
            {
                Debug.Log($"[SWAP] NO - Reserves compacting NOT needed");
            }

            // Step 7: Update views
            // using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            // {
                UpdateViewsAfterSwap(sourceView, targetView);
            // }

            LogModelState("AFTER VIEW UPDATE");
            Debug.Log("========== SWAP END ==========");
        }

        /// <summary>
        /// Sync the model's StartingPositionPlayerMapping with the current formation from views.
        /// Rebuilds the dictionary with exactly 11 positions based on InUseForFormation flags.
        /// </summary>
        private void SyncModelFormationFromViews()
        {
            var activePositions = new List<TacticalPositionOption>();
            var positionPlayerMap = new Dictionary<TacticalPositionOption, Core.Entities.Player>();

            foreach (var playerView in view.startingPlayersViews)
            {
                if (playerView.InUseForFormation && playerView.ParentPositionZoneView != null)
                {
                    var position = playerView.ParentPositionZoneView.tacticalPositionOption;
                    activePositions.Add(position);

                    // Get current player at this position from model (if exists)
                    var player = _model.StartingPositionPlayerMapping.TryGetValue(position, out var p) ? p : null;
                    positionPlayerMap[position] = player;
                }
            }

            Debug.Log($"[SYNC FORMATION] Syncing model with {activePositions.Count} active positions");
            _model.SyncFormationFromViews(activePositions.ToArray(), positionPlayerMap);
        }

        /// <summary>
        /// DEPRECATED: Use unified SwapPlayers() instead.
        /// </summary>
        [System.Obsolete("Use SwapPlayers(source, target) instead")]
        public void SwapPlayersDropped(PlayerItemDragData dragged, PlayerItemDragData dropTarget)
        {
            SwapPlayers(dragged, dropTarget);
        }

        /// <summary>
        /// Targeted view updates after swap - more efficient than RefreshAllViews
        /// </summary>
        private void UpdateViewsAfterSwap(PlayerItemView sourceView, PlayerItemView targetView)
        {
            // Update source view
            if (sourceView != null && sourceView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                if (sourceView.ParentPositionZoneView != null)
                {
                    var position = sourceView.ParentPositionZoneView.tacticalPositionOption;

                    // Only update if position is in current formation
                    if (_model.StartingPositionPlayerMapping.TryGetValue(position, out var player))
                    {
                        // Position IS in formation - update with current player (may be null)
                        sourceView.SetPlayerData(player);
                        // NOTE: Don't change InUseForFormation here - it was already set correctly above
                    }
                    else
                    {
                        // Position NOT in current formation - clear the view
                        sourceView.SetPlayerData(null);
                        // NOTE: Don't change InUseForFormation here - it was already set correctly above
                    }
                }
            }
            else if (sourceView != null && sourceView.ViewOwnerOption == PlayerItemViewOwnerOption.BenchList)
            {
                RefreshSubstituteViews();
            }
            else if (sourceView != null && sourceView.ViewOwnerOption == PlayerItemViewOwnerOption.ReserveList)
            {
                RefreshReserveViews();
            }

            // Update target view
            if (targetView != null && targetView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                if (targetView.ParentPositionZoneView != null)
                {
                    var position = targetView.ParentPositionZoneView.tacticalPositionOption;

                    // Only update if position is in current formation
                    if (_model.StartingPositionPlayerMapping.TryGetValue(position, out var player))
                    {
                        // Position IS in formation - update with current player (may be null)
                        targetView.SetPlayerData(player);
                        // NOTE: Don't change InUseForFormation here - it was already set correctly above
                    }
                    else
                    {
                        // Position NOT in current formation - clear the view
                        targetView.SetPlayerData(null);
                        // NOTE: Don't change InUseForFormation here - it was already set correctly above
                    }
                }
            }
            else if (targetView != null && targetView.ViewOwnerOption == PlayerItemViewOwnerOption.BenchList)
            {
                RefreshSubstituteViews();
            }
            else if (targetView != null && targetView.ViewOwnerOption == PlayerItemViewOwnerOption.ReserveList)
            {
                RefreshReserveViews();
            }

            // Hide unused views if swapping in starting list
            if ((sourceView != null && sourceView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList) ||
                (targetView != null && targetView.ViewOwnerOption == PlayerItemViewOwnerOption.StartingList))
            {
                view.HideUnusedPlayerItemViews();
            }
        }

        /// <summary>
        /// Log the complete model state for debugging
        /// </summary>
        private void LogModelState(string label)
        {
            Debug.Log($"===== MODEL STATE: {label} =====");

            // Log Starting Formation
            Debug.Log($"[STARTING] Count: {_model.StartingPositionPlayerMapping.Count}");
            foreach (var kvp in _model.StartingPositionPlayerMapping.OrderBy(x => x.Key.ToString()))
            {
                Debug.Log($"  {kvp.Key} = {kvp.Value?.Name ?? "null"}");
            }

            // Log Substitutes
            Debug.Log($"[SUBS] Count: {_model.SubstitutesPlayersItems.Count}, Allowed: {_model.AllowedSubstitutes}");
            for (int i = 0; i < _model.SubstitutesPlayersItems.Count; i++)
            {
                var player = _model.SubstitutesPlayersItems[i];
                Debug.Log($"  [{i}] = {player?.Name ?? "null"}");
            }

            // Log Reserves
            Debug.Log($"[RESERVES] Count: {_model.ReservePlayersItems.Count}");
            for (int i = 0; i < _model.ReservePlayersItems.Count; i++)
            {
                var player = _model.ReservePlayersItems[i];
                Debug.Log($"  [{i}] = {player?.Name ?? "null"}");
            }

            Debug.Log($"=====================================");
        }

        #endregion
    }
}
