// using System;
// using System.Linq;
// using UltimateFootballSystem.Core.Entities;
// using UltimateFootballSystem.Core.TacticsEngine;
// using UltimateFootballSystem.Core.TacticsEngine.Utils;
// using UltimateFootballSystem.Gameplay.Tactics.Tactics;
// using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
// using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
// using UnityEngine;
//
// namespace UltimateFootballSystem.Gameplay.Tactics
// {
//     /// <summary>
//     /// Player Profile View.
//     /// </summary>
//     [Serializable]
//     public class PlayerItemView : MonoBehaviour
//     {
//         public TacticsBoardController Controller;
//         
//         public PlayerItemViewMain mainView;
//         // REMOVE THIS FIELD: public PlayerItemViewPlaceholder placeholderView; // This no longer exists!
//
//         public PlayerItemViewSelectionLayer PlayerItemViewSelectionLayer;
//     
//         /// <summary>
//         /// ViewOwnerOption
//         /// </summary>
//         [NonSerialized]
//         public PlayerItemViewOwnerOption ViewOwnerOption;
//     
//         /// <summary>
//         /// Parent Zone tacticsPitch.
//         /// </summary>
//         [NonSerialized]
//         public PositionZoneView ParentPositionZoneView;
//     
//         public event Action<bool> OnFormationStatusChanged;
//
//         /// <summary>
//         /// Indicates whether the profile is in use for formation.
//         /// </summary>
//         public bool InUseForFormation { get; private set; }
//
//         /// <summary>
//         /// Indicates whether the player profile has valid data.
//         /// </summary>
//         public bool HasPlayerItem;
//
//         /// <summary>
//         /// Starting Players ListIndex index.
//         /// </summary>
//         [HideInInspector]
//         [NonSerialized]
//         public int StartingPlayersListIndex = -1;
//     
//         /// <summary>
//         /// Substitutes Players ListIndex index.
//         /// </summary>
//         [HideInInspector]
//         [NonSerialized]
//         public int BenchPlayersListIndex = -1;
//     
//         /// <summary>
//         /// Formation Zones List Index index.
//         /// </summary>
//         [HideInInspector]
//         [NonSerialized]
//         public int FormationZonesListIndex = -1;
//     
//         /// <summary>
//         /// Reserve Players ListIndex index.
//         /// </summary>
//         [HideInInspector]
//         [NonSerialized]
//         public int ReservePlayersListIndex = -1;
//     
//         /// <summary>
//         /// Player Profile.
//         /// </summary>
//         public Core.Entities.Player Profile { get; protected set; }
//     
//         /// <summary>
//         /// Tactical Position Option
//         /// </summary>
//         public TacticalPositionOption TacticalPositionOption { get; set; }
//     
//         public TacticalPosition TacticalPosition;
//
//         private void OnEnable()
//         {
//             // TacticalPosition = new TacticalPosition(
//             //     TacticalPositionUtils.GetGroupForPosition(TacticalPositionOption),
//             //     TacticalPositionOption,
//             //     RoleManager.GetRolesForPosition(TacticalPositionOption)
//             //         .Select(roleOption => RoleManager.GetRole(roleOption))
//             //         .ToList()
//             // );
//         }
//
//         private void Awake()
//         {
//             if (ParentPositionZoneView != null)
//             {
//                 TacticalPositionOption = ParentPositionZoneView.tacticalPositionOption;
//                 Debug.Log($"Awake: TacticalPositionOption is set to {TacticalPositionOption}");
//             }
//             else
//             {
//                 Debug.LogWarning("Awake: ParentPositionZoneView is null");
//             }
//
//             Profile = new Core.Entities.Player() { Id = 0, Name = null, SquadNumber = null, CurrentAbility = 0 };
//             HasPlayerItem = false;
//         }
//
//         private void Start()
//         {
//             if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
//             {
//                 var roles = RoleManager.GetRolesForPosition(TacticalPositionOption);
//                 Debug.Log(
//                     $"Awake: TacticalPositionOption {TacticalPositionOption} has {roles.Count} roles: {string.Join(", ", roles.Select(r => r.ToString()))}");
//
//                 Debug.Log($"Awake: TacticalPositionOption is set to {TacticalPositionOption}");
//             }
//         }
//
//         /// <summary>
//         /// Get drag data.
//         /// </summary>
//         /// <returns>Drag data.</returns>
//         public virtual PlayerItemDragData GetDragData()
//         {
//             return ViewOwnerOption switch
//             {
//                 PlayerItemViewOwnerOption.BenchList => PlayerItemDragData.Bench(Profile, BenchPlayersListIndex),
//                 PlayerItemViewOwnerOption.ReserveList => PlayerItemDragData.Reserve(Profile, ReservePlayersListIndex),
//                 _ => PlayerItemDragData.Starting(Profile, StartingPlayersListIndex)
//             };
//         }
//
//         /// <summary>
//         /// Set data.
//         /// </summary>
//         /// <param name="profile">Player.</param>
//         public virtual void SetPlayerData(Player profile)
//         {
//             Profile = profile;
//             bool oldHasPlayerItem = HasPlayerItem; 
//             if (Profile != null && !string.IsNullOrEmpty(Profile.Name) && Profile.Id >= 1)
//             {
//                 HasPlayerItem = true;
//                 Debug.Log("View owner:" +ViewOwnerOption);
//                 if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
//                 {
//                     TacticalPosition.AssignedPlayerId = Profile.Id;
//                 }
//                 Debug.Log("SetPlayerData: Data reaching profile tacticsPitch belongs to: " + Profile.Name);
//             }
//             else
//             {
//                 HasPlayerItem = false;
//                 if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
//                 {
//                     TacticalPosition.AssignedPlayerId = null;
//                 }
//                 Profile = new Player(); 
//                 Debug.Log("SetPlayerData: Data reaching profile tacticsPitch is null: ");
//             }
//             
//             mainView.UpdateView(); // Always update mainView which now handles all visual states
//             
//             if (oldHasPlayerItem != HasPlayerItem)
//             {
//                 OnFormationStatusChanged?.Invoke(HasPlayerItem);
//             }
//         }
//     
//         public void SetInUseForFormation(bool inUseForFormation, bool isCalledFromFormationInit = false, bool isCalledFromParentView = false)
//         {
//             if (ViewOwnerOption != PlayerItemViewOwnerOption.StartingList) return;
//
//             InUseForFormation = inUseForFormation;
//
//             mainView.UpdateView(); // Call the method to manage visibility and placeholder state
//
//             if (!isCalledFromParentView)
//             {
//                 ParentPositionZoneView.SetInUseForFormation(inUseForFormation, isCalledFromChildView: true);
//             }
//
//             if(!isCalledFromFormationInit)
//             {
//                 OnFormationStatusChanged?.Invoke(inUseForFormation);
//             }
//
//             Debug.Log($"SetInUseForFormation: PlayerItemView: {gameObject.name}, inUseForFormation: {inUseForFormation}");
//         }
//         
//         public void BrightenMainView() => mainView.BrightenView();
//         public void FadeMainView() => mainView.FadeView();
//         public void Show() => gameObject.SetActive(true); // This controls the root PlayerItemView GameObject
//         public void ToggleShow(bool show) => gameObject.SetActive(show);
//         public void Hide() => gameObject.SetActive(false); // This controls the root PlayerItemView GameObject
//     }
// }

