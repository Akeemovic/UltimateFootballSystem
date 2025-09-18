using UnityEngine.InputSystem;

namespace UltimateFootballSystem.Gameplay.InputManagement
{
    public class UIInputHandler : IInputHandler
    {
        private MainInputActions.UIActions _uiActions;

        public UIInputHandler(MainInputActions.UIActions actions)
        {
            _uiActions = actions;
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