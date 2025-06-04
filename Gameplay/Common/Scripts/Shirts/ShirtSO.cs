using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Common
{
    [CreateAssetMenu(menuName = "Actionmatic/UPF/ShirtSO", fileName = "NewShirtSO")]
    public class ShirtSO : ScriptableObject
    {
        public long Guid;
    
        public ShirtType Type;
    
        /// <summary>
        /// Icon used to represent the shirt
        /// </summary>
        public Texture Icon;

        /// <summary>
        /// Texture applied on player models.
        /// </summary>
        public Texture ModelTexture;

        /// <summary>
        /// The Color which the shirt texture generally look like.
        /// </summary>
        public Color TextureColor;
    
        public Color TextColor;
    }
}