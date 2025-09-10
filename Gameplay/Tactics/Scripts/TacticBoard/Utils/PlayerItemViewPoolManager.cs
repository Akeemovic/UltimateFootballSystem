using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public static class PlayerItemViewPoolManager
    {
        public static PlayerItemView SpawnPlayerItemView(PlayerItemView prefab, Transform parent)
        {
            if (prefab == null)
            {
                Debug.LogError("Attempting to spawn null PlayerItemView prefab");
                return null;
            }

            return LeanPool.Spawn(prefab, parent);
        }

        public static void DespawnPlayerItemView(PlayerItemView view)
        {
            if (view == null || view.gameObject == null) return;

            // Clear the player data before despawning
            view.SetPlayerData(null);
            
            // Reset any view-specific state
            ResetViewState(view);
            
            // Despawn using LeanPool
            LeanPool.Despawn(view);
        }

        public static void DespawnAllInContainer(Transform container)
        {
            if (container == null) return;

            var children = new List<Transform>();
            for (int i = 0; i < container.childCount; i++)
            {
                children.Add(container.GetChild(i));
            }

            foreach (var child in children)
            {
                var playerView = child.GetComponent<PlayerItemView>();
                if (playerView != null)
                {
                    DespawnPlayerItemView(playerView);
                }
            }
        }

        private static void ResetViewState(PlayerItemView view)
        {
            // Reset any state that shouldn't persist between spawns
            view.BenchPlayersListIndex = -1;
            view.ReservePlayersListIndex = -1;
            view.StartingPlayersListIndex = -1;
            
            // Event cleanup is handled within PlayerItemView's CleanupForPool method
            // We cannot access events directly from outside the class
        }

        public static void ClearAllPools()
        {
            // LeanPool will handle its own cleanup automatically
        }
    }
}