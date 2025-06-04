namespace UltimateFootballSystem.Gameplay.Tactics
{
    public interface IPlayerItemViewMode
    {
        void Show();
        void Hide();
        // void UpdateView(Player profile);
        void SetDefaultView();
    }
}