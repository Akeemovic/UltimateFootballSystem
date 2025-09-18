using System;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Common
{
    public class GameStateManager : MonoBehaviour
    {
        public event Action<GameState> OnGameStateChange;
        [SerializeField]
        public GameState CurrentGameState { get; private set; }
     
        public static GameStateManager Instance { get; private set; }
        private void Awake()
        {
            if(Instance != null)
            {
                Debug.LogWarning("[GamerStateManager] Trying to instantiate a second instance of a singleton class.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public void SetState(GameState state) {
            CurrentGameState = state;
            OnGameStateChange?.Invoke(state);
            Debug.Log($"[GameStateManager] State changed to: {state}");
        }
    }
}