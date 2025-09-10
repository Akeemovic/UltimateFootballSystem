using UnityEngine;
using Lean.Pool;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class LeanPoolTestHelper : MonoBehaviour
    {
        [Header("Pool Testing")]
        [SerializeField] private PlayerItemView testPrefab;
        [SerializeField] private Transform testContainer;
        [SerializeField] private int spawnCount = 10;
        
        [Space]
        [Header("Controls")]
        [SerializeField] private KeyCode spawnKey = KeyCode.Space;
        [SerializeField] private KeyCode despawnKey = KeyCode.Backspace;
        [SerializeField] private KeyCode clearKey = KeyCode.Delete;
        
        private PlayerItemView[] spawnedItems;

        private void Start()
        {
            if (testContainer == null)
                testContainer = transform;
                
            spawnedItems = new PlayerItemView[spawnCount];
            
            Debug.Log("LeanPool Test Helper initialized. Use the following keys:");
            Debug.Log($"- {spawnKey}: Spawn {spawnCount} PlayerItemViews");
            Debug.Log($"- {despawnKey}: Despawn all spawned PlayerItemViews");
            Debug.Log($"- {clearKey}: Clear all pools");
        }

        private void Update()
        {
            if (Input.GetKeyDown(spawnKey))
            {
                SpawnTestItems();
            }
            
            if (Input.GetKeyDown(despawnKey))
            {
                DespawnTestItems();
            }
            
            if (Input.GetKeyDown(clearKey))
            {
                ClearAllPools();
            }
        }

        [ContextMenu("Spawn Test Items")]
        public void SpawnTestItems()
        {
            if (testPrefab == null)
            {
                Debug.LogError("Test prefab is not assigned!");
                return;
            }

            Debug.Log($"Spawning {spawnCount} PlayerItemViews using LeanPool...");
            
            for (int i = 0; i < spawnCount; i++)
            {
                spawnedItems[i] = PlayerItemViewPoolManager.SpawnPlayerItemView(testPrefab, testContainer);
                
                if (spawnedItems[i] != null)
                {
                    spawnedItems[i].transform.localPosition = new Vector3(i * 50, 0, 0);
                    spawnedItems[i].name = $"Pooled_PlayerItemView_{i}";
                }
            }
            
            Debug.Log($"Successfully spawned {spawnCount} PlayerItemViews");
        }

        [ContextMenu("Despawn Test Items")]
        public void DespawnTestItems()
        {
            Debug.Log("Despawning all test PlayerItemViews...");
            
            int despawnedCount = 0;
            for (int i = 0; i < spawnedItems.Length; i++)
            {
                if (spawnedItems[i] != null)
                {
                    PlayerItemViewPoolManager.DespawnPlayerItemView(spawnedItems[i]);
                    spawnedItems[i] = null;
                    despawnedCount++;
                }
            }
            
            Debug.Log($"Despawned {despawnedCount} PlayerItemViews");
        }

        [ContextMenu("Clear All Pools")]
        public void ClearAllPools()
        {
            Debug.Log("Clearing all LeanPool pools...");
            LeanPool.DespawnAll();
            PlayerItemViewPoolManager.ClearAllPools();
            Debug.Log("All pools cleared");
        }

        [ContextMenu("Show Pool Statistics")]
        public void ShowPoolStatistics()
        {
            if (testPrefab != null)
            {
                LeanGameObjectPool pool = null;
                bool poolExists = LeanGameObjectPool.TryFindPoolByPrefab(testPrefab.gameObject, ref pool);
                
                Debug.Log($"Pool Statistics for {testPrefab.name}:");
                Debug.Log($"- Pool exists: {poolExists}");
                Debug.Log($"- Active in scene: {testContainer.childCount}");
                
                if (poolExists && pool != null)
                {
                    Debug.Log($"- Pool capacity: {pool.Capacity}");
                    Debug.Log($"- Pool preload: {pool.Preload}");
                }
            }
        }

        private void OnDestroy()
        {
            // Clean up any remaining test items
            DespawnTestItems();
        }
    }
}