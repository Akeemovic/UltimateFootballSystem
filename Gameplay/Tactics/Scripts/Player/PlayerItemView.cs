using System;
using System.Linq;
using UltimateFootballSystem.Core.Tactics;
using UltimateFootballSystem.Core.Tactics.Utils;
using UnityEngine;
using Lean.Pool;
using UltimateFootballSystem.Core.Entities;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    /// <summary>
    /// Player Profile View.
    /// </summary>
    [Serializable]
    public class PlayerItemView : MonoBehaviour, IPoolable
    {
        public TacticBoardController Controller;
    
        public PlayerItemViewMain mainView;
        // REMOVED: public PlayerItemViewPlaceholder placeholderView; // Make sure this is deleted from Inspector too!

        public PlayerItemViewSelectionLayer PlayerItemViewSelectionLayer;
    
        /// <summary>
        /// ViewOwnerOption
        /// </summary>
        [NonSerialized]
        public PlayerItemViewOwnerOption ViewOwnerOption;
    
        /// <summary>
        /// Parent Zone tacticsPitch.
        /// </summary>
        [NonSerialized]
        public PositionZoneView ParentPositionZoneView;
    
        public event Action<bool> OnFormationStatusChanged;

        /// <summary>
        /// Indicates whether the profile is in use for formation.
        /// </summary>
        public bool InUseForFormation { get; private set; }

        /// <summary>
        /// Indicates whether the player profile has valid data.
        /// </summary>
        public bool HasPlayerItem;

        /// <summary>
        /// Starting Players ListIndex index.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public int StartingPlayersListIndex = -1;
    
        /// <summary>
        /// Substitutes Players ListIndex index.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public int BenchPlayersListIndex = -1;
    
        /// <summary>
        /// Formation Zones List Index index.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public int FormationZonesListIndex = -1;
    
        /// <summary>
        /// Reserve Players ListIndex index.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public int ReservePlayersListIndex = -1;
    
        /// <summary>
        /// Player Profile.
        /// </summary>
        public Core.Entities.Player Profile { get; protected set; }
    
        /// <summary>
        /// Tactical Position Option
        /// </summary>
        public TacticalPositionOption TacticalPositionOption { get; set; }
    
        public TacticalPosition TacticalPosition;

        // EVENTS
        public event Action<Player> OnDataChanged;
        public event Action<TacticalRoleOption> OnRoleChanged;
        public event Action<TacticalDutyOption> OnDutyChanged;

        private void OnEnable()
        {
            // TacticalPosition = new TacticalPosition(
            //     TacticalPositionUtils.GetGroupForPosition(TacticalPositionOption),
            //     TacticalPositionOption,
            //     RoleManager.GetRolesForPosition(TacticalPositionOption)
            //         .Select(roleOption => RoleManager.GetRole(roleOption))
            //         .ToList()
            // );
        }

        private void Awake()
        {
            SetInUseForFormation(false);
            SetPlayerData(null);
        }

        private void Start()
        {
            // Initialization is now handled by InitializeTacticalPosition method
        }

        /// <summary>
        /// Get drag data.
        /// </summary>
        /// <returns>Drag data.</returns>
        public virtual PlayerItemDragData GetDragData()
        {
            return ViewOwnerOption switch
            {
                PlayerItemViewOwnerOption.BenchList => PlayerItemDragData.Bench(Profile, BenchPlayersListIndex),
                PlayerItemViewOwnerOption.ReserveList => PlayerItemDragData.Reserve(Profile, ReservePlayersListIndex),
                _ => PlayerItemDragData.Starting(Profile, StartingPlayersListIndex)
            };
        }

        /// <summary>
        /// Set data.
        /// </summary>
        /// <param name="profile">Player.</param>
        public virtual void SetPlayerData(Core.Entities.Player profile)
        {
            Profile = profile;
            bool oldHasPlayerItem = HasPlayerItem;
            
            if (Profile != null && !string.IsNullOrEmpty(Profile.Name) && Profile.Id >= 1)
            {
                HasPlayerItem = true;
                Debug.Log("View owner:" + ViewOwnerOption);
                
                // Safely assign player to tactical position
                if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    EnsureTacticalPositionInitialized();
                    if (TacticalPosition != null)
                    {
                        TacticalPosition.AssignedPlayerId = Profile.Id;
                        TacticalPosition.AssignedPlayer = Profile;
                    }
                }
                Debug.Log("SetPlayerData: Data reaching profile tacticsPitch belongs to: " + Profile.Name);
            }
            else
            {
                HasPlayerItem = false;
                
                // Safely clear player from tactical position
                if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    EnsureTacticalPositionInitialized();
                    if (TacticalPosition != null)
                    {
                        TacticalPosition.AssignedPlayerId = null;
                        TacticalPosition.AssignedPlayer = null;
                    }
                }
                Profile = new Core.Entities.Player();
                Debug.Log("SetPlayerData: Data reaching profile tacticsPitch is null: ");
            }

            // mainView handles all display logic now
            mainView.Show(); // Ensure the mainView is active
            mainView.UpdateView(); // This will trigger PlayerItemGeneralViewMode.UpdateView()

            OnDataChanged?.Invoke(Profile);

            if (oldHasPlayerItem != HasPlayerItem)
            {
                OnFormationStatusChanged?.Invoke(HasPlayerItem);
            }
        }
    
        public void SetInUseForFormation(bool inUseForFormation, bool isCalledFromFormationInit = false, bool isCalledFromParentView = false)
        {
            if (ViewOwnerOption != PlayerItemViewOwnerOption.StartingList) return;

            InUseForFormation = inUseForFormation;

            // NOTE: Do NOT clear player data here! The swap logic needs the data.
            // UpdateViewsAfterSwap() will handle clearing data for positions not in formation.

            // mainView handles all display logic now
            mainView.Show(); // Ensure the mainView is active
            mainView.UpdateView(); // This will trigger PlayerItemGeneralViewMode.UpdateView()

            if (!isCalledFromParentView && ParentPositionZoneView != null)
            {
                ParentPositionZoneView.SetInUseForFormation(inUseForFormation, isCalledFromChildView: true);
            }

            if(!isCalledFromFormationInit)
            {
                OnFormationStatusChanged?.Invoke(inUseForFormation);
            }

            Debug.Log($"SetInUseForFormation: PlayerItemView: {gameObject.name}, inUseForFormation: {inUseForFormation}");
        }
        
        // These methods control the root PlayerItemView GameObject's visibility
        public void BrightenMainView() => mainView.BrightenView();
        public void FadeMainView() => mainView.FadeView();
        public void Show() => gameObject.SetActive(true); 
        public void ToggleShow(bool show) => gameObject.SetActive(show);
        public void Hide() => gameObject.SetActive(false);

        // IPoolable implementation for LeanPool
        public void OnSpawn()
        {
            // Reset the view state when spawned from pool
            ResetPooledState();
        }

        public void OnDespawn()
        {
            // Clean up the view state before returning to pool
            CleanupForPool();
        }

        private void ResetPooledState()
        {
            // Reset indices
            StartingPlayersListIndex = -1;
            BenchPlayersListIndex = -1;
            FormationZonesListIndex = -1;
            ReservePlayersListIndex = -1;

            // Reset formation status
            InUseForFormation = false;
            
            // Show the view and ensure it's in a clean state
            gameObject.SetActive(true);
        }

        private void CleanupForPool()
        {
            // Clear player data
            SetPlayerData(null);
            
            // Reset controller reference
            Controller = null;
            
            // Reset parent zone reference
            ParentPositionZoneView = null;
            
            // Clear any event subscriptions to prevent memory leaks
            if (OnFormationStatusChanged != null)
            {
                foreach (System.Delegate d in OnFormationStatusChanged.GetInvocationList())
                {
                    OnFormationStatusChanged -= (System.Action<bool>)d;
                }
            }

            // Reset tactical position completely
            TacticalPosition = null;
            TacticalPositionOption = default;
        }

        public void Initialize(TacticBoardController controller, PositionZoneView zoneView,
            PlayerItemViewOwnerOption viewOwnerOption, int index)
        {
            Controller = controller;   
            ParentPositionZoneView = zoneView;
            ViewOwnerOption = viewOwnerOption;

            if (viewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                StartingPlayersListIndex = index;
                InitializeTacticalPosition();
            }
            else if (viewOwnerOption == PlayerItemViewOwnerOption.BenchList)
            {
                BenchPlayersListIndex = index;
            }
            else if (viewOwnerOption == PlayerItemViewOwnerOption.ReserveList)
            {
                ReservePlayersListIndex = index;
            }
        }
        
        /// <summary>
        /// Initialize the tactical position for this player view.
        /// Should be called after ParentPositionZoneView is set.
        /// </summary>
        public void InitializeTacticalPosition()
        {
            if (ViewOwnerOption != PlayerItemViewOwnerOption.StartingList)
                return;

            if (ParentPositionZoneView == null)
            {
                // Debug.LogError($"InitializeTacticalPosition: ParentPositionZoneView is null for {gameObject.name}");
                return;
            }

            TacticalPositionOption = ParentPositionZoneView.tacticalPositionOption;
            
            // Create the tactical position using the simpler constructor
            TacticalPosition = new TacticalPosition(
                TacticalPositionUtils.GetGroupForPosition(TacticalPositionOption),
                TacticalPositionOption
            );

            Debug.Log($"InitializeTacticalPosition: Created TacticalPosition for {TacticalPositionOption} with {TacticalPosition.AvailableRoles.Count} roles");
        }

        /// <summary>
        /// Ensures TacticalPosition is initialized before accessing it
        /// </summary>
        private void EnsureTacticalPositionInitialized()
        {
            if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList && TacticalPosition == null)
            {
                InitializeTacticalPosition();
            }
        }

        // METHODS TO TRIGGER EVENTS
        public void TriggerRoleChanged(TacticalRoleOption newRole)
        {
            OnRoleChanged?.Invoke(newRole);
        }

        public void TriggerDutyChanged(TacticalDutyOption newDuty)
        {
            OnDutyChanged?.Invoke(newDuty);
        }
    }
}