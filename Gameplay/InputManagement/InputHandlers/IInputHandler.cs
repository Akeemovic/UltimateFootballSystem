namespace UltimateFootballSystem.Gameplay.InputManagement
{
    public interface IInputHandler
    {
        void RegisterInputActions();
        void Cleanup();
    }
}