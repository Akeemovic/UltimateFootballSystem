using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class SubstitutesListSection : MonoBehaviour
    {
        [SerializeField]
        public TextMeshProUGUI headerTextView;

        [SerializeField] public ScrollRect scrollRect;
    
        public void SetHeaderText(string text)
        {
            headerTextView.text = text;
        }

        public void DisableScroll()
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = false;
        }
    }
}