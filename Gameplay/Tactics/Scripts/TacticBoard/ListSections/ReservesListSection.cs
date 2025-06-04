using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class ReservesListSection : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI headerTextView;

        [SerializeField] public ScrollRect scrollRect;
    
        public ListSectionType sectionType;

        [SerializeField] public Transform viewsContainer;
    
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