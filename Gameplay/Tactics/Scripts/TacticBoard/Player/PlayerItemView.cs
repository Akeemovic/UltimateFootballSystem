using System;
using System.Linq;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.Drag_and_Drop_Support;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.Options;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player
{
    /// <summary>
    /// Player Profile View.
    /// </summary>
    [Serializable]
    public class PlayerItemView : MonoBehaviour
    {
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
        /// Parent Zone view.
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
            //     PositionGroupManager.GetGroupForPosition(TacticalPositionOption),
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
            if (Profile != null && !string.IsNullOrEmpty(Profile.Name) && Profile.Id >= 1)
            {
                HasPlayerItem = true;
                Debug.Log("View owner:" +ViewOwnerOption);
                if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    TacticalPosition.AssignedPlayerId = Profile.Id;
                }
                Debug.Log("SetPlayerData: Data reaching profile view belongs to: " + Profile.Name);
            }
            else
            {
                HasPlayerItem = false;
                if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
                {
                    TacticalPosition.AssignedPlayerId = null;
                }
                Profile = new Core.Entities.Player();
                Debug.Log("SetPlayerData: Data reaching profile view is null: ");
            }
        
            // Performs player/position mapping updates 
            if (ViewOwnerOption == PlayerItemViewOwnerOption.StartingList)
            {
                OnFormationStatusChanged?.Invoke(true);
            }

            if (ViewOwnerOption == PlayerItemViewOwnerOption.BenchList)
            {
                if (HasPlayerItem)
                {
                    if(!mainView.isActiveAndEnabled) mainView.Show();
                    if(placeholderView.isActiveAndEnabled) placeholderView.Hide();
                }
                else
                {
                    if(mainView.isActiveAndEnabled) mainView.Hide();
                    if(!placeholderView.isActiveAndEnabled) placeholderView.Show();
                }
            }
        
            mainView.UpdateView();
            // mainView.UpdateViewAsync();
        }
    
        public void SetInUseForFormation(bool inUseForFormation, bool isCalledFromFormationInit = false, bool isCalledFromParentView = false)
        {
            if (ViewOwnerOption != PlayerItemViewOwnerOption.StartingList) return;

            InUseForFormation = inUseForFormation;

            mainView.Show();
            placeholderView.ToggleShow(!inUseForFormation);

            if (!isCalledFromParentView)
            {
                ParentPositionZoneView.SetInUseForFormation(inUseForFormation, isCalledFromChildView: true);
            }

            if(!isCalledFromFormationInit)
            {
                // Invoke the OnFormationStatusChanged event
                // OnFormationStatusChanged?.Invoke(inUseForFormation, isCalledFromFormationInit);
                OnFormationStatusChanged?.Invoke(inUseForFormation);
            }

            Debug.Log($"SetInUseForFormation: PlayerItemView: {gameObject.name}, inUseForFormation: {inUseForFormation}");
        }
    
        // public async UniTask SetInUseForFormationAsync(bool inUseForFormation, bool isCalledFromParentView = false)
        // {
        //     if (ViewOwnerOption != PlayerItemViewOwnerOption.StartingList) return;
        //     
        //     InUseForFormation = inUseForFormation;
        //
        //     mainView.gameObject.SetActive(inUseForFormation);
        //     placeholderView.gameObject.SetActive(!inUseForFormation);
        //
        //     if (!isCalledFromParentView)
        //     {
        //         await ParentPositionZoneView.SetInUseForFormationAsync(inUseForFormation, true);
        //     }
        //     
        //     Debug.Log($"SetInUseForFormationAsync: PlayerItemView: {gameObject.name}, inUseForFormation: {inUseForFormation}");
        // }
    
        public void BrightenMainView()
        {
            mainView.BrightenView();
        }
    
        public void FadeMainView()
        {
            mainView.FadeView();
        }
    
        public void Show()
        {
            gameObject.SetActive(true);
        }
    
        public void ToggleShow(bool show)
        {
            gameObject.SetActive(show);
        }
    
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
