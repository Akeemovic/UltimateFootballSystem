using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay
{
    public class SideSelectionIndicator : MonoBehaviour
    {
        [Header("Indicator Visual")]
        [SerializeField] private Image indicatorImage;

        void Start()
        {
            if (indicatorImage == null)
                indicatorImage = GetComponent<Image>();

            Debug.Log("[SideSelectionIndicator] Indicator initialized as prefab instance");
        }

        public void SetColor(Color color)
        {
            if (indicatorImage != null)
            {
                indicatorImage.color = color;
            }
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}