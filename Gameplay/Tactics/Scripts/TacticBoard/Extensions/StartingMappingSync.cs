using System.Collections.Generic;
using UltimateFootballSystem.Core.TacticsEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    /// <summary>
    /// Lightweight extension for syncing StartingPositionPlayerMapping when needed
    /// </summary>
    public static class StartingMappingSync
    {
        /// <summary>
        /// Sync StartingPositionPlayerMapping from current UI state - call only when you need the mapping to be accurate
        /// </summary>
        public static void SyncStartingPositionPlayerMappingFromUI(this TacticsBoardController controller)
        {
            if (controller.startingPlayersViews == null) return;

            // Clear and rebuild StartingPositionPlayerMapping from current UI state
            controller.StartingPositionPlayerMapping.Clear();
            
            foreach (var view in controller.startingPlayersViews)
            {
                if (view?.ParentPositionZoneView == null) continue;
                
                var position = view.ParentPositionZoneView.tacticalPositionOption;
                var player = view.HasPlayerItem ? view.Profile : null;
                
                controller.StartingPositionPlayerMapping[position] = player;
            }
        }

        /// <summary>
        /// Get current StartingPositionPlayerMapping without modifying the stored one - useful for read-only operations
        /// </summary>
        public static Dictionary<TacticalPositionOption, Core.Entities.Player?> GetCurrentStartingPositionPlayerMapping(this TacticsBoardController controller)
        {
            var mapping = new Dictionary<TacticalPositionOption, Core.Entities.Player?>();
            
            if (controller.startingPlayersViews == null) return mapping;
            
            foreach (var view in controller.startingPlayersViews)
            {
                if (view?.ParentPositionZoneView == null) continue;
                
                var position = view.ParentPositionZoneView.tacticalPositionOption;
                var player = view.HasPlayerItem ? view.Profile : null;
                
                mapping[position] = player;
            }
            
            return mapping;
        }

        /// <summary>
        /// Sync StartingPositionPlayerMapping after drag/drop operations - lightweight hook
        /// </summary>
        public static void SyncStartingPositionPlayerMappingAfterSwap(this TacticsBoardController controller)
        {
            // Only sync StartingPositionPlayerMapping if it's actually used somewhere
            // For most operations, UI views are the source of truth
            controller.SyncStartingPositionPlayerMappingFromUI();
        }
    }
}