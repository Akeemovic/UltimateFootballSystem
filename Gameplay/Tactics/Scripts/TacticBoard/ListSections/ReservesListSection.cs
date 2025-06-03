using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.ListSections
{
    public class ReservesListSection : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI headerTextView;

        [SerializeField] public ScrollRect scrollRect;
    
        [SerializeField] public ListSectionType SectionType;

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