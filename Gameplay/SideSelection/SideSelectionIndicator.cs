using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay
{
    public class SideSelectionIndicator : MonoBehaviour
    {
        [Header("Indicator Visual")]
        [SerializeField] private Image indicatorImage;
        [SerializeField] private Color initialIndicatorColor = Color.red;
        [SerializeField] private Color confirmedIndicatorColor = Color.green;
        [SerializeField] private TextMeshProUGUI deviceTypeText;
        
        
        public void SetInitialColor() => SetColor(initialIndicatorColor);
        public void SetConfirmedColor() => SetColor(confirmedIndicatorColor);
        
        private void Start()
        {
            if (indicatorImage == null)
                indicatorImage = GetComponent<Image>();

            Debug.Log("[SideSelectionIndicator] Indicator initialized as prefab instance");
        }

        private void SetColor(Color color)
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

        public void ChangeDeviceTypeToMobile() => deviceTypeText.text = "Mobile";
        public void ChangeDeviceTypeToKeyboardMouse() => deviceTypeText.text = "Keyboard & Mouse";
        public void ChangeDeviceTypeToGamepad() => deviceTypeText.text = "Gamepad";
    }
}