namespace UltimateFootballSystem.Common.Scripts.Utils
{
    public class UFSGlobal
    {
        public static readonly UFSGlobal _instance = new UFSGlobal();
        public static UFSGlobal Instance { get { return _instance; } }
        public static string AssetRoot = "Actionmatic/UPF/ShirtSO";
        private UFSGlobal() { }
    }
}