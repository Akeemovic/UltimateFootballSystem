using UltimateFootballSystem.Core.TacticsEngine;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player.Drag_and_Drop_Support
{
    /// <summary>
    /// Player Profile drag data.
    /// </summary>
    public class PlayerItemDragData
    {
        /// <summary>
        /// Player Profile.
        /// </summary>
        public Core.Entities.Player Profile { get; }

        /// <summary>
        /// Drag Source View Reference.
        /// </summary>
        public PlayerItemView DragSourceView { get; private set; }

        /// <summary>
        /// Drop Target View Reference.
        /// </summary>
        public PlayerItemView DropTargetView { get; private set; }

        /// <summary>
        /// Starting Players List index.
        /// </summary>
        public int StartingPlayersListIndex { get; }

        /// <summary>
        /// Substitutes Players List index.
        /// </summary>
        public int BenchPlayersListIndex { get; }

        /// <summary>
        /// Reserve Players List index.
        /// </summary>
        public int ReservePlayersListIndex { get; }

        /// <summary>
        /// Indicates whether the player is in use for formation.
        /// </summary>
        public bool InUseForFormation
        {
            get
            {
                if (DragSourceView != null)
                {
                    Debug.Log($"DragSourceView.InUseForFormation: {DragSourceView.InUseForFormation}");
                    return DragSourceView.InUseForFormation;
                }
                if (DropTargetView != null)
                {
                    Debug.Log($"DropTargetView.InUseForFormation: {DropTargetView.InUseForFormation}");
                    return DropTargetView.InUseForFormation;
                }
                return false;
            }
        }

        /// <summary>
        /// Tactical Position Option.
        /// </summary>
        public TacticalPositionOption TacticalPositionOption
        {
            get
            {
                if (DragSourceView != null && DragSourceView.ParentPositionZoneView != null)
                {
                    Debug.Log($"TacticalPositionOption: {DragSourceView.ParentPositionZoneView.tacticalPositionOption}");
                    return DragSourceView.ParentPositionZoneView.tacticalPositionOption;
                }

                Debug.LogWarning("TacticalPositionOption: DragSourceView or ParentPositionZoneView is null");
                return TacticalPositionOption.None;
            }
        }

        /// <summary>
        /// Empty instance.
        /// </summary>
        public static PlayerItemDragData Empty() 
        { 
            return new PlayerItemDragData(null, -1, -1, -1)
            {
                DragSourceView = null,
                DropTargetView = null,
            };
        }

        private PlayerItemDragData(Core.Entities.Player profile, int startingPlayersListIndex, int benchPlayersListIndex, int reservePlayersListIndex)
        {
            Profile = profile;
            StartingPlayersListIndex = startingPlayersListIndex;
            BenchPlayersListIndex = benchPlayersListIndex;
            ReservePlayersListIndex = reservePlayersListIndex;
        }

        public void SetDragSourceViewReference(PlayerItemView sourceView)
        {
            DragSourceView = sourceView;
            // Debug.Log($"SetDragSourceViewReference: {sourceView.InUseForFormation}");
        }

        public void SetDropTargetViewReference(PlayerItemView targetView)
        {
            DropTargetView = targetView;
            // Debug.Log($"SetDropTargetViewReference: {targetView.InUseForFormation}");
        }

        /// <summary>
        /// Create Starting Players List profile drag data instance.
        /// </summary>
        /// <param name="player">Player Profile.</param>
        /// <param name="startingPlayersListIndex">Starting Players List index.</param>
        /// <returns>Drag data.</returns>
        public static PlayerItemDragData Starting(Core.Entities.Player player, int startingPlayersListIndex)
        {
            return new PlayerItemDragData(player, startingPlayersListIndex, -1, -1);
        }

        /// <summary>
        /// Create Substitutes Players List profile drag data instance.
        /// </summary>
        /// <param name="player">Player Profile.</param>
        /// <param name="benchPlayersListIndex">Substitutes Players List index.</param>
        /// <returns>Drag data.</returns>
        public static PlayerItemDragData Bench(Core.Entities.Player player, int benchPlayersListIndex)
        {
            return new PlayerItemDragData(player, -1, benchPlayersListIndex, -1);
        }

        /// <summary>
        /// Create Reserve Players List profile drag data instance.
        /// </summary>
        /// <param name="player">Player Profile.</param>
        /// <param name="reservePlayersListIndex">Reserve Players List index.</param>
        /// <returns>Drag data.</returns>
        public static PlayerItemDragData Reserve(Core.Entities.Player player, int reservePlayersListIndex)
        {
            return new PlayerItemDragData(player, -1, -1, reservePlayersListIndex);
        }

        /// <summary>
        /// Check whether the player profile is valid.
        /// </summary>
        /// <returns>True if the player profile is valid.</returns>
        public bool IsValidPlayer()
        {
            return Profile != null && !string.IsNullOrEmpty(Profile.Name);
        }

        /// <summary>
        /// Check whether current instance and target instance are the same.
        /// </summary>
        /// <param name="target">Target PlayerItemDragData.</param>
        /// <returns>True, if instance and target are the same.</returns>
        public bool IsSamePositionAs(PlayerItemDragData target)
        {
            if (target == null)
            {
                return false;
            }

            return (StartingPlayersListIndex >= 0 && target.StartingPlayersListIndex == StartingPlayersListIndex) || 
                   (BenchPlayersListIndex >= 0 && target.BenchPlayersListIndex == BenchPlayersListIndex) || 
                   (ReservePlayersListIndex >= 0 && target.ReservePlayersListIndex == ReservePlayersListIndex);
        }
    }
}
