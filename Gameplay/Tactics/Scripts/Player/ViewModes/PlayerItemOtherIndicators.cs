using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PlayerItemOtherIndicators : MonoBehaviour
    {
        [SerializeField]
        public Image BookingIndicatorImage;
    
        [SerializeField]
        public Image SubstitutionStatusIndicatorImage;

        public void UpdateView(PlayerItemView view)
        {
            if (view.HasPlayerItem)
            {
                var profile = view.Profile;
            }
            else
            {
                SetDefaultView();
            }
        }
    
        public void SetDefaultView()
        {
            gameObject.SetActive(false);
        }
    }
}