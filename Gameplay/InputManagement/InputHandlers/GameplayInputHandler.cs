using UnityEngine.InputSystem;

namespace UltimateFootballSystem.Gameplay.InputManagement
{
    public class GameplayInputHandler : IInputHandler
    {
        private MainInputActions.GameplayActions gameplayActions;

        public GameplayInputHandler(MainInputActions.GameplayActions actions)
        {
            gameplayActions = actions;
        }

        public void RegisterInputActions()
        {
            throw new System.NotImplementedException();
        }

        public void Cleanup()
        {
            throw new System.NotImplementedException();
        }
    }
}