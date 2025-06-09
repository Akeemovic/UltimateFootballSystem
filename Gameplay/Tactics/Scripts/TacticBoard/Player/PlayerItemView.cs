using System;
using System.Linq;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UltimateFootballSystem.Gameplay.Tactics.Tactics;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player;
using UltimateFootballSystem.Gameplay.Tactics.Tactics.Player.Drag_and_Drop_Support;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    /// <summary>
    /// Player Profile View.
    /// </summary>
    [Serializable]
    public class PlayerItemView : MonoBehaviour
    {
        // public TacticsBoardController Controller;
        //
        // public PlayerItemViewMain mainView;
        // public PlayerItemViewPlaceholder placeholderView;
        //
        // public PlayerItemViewSelectionLayer PlayerItemViewSelectionLayer;
        //
        // /// <summary>
        // /// ViewOwnerOption
        // /// </summary>
        // [NonSerialized]
        // public PlayerItemViewOwnerOption ViewOwnerOption;
        //
        // /// <summary>
        // /// Parent Zone tacticsPitch.
        // /// </summary>
        // [NonSerialized]
        // public PositionZoneView ParentPositionZoneView;
        //
        // public event Action<bool> OnFormationStatusChanged;
        //
        // /// <summary>
        // /// Indicates whether the profile is in use for formation.
        // /// </summary>
        // public bool InUseForFormation { get; private set; }
        //
        // /// <summary>
        // /// Indicates whether the player profile has valid data.
        // /// </summary>
        // // [NonSerialized]
        // public bool HasPlayerItem;
        //
        // /// <summary>
        // /// Starting Players ListIndex index.
        // /// </summary>
        // [HideInInspector]
        // [NonSerialized]
        // public int StartingPlayersListIndex = -1;
        //
        // /// <summary>
        // /// Substitutes Players ListIndex index.
        // /// </summary>
        // [HideInInspector]
        // [NonSerialized]
        // public int BenchPlayersListIndex = -1;
        //
        // /// <summary>
        // /// Formation Zones List Index index.
        // /// </summary>
        // [HideInInspector]
        // [NonSerialized]
        // public int FormationZonesListIndex = -1;
        //
        // /// <summary>
        // /// Reserve Players ListIndex index.
        // /// </summary>
        // [HideInInspector]
        // [NonSerialized]
        // public int ReservePlayersListIndex = -1;
        //
        // /// <summary>
        // /// Player Profile.
        // /// </summary>
        // public Core.Entities.Player Profile { get; protected set; }
        //
        // /// <summary>
        // /// Tactical Position Option
        // /// </summary>
        // public TacticalPositionOption TacticalPositionOption { get; set; }
        //
        // public TacticalPosition TacticalPosition;
        //
        // private void OnEnable()
        // {
        //     // TacticalPosition = new TacticalPosition(
        //     //     TacticalPositionUtils.GetGroupForPosition(TacticalPositionOption),
        //     //     TacticalPositionOption,
        //     //     RoleManager.GetRolesForPosition(TacticalPositionOption)
        //     //         .Select(roleOption => RoleManager.GetRole(roleOption))
        //     //         .ToList()
        //     // );
        // }
        //
        // private void Awake()
        // {
        //     if (ParentPositionZoneView != null)
        //     {
        //         TacticalPositionOption = ParentPositionZoneView.tacticalPositionOption;
        //         Debug.Log($"Awake: TacticalPositionOption is set to {TacticalPositionOption}");
        //     }
        //     else
        //     {
        //         Debug.LogWarning("Awake: ParentPositionZoneView is null");
        //     }
        //
        //     Profile = new Core.Entities.Player() { Id = 0, Name = null, SquadNumber = null, CurrentAbility = 0 };
        //     HasPlayerItem = false;
        //
        //     // if (ViewOwnerOption == PlayerItemViewOwnerOption.BenchList && HasPlayerItem == false)
        //     // {
        //     //     placeholderView.Show();
        //     //     mainView.Hide();
        //     // }
        // }
        //
        // private void Start()
        // {
        //     if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
        //     {
        //         var roles = RoleManager.GetRolesForPosition(TacticalPositionOption);
        //         Debug.Log(
        //             $"Awake: TacticalPositionOption {TacticalPositionOption} has {roles.Count} roles: {string.Join(", ", roles.Select(r => r.ToString()))}");
        //
        //         Debug.Log($"Awake: TacticalPositionOption is set to {TacticalPositionOption}");
        //     }
        // }
        //
        // /// <summary>
        // /// Get drag data.
        // /// </summary>
        // /// <returns>Drag data.</returns>
        // public virtual PlayerItemDragData GetDragData()
        // {
        //     return ViewOwnerOption switch
        //     {
        //         PlayerItemViewOwnerOption.BenchList => PlayerItemDragData.Bench(Profile, BenchPlayersListIndex),
        //         PlayerItemViewOwnerOption.ReserveList => PlayerItemDragData.Reserve(Profile, ReservePlayersListIndex),
        //         _ => PlayerItemDragData.Starting(Profile, StartingPlayersListIndex)
        //     };
        // }
        //
        // // Gemini 1.0
        // /// <summary>
        // /// Set data.
        // /// </summary>
        // /// <param name="profile">Player.</param>
        // public virtual void SetPlayerData(Core.Entities.Player profile)
        // {
        //     Profile = profile;
        //     bool oldHasPlayerItem = HasPlayerItem; // Store previous state
        //     if (Profile != null && !string.IsNullOrEmpty(Profile.Name) && Profile.Id >= 1)
        //     {
        //         HasPlayerItem = true;
        //         Debug.Log("View owner:" +ViewOwnerOption);
        //         if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
        //         {
        //             TacticalPosition.AssignedPlayerId = Profile.Id;
        //         }
        //         Debug.Log("SetPlayerData: Data reaching profile tacticsPitch belongs to: " + Profile.Name);
        //     }
        //     else
        //     {
        //         HasPlayerItem = false;
        //         if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
        //         {
        //             TacticalPosition.AssignedPlayerId = null;
        //         }
        //         Profile = new Core.Entities.Player(); // Ensure Profile is never null for default values
        //         Debug.Log("SetPlayerData: Data reaching profile tacticsPitch is null: ");
        //     }
        //
        //     // Centralized logic for showing/hiding mainView and placeholderView
        //     if (HasPlayerItem)
        //     {
        //         mainView.Show();
        //         placeholderView.Hide();
        //     }
        //     else
        //     {
        //         // If no player, determine which placeholder to show based on owner option
        //         // This is where we ensure the correct placeholder type is displayed
        //         if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList && InUseForFormation) // Show starting placeholder if in formation but no player
        //         {
        //             mainView.Hide();
        //             placeholderView.Show(); // PlayerItemViewPlaceholder will handle which specific image/text to show
        //         }
        //         else if (ViewOwnerOption == PlayerItemViewOwnerOption.BenchList) // Show bench placeholder if in bench and no player
        //         {
        //             mainView.Hide();
        //             placeholderView.Show(); // PlayerItemViewPlaceholder will handle which specific image/text to show
        //         }
        //         else // For other cases, or if not in formation/bench, just hide both or show default
        //         {
        //             mainView.Hide();
        //             placeholderView.Hide(); // Or decide on a default hidden state
        //         }
        //     }
        //     
        //     mainView.UpdateView(); // Always update mainView to reflect current player data or default state
        //     
        //     // Only invoke if the formation status actually changed for this item
        //     if (oldHasPlayerItem != HasPlayerItem)
        //     {
        //         OnFormationStatusChanged?.Invoke(HasPlayerItem);
        //     }
        // }
        // // Gemini 1.0
        // public void SetInUseForFormation(bool inUseForFormation, bool isCalledFromFormationInit = false, bool isCalledFromParentView = false)
        // {
        //     if (ViewOwnerOption != PlayerItemViewOwnerOption.StartingList) return;
        //
        //     InUseForFormation = inUseForFormation;
        //
        //     // Instead of directly manipulating mainView/placeholderView here,
        //     // call SetPlayerData with the current profile to re-evaluate visibility
        //     // based on both InUseForFormation and HasPlayerItem
        //     SetPlayerData(Profile); 
        //
        //     if (!isCalledFromParentView)
        //     {
        //         ParentPositionZoneView.SetInUseForFormation(inUseForFormation, isCalledFromChildView: true);
        //     }
        //
        //     if(!isCalledFromFormationInit)
        //     {
        //         OnFormationStatusChanged?.Invoke(inUseForFormation);
        //     }
        //
        //     Debug.Log($"SetInUseForFormation: PlayerItemView: {gameObject.name}, inUseForFormation: {inUseForFormation}");
        // }
        
        public TacticsBoardController Controller;
    
        public PlayerItemViewMain mainView;
        public PlayerItemViewPlaceholder placeholderView;

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
        // [NonSerialized]
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

            Profile = new Core.Entities.Player() { Id = 0, Name = null, SquadNumber = null, CurrentAbility = 0 };
            HasPlayerItem = false;
        
            // if (ViewOwnerOption == PlayerItemViewOwnerOption.BenchList && HasPlayerItem == false)
            // {
            //     placeholderView.Show();
            //     mainView.Hide();
            // }
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
        
            UpdateViewVisibility(); // Call the new method to manage visibility
            
            mainView.UpdateView(); 
            
            if (oldHasPlayerItem != HasPlayerItem)
            {
                OnFormationStatusChanged?.Invoke(HasPlayerItem);
            }
        }
    
        public void SetInUseForFormation(bool inUseForFormation, bool isCalledFromFormationInit = false, bool isCalledFromParentView = false)
        {
            if (ViewOwnerOption != PlayerItemViewOwnerOption.StartingList) return;

            InUseForFormation = inUseForFormation;

            UpdateViewVisibility(); // Call the new method to manage visibility

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

        // /// <summary>
        // /// Manages the visibility of mainView and placeholderView based on current state.
        // /// </summary>
        // private void UpdateViewVisibility()
        // {
        //     if (HasPlayerItem)
        //     {
        //         mainView.Show();
        //         placeholderView.Hide();
        //     }
        //     else
        //     {
        //         if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList && InUseForFormation)
        //         {
        //             mainView.Hide();
        //             placeholderView.Show();
        //         }
        //         else if (ViewOwnerOption == PlayerItemViewOwnerOption.BenchList)
        //         {
        //             mainView.Hide();
        //             placeholderView.Show();
        //         }
        //         else
        //         {
        //             mainView.Hide();
        //             placeholderView.Hide();
        //         }
        //     }
        //     // Ensure placeholder updates its text when shown
        //     if (placeholderView.isActiveAndEnabled)
        //     {
        //         placeholderView.UpdatePositionText();
        //     }
        // }
        /// <summary>
        /// Manages the visibility of mainView and placeholderView based on current state.
        /// </summary>
        private void UpdateViewVisibility()
        {
            if (HasPlayerItem)
            {
                // If player exists, always show main view and hide placeholder
                mainView.Show();
                placeholderView.Hide();
            }
            else // No player item
            {
                // If no player, show placeholder if it's a starting list position OR a bench position
                if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    mainView.Hide();
                    placeholderView.Show();
                }
                else if (ViewOwnerOption == PlayerItemViewOwnerOption.DragAndDrop)
                {
                    mainView.Hide();
                    placeholderView.Show();
                } 
                else if (ViewOwnerOption == PlayerItemViewOwnerOption.BenchList)
                {
                    mainView.Hide();
                    placeholderView.Show();
                }
                else // For other owner options (e.g., ReserveList), hide
                {
                    mainView.Hide();
                    placeholderView.Hide();
                }
            }
    
            // Always ensure placeholder updates its text if it is going to be shown.
            // Calling it unconditionally here ensures the text is set whenever visibility is updated,
            // and the placeholder's own logic will handle if it's actually active.
            // Alternatively, you can keep the check if you only want it to run when active,
            // but the issue was it was being hidden.
            if (placeholderView.isActiveAndEnabled) // Keep this check if you prefer to only update if active
            {
                placeholderView.UpdatePositionText();
            }
            // Or simply: placeholderView.UpdatePositionText(); if you want it to set text regardless of active state
        }
        
        public void BrightenMainView() => mainView.BrightenView();
        public void FadeMainView() => mainView.FadeView();
        public void Show() => gameObject.SetActive(true);
        public void ToggleShow(bool show) => gameObject.SetActive(show);
        public void Hide() => gameObject.SetActive(false);
    }
}