using System;
using System.Linq;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UltimateFootballSystem.Gameplay.Tactics.Tactics;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
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
        public TacticsBoardController Controller;
    
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
            if (ParentPositionZoneView != null)
            {
                TacticalPositionOption = ParentPositionZoneView.tacticalPositionOption;
                Debug.Log($"Awake: TacticalPositionOption is set to {TacticalPositionOption}");
            }
            else
            {
                Debug.LogWarning("Awake: ParentPositionZoneView is null");
            }

            
            // Profile = new Core.Entities.Player() { Id = 0, Name = null, SquadNumber = null, CurrentAbility = 0 };
            // HasPlayerItem = false;
            SetInUseForFormation(false);
            SetPlayerData(null);
        }

        private void Start()
        {
            if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                var roles = RoleManager.GetRolesForPosition(TacticalPositionOption);
                Debug.Log(
                    $"Awake: TacticalPositionOption {TacticalPositionOption} has {roles.Count} roles: {string.Join(", ", roles.Select(r => r.ToString()))}");

                Debug.Log($"Awake: TacticalPositionOption is set to {TacticalPositionOption}");
            }
            
            SetInUseForFormation(false);
            SetPlayerData(null);
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
                Debug.Log("View owner:" +ViewOwnerOption);
                if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    TacticalPosition.AssignedPlayerId = Profile.Id;
                }
                Debug.Log("SetPlayerData: Data reaching profile tacticsPitch belongs to: " + Profile.Name);
            }
            else
            {
                HasPlayerItem = false;
                if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    TacticalPosition.AssignedPlayerId = null;
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

            // mainView handles all display logic now
            mainView.Show(); // Ensure the mainView is active
            mainView.UpdateView(); // This will trigger PlayerItemGeneralViewMode.UpdateView()

            if (!isCalledFromParentView)
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

            // Reset tactical position
            if (TacticalPosition != null)
            {
                TacticalPosition.AssignedPlayerId = null;
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