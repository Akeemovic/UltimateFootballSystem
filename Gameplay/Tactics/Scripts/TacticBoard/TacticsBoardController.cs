using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UltimateFootballSystem.Core.Entities;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UltimateFootballSystem.Core.Utils;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Compositions;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.ListSections;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.Options;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.ViewModes.Options;
using UnityEngine;
using UnityEngine.Serialization;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard
{
    public class TacticsBoardController : MonoBehaviour
    {
        public Canvas canvas;
    
        [HideInInspector]
        public PlayerItemView DragInfoView;
    
        [SerializeField] 
        public Transform playerItemViewPrefab;
    
        public TacticsPitch View;

        [HideInInspector] 
        public PositionZonesContainerView[] zoneContainerViews = new PositionZonesContainerView[6];
    
        [FormerlySerializedAs("benchListSection")] [SerializeField] 
        public ListSection substitutesListSection;
    
        [SerializeField] 
        public ListSection reserveListSection;
    
        // [SerializeField]
        public PlayerItemView[] StartingPlayersViews = new PlayerItemView[24];
    
        // [SerializeField]
        // protected PlayerItemView[] SubstitutesPlayersViews = new PlayerItemView[7];
        [FormerlySerializedAs("BenchPlayersViews")] public PlayerItemView[] SubstitutesPlayersViews;
    
        // [SerializeField]
        // protected PlayerItemView[] ReservePlayersViews = new PlayerItemView[7];
        public PlayerItemView[] ReservePlayersViews;

        [SerializeField] 
        public Dialog RoleSelectorDialog;
    
        public PlayerDataManager playerDataManager;
        public int teamId = 419;

        // Mapping tactical positions to player IDs
        public Dictionary<TacticalPositionOption, int?> startingPositionIdMapping;
        public List<int?> benchPlayersIds;
        public List<int> reservePlayersIds;

        // Populated based on player data from PlayerDataManager
        public Dictionary<TacticalPositionOption, Core.Entities.Player?> startingPositionPlayerMapping;
        public ObservableList<Core.Entities.Player> substitutesPlayersItems;
        public ObservableList<Core.Entities.Player> reservePlayersItems;


        private bool _isFirstLoad = true;

        [SerializeField]
        private bool AutosaveApply = true;

        public int AllowedSubstitutes = 9;
    
        // public static TacticsBoardController Instance;

        public PlayerItemManager PlayerItemManager;
        public FormationManager FormationManager;

        private Team _team;
    
        private void Awake()
        {
            // if (Instance == null)
            // {
            //     Instance = this;
            // }
            // else
            // {
            //     Destroy(gameObject);
            // }

            if (View == null)
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
            // _team.ActiveTactic.ChangeFormation(FormationsPositions.F3232_352);
            // var starting11 = new List<int?>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, null };
            _team.ActiveTactic.ChangeFormation(FormationsPositions.F343);
            // var starting11 = new List<int?>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, null, 10};
            var starting11 = new List<int?>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 225, 10};
            _team.ActiveTactic.AssignPlayersToPosition(starting11);
            _team.ActiveTactic.Substitutes = new List<int?>() { 101, 110, 107, 108 };
        
        
            Debug.Log("Active positions count" + _team.ActiveTactic.ActivePositions.Count);
        
            startingPositionPlayerMapping = new Dictionary<TacticalPositionOption, Core.Entities.Player?>();
            substitutesPlayersItems = new ObservableList<Core.Entities.Player>();
            reservePlayersItems = new ObservableList<Core.Entities.Player>();
        
            var canvas = FindObjectOfType<Canvas>();
            DragInfoView =
                Instantiate(playerItemViewPrefab, canvas.gameObject.transform).GetComponent<PlayerItemView>();
            DragInfoView.ViewOwnerOption = PlayerItemViewOwnerOption.DragAndDrop;
            // DragInfoView.GetComponent<PlayerItemDragSupport>().Controller = this;
            // DragInfoView.GetComponent<PlayerItemDropSupport>().Controller = this;
            DragInfoView.Controller = this;
            DragInfoView.gameObject.SetActive(false);
        
            zoneContainerViews = View.zoneContainerViews;
        
        
            Debug.Log("zoneContainerViews count" + zoneContainerViews.Length);

            PlayerItemManager = new PlayerItemManager(this);
            FormationManager = new FormationManager(this);
        }

        private void Start()
        {
           
            InitData();
            InitViews();

            RegisterDropdownListeners();
        
            substitutesPlayersItems.OnCollectionChange += () =>
            {
                substitutesListSection.UpdateFormattedHeaderText(substitutesPlayersItems.Count.ToString());
                Debug.Log("Substitutes Count: " + substitutesPlayersItems.Count + "*** Players: " + string.Join(" ", substitutesPlayersItems.Where(item => item != null).Select(item => item.Id)));
            };

            reservePlayersItems.OnCollectionChange += () =>
            {
                reserveListSection.UpdateFormattedHeaderText(reservePlayersItems.Count.ToString());
                Debug.Log("Reserves Count: " + reservePlayersItems.Count + "*** Players: " + string.Join(" ", reservePlayersItems.Where(item => item != null).Select(item => item.Id)));
            };
        }
    
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                FormationManager.ClearSelection();
            }
        
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    FormationManager.SetFormationViews(FormationsPositions.F442);
                    LoadPlayersAutofill(startingPositionPlayerMapping.Values);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    FormationManager.SetFormationViews(FormationsPositions.F433_DM_Wide);
                    LoadPlayersAutofill(startingPositionPlayerMapping.Values);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    FormationManager.SetFormationViews(FormationsPositions.F4141);
                    LoadPlayersAutofill(startingPositionPlayerMapping.Values);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    FormationManager.SetFormationViews(FormationsPositions.F4231_Wide);
                    LoadPlayersAutofill(startingPositionPlayerMapping.Values);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    FormationManager.SetFormationViews(FormationsPositions.F3232_352);
                    LoadPlayersAutofill(startingPositionPlayerMapping.Values);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    FormationManager.SetFormationViews(FormationsPositions.F343);
                    LoadPlayersAutofill(startingPositionPlayerMapping.Values);
                }
            }
        }

        #region Initializers
        private void InitViews()
        {
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                InitializeAndSetupBoard();
                InitializeSubstitutePlayers();
                InitializeReservePlayers();
            }
        }
    
        private void InitData()
        {
            // Initialize collections
            // startingPositionIdMapping = new Dictionary<TacticalPositionOption, int?>()
            // {
            //     { TacticalPositionOption.GK, 1 },
            //     { TacticalPositionOption.DL, 2 }, 
            //     { TacticalPositionOption.DCL, 3 }, 
            //     { TacticalPositionOption.DCR, 4 }, 
            //     { TacticalPositionOption.DR, 5 },
            //     { TacticalPositionOption.ML, 6 }, 
            //     { TacticalPositionOption.MCL, 7 }, 
            //     { TacticalPositionOption.MCR, 8 }, 
            //     { TacticalPositionOption.MR, 9 },
            //     { TacticalPositionOption.STCL, 10 },
            //     // { TacticalPositionOption.STCR, 11 },
            //     { TacticalPositionOption.STCR, null }
            // };

            Debug.Log("Active positions count" + _team.ActiveTactic.ActivePositions.Count);
        
            // Initialize the mapping dictionary
            startingPositionPlayerMapping = new Dictionary<TacticalPositionOption, Core.Entities.Player?>();

            // Example bench player IDs (these would come from your PlayerDataManager in a real scenario)
            benchPlayersIds = _team.ActiveTactic.Substitutes;

            // Step 1: Get the current squad from the PlayerDataManager
            List<Core.Entities.Player> currentSquad = _team.Players
                .Select(id => PlayerDataManager.GetPlayerById(id))
                .Where(player => player != null)
                .ToList();

            // Step 2: Populate starting position mapping
            // foreach (var kvp in startingPositionIdMapping)
            // {
            //     TacticalPositionOption position = kvp.Key;
            //     var playerId = kvp.Value;
            //
            //     // Find the player profile with matching Id or set to null if not found
            //     var playerProfile = currentSquad.FirstOrDefault(p => p.Id == playerId);
            //     startingPositionPlayerMapping[position] = playerProfile;
            // }
            foreach (var position in _team.ActiveTactic.ActivePositions)
            {
                Debug.Log(position.ToString());
                TacticalPositionOption posOption = position.Position;
                var playerId = position.AssignedPlayerId;

                // Find the player profile with matching Id or set to null if not found
                // var playerProfile = currentSquad.FirstOrDefault(p => p.Id == playerId);
                var playerProfile = currentSquad.FirstOrDefault(p => p.Id == playerId);
                startingPositionPlayerMapping[posOption] = playerProfile;
            }

            // Log the startingPositionPlayerMapping
            // Debug.Log("Starting Position Player Mapping:");
            // foreach (var kvp in startingPositionPlayerMapping)
            // {
            //     string playerId = kvp.Value?.Id.ToString() ?? "None";
            //     Debug.Log($"{kvp.Key}: {playerId}");
            // }
        
            substitutesPlayersItems.Clear();
            reservePlayersItems.Clear();

            int benchCount = 0;

            // Distribute bench players according to the AllowedSubstitutes limit
            for (int i = 0; i < benchPlayersIds.Count; i++)
            {
                var playerId = benchPlayersIds[i];
                var playerProfile = currentSquad.FirstOrDefault(p => p.Id == playerId);
    
                if (playerProfile != null)
                {
                    if (benchCount < AllowedSubstitutes)
                    {
                        // Add to bench until the AllowedSubstitutes limit is reached
                        substitutesPlayersItems.Add(playerProfile);
                        benchCount++;
                    }
                    else
                    {
                        // Once bench is full, add remaining players to reserves
                        reservePlayersItems.Add(playerProfile);
                    }
                }
            
                Debug.Log("bench player count: " + substitutesPlayersItems.Count);
                Debug.Log("bench players: " + string.Join(", ", substitutesPlayersItems.Select(player => player.Name)));
            }

            // Add any remaining players who aren't in startingPositionPlayerMapping, substitutesPlayersItems, or reservePlayersItems to reserves
            foreach (var player in currentSquad)
            {
                if (!startingPositionPlayerMapping.ContainsValue(player) &&
                    !substitutesPlayersItems.Contains(player) &&
                    !reservePlayersItems.Contains(player))
                {
                    reservePlayersItems.Add(player);
                }
            }

        }
   
        public void InitializeAndSetupBoard()
        {
            // Initialize starting player views
            InitializeStartingPlayersViews();

            // Set formation views based on starting positions mappings,
            // the Keys are TacticalPositions (representing the formation)
            FormationManager.SetFormationViews(startingPositionPlayerMapping.Keys.ToArray(), initCall: true);

            // Load players into the board
            LoadPlayers(startingPositionPlayerMapping);
        }

    
        void InitializeStartingPlayersViews()
        {
            int index = 0;
            foreach (var zoneContainer in zoneContainerViews)
            {
                zoneContainer.gameObject.SetActive(true);
                var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);
                foreach (var zoneView in zoneViews)
                {
                    if (index < StartingPlayersViews.Length)
                    {
                        var playerItemView = zoneView.GetComponentInChildren<PlayerItemView>(true);
                        if (playerItemView != null)
                        {
                            StartingPlayersViews[index] = playerItemView;
                            // Debug.Log($"Starting player view assigned at index {index}");
                        
                            // playerItemView.OnFormationStatusChanged += HandleFormationStatusChanged;
                            playerItemView.Controller = this;   
                            playerItemView.ParentPositionZoneView = zoneView;
                            playerItemView.ViewOwnerOption = PlayerItemViewOwnerOption.StartingList;
                            playerItemView.StartingPlayersListIndex = index;
                            var tacticalPositionOption = zoneView.tacticalPositionOption;
                            // Create an instance of the tactical position to manage positions related roles & duties 
                            playerItemView.TacticalPosition = new TacticalPosition(
                                PositionGroupManager.GetGroupForPosition(tacticalPositionOption),
                                tacticalPositionOption,
                                RoleManager.GetRolesForPosition(tacticalPositionOption)
                                    .Select(roleOption => RoleManager.GetRole(roleOption))
                                    .ToList()
                            );
                        }
                        else
                        {
                            // Debug.LogError($"No PlayerItemView found in PositionZoneView at index {index}");
                        }
                        index++;
                    }
                    else
                    {
                        // Debug.LogWarning("StartingPlayersViews array is full. Skipping additional ZoneViews.");
                    }
                }
            }

            for (int i = 0; i < StartingPlayersViews.Length; i++)
            {
                if (StartingPlayersViews[i] == null)
                {
                    Debug.LogError($"StartingPlayersViews[{i}] is not initialized.");
                }
            }
        }

        public void LoadPlayers(Dictionary<TacticalPositionOption, Core.Entities.Player?> startingPositionPlayersMapping)
        {
            // Loop through each container of zone views on the board
            foreach (var zoneContainer in zoneContainerViews)
            {
                // Get all zone views (positions) within the current container
                var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);
        
                // Loop through each zone view to find matching tactical positions
                foreach (var zoneView in zoneViews)
                {
                    // Check if the zone is in use for the formation
                    if (zoneView.InUseForFormation)
                    {
                        // Try to find a player for the current zone's tactical position
                        // if (startingPositionPlayersMapping.TryGetValue(zoneView.tacticalPositionOption, out var player) && player != null)
                        // Find player, set whatever is return, even nulls
                        if (startingPositionPlayersMapping.TryGetValue(zoneView.tacticalPositionOption, out var player))
                        {
                            // Debug.Log($"LoadPlayers: Setting player {player.Name} in zone view {zoneView.name}.");
                    
                            // Set the player data in the zone's child player item view
                            zoneView.childPlayerItemView.SetPlayerData(player);
                        }
                        else
                        {
                            // Debug.Log($"LoadPlayers: No player found or assigned for tactical position {zoneView.tacticalPositionOption}.");
                        }
                    }
                    else
                    {
                        // Debug.Log($"LoadPlayers: Zone view {zoneView.name} is not in use for formation.");
                    }
                }
            }
        }

        public void LoadPlayersAutofill(IEnumerable<Core.Entities.Player?> players)
        {
            Debug.Log("LoadPlayersAutofill method called.");
    
            int playerIndex = 0;
    
            // Loop through each container of zone views on the board
            foreach (var zoneContainer in zoneContainerViews)
            {
                var zoneViews = zoneContainer.GetComponentsInChildren<PositionZoneView>(true);
                // Loop through each zone view and assign players or set to null if not in use
                foreach (var zoneView in zoneViews)
                {
                    if (zoneView.InUseForFormation && playerIndex < players.Count())
                    {
                        var player = players.ElementAt(playerIndex);
                
                        if (player != null)
                        {
                            Debug.Log($"LoadPlayersAutofill: Setting player {player.Name} in zone view {zoneView.name}.");
                            zoneView.childPlayerItemView.SetPlayerData(player);
                        }
                        else
                        {
                            Debug.Log($"LoadPlayersAutofill: No player to assign to zone view {zoneView.name}, setting data to null.");
                            zoneView.childPlayerItemView.SetPlayerData(null);
                        }

                        playerIndex++;
                    }
                    else
                    {
                        Debug.Log($"LoadPlayersAutofill: Zone view {zoneView.name} is not in use for formation, setting data to null.");
                        zoneView.childPlayerItemView.SetPlayerData(null);
                    }
                }
            }
            Debug.Log("LoadPlayersAutofill method completed.");
        }
    
        public void InitializeSubstitutePlayers()
        {
            // Clear existing views
            foreach (Transform child in substitutesListSection.viewsContainer)
            {
                Destroy(child.gameObject);
            }

            // Initialize the SubstitutesPlayersViews array with the size of AllowedSubstitutes
            SubstitutesPlayersViews = new PlayerItemView[AllowedSubstitutes];

            for (int i = 0; i < SubstitutesPlayersViews.Length; i++)
            {
                // Instantiate a new PlayerItemView (assuming you have a prefab for PlayerItemView)
                GameObject playerItemViewObject = Instantiate(playerItemViewPrefab.gameObject, substitutesListSection.viewsContainer);
                PlayerItemView playerItemView = playerItemViewObject.GetComponent<PlayerItemView>();
            
                playerItemView.Controller = this;
                playerItemView.ViewOwnerOption = PlayerItemViewOwnerOption.BenchList;
                if (i < substitutesPlayersItems.Count && substitutesPlayersItems[i] != null)
                {
                    // Set data for the player profile view if the item exists
                    playerItemView.SetPlayerData(substitutesPlayersItems[i]);
                }
                else
                {
                    // Show placeholder and hide the main view if the player item is null or out of range
                    playerItemView.placeholderView.Show();
                    playerItemView.mainView.Hide();
                }
                playerItemView.BenchPlayersListIndex = i;
                SubstitutesPlayersViews[i] = playerItemView;
                playerItemView.placeholderView.UpdatePositionText();
                
                // Logging to verify correct initialization
                string playerName = playerItemView.HasPlayerItem ? playerItemView.Profile.Name : "Placeholder";
                Debug.Log($"Substitutes player {playerName} initialized at index {i}");
            }
    
            if (AllowedSubstitutes <= 9)
            {
                substitutesListSection.DisableScroll();
            }
        
            // substitutesListSection.SetHeaderText($"Substitutes ({substitutesPlayersItems.Count}/{AllowedSubstitutes})");
            substitutesListSection.SetHeaderTextFormat("{0} ({1}/"+ AllowedSubstitutes +")", "Substitutes");
            substitutesListSection.UpdateFormattedHeaderText(substitutesPlayersItems.Count.ToString());
        }
    
        public void InitializeReservePlayers()
        {
            // Clear existing views
            foreach (Transform child in reserveListSection.viewsContainer)
            {
                Destroy(child.gameObject);
            }

            ReservePlayersViews = new PlayerItemView[reservePlayersItems.Count];

            for (int i = 0; i < reservePlayersItems.Count; i++)
            {
                // Instantiate a new PlayerItemView (assuming you have a prefab for PlayerItemView)
                GameObject playerItemViewObject = Instantiate(playerItemViewPrefab.gameObject, reserveListSection.viewsContainer); // Changed from benchContainerView to reserveContainerView
                PlayerItemView playerItemView = playerItemViewObject.GetComponent<PlayerItemView>();

                playerItemView.Controller = this;
                playerItemView.ViewOwnerOption = PlayerItemViewOwnerOption.ReserveList;
                // Set data for the player profile view
                playerItemView.SetPlayerData(reservePlayersItems[i]);
                playerItemView.ReservePlayersListIndex = i;
                ReservePlayersViews[i] = playerItemView;

                Debug.Log($"Reserve player {playerItemView.Profile.Name} initialized at index {i}");
            }
        
            // reserveListSection.SetHeaderText("Reserves (" + reservePlayersItems.Count + ")");
            reserveListSection.SetHeaderTextFormat("{0} ({1})", "Reserves");
            reserveListSection.UpdateFormattedHeaderText(reservePlayersItems.Count.ToString());
        }
        #endregion
    
        public void RegisterDropdownListeners()
        {
            View.viewModesDropDown.onValueChanged.AddListener((int selectedIndex) =>
            {
                using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
                {
                    // Handle the view mode change based on the selected index
                    PlayerItemViewModeOption selectedMode = (PlayerItemViewModeOption)selectedIndex;

                    // Implement your logic for the selected mode
                    Debug.Log("Selected View Mode: " + selectedMode);

                    foreach (var itemView in StartingPlayersViews)
                    {
                        itemView.mainView.ViewMode = selectedMode;
                    }

                    // No Tactical Roles for substitutes and reserve players
                    if (selectedMode == PlayerItemViewModeOption.Roles) return;
                    {
                        // Continue to perform change for other view modes
                        foreach (var itemView in SubstitutesPlayersViews)
                        {
                            itemView.mainView.ViewMode = selectedMode;
                        }

                        foreach (var itemView in ReservePlayersViews)
                        {
                            itemView.mainView.ViewMode = selectedMode;
                        }
                    }
                }
            });
        }

        public void OnDisable()
        {
            foreach (var playerItemView in StartingPlayersViews)
            {
                if (playerItemView != null)
                {
                    playerItemView.OnFormationStatusChanged -= FormationManager.HandleFormationStatusChanged;
                    Debug.Log($"Unsubscribed from {playerItemView.gameObject.name} OnFormationStatusChanged");
                }
            }
        }

    }
}
