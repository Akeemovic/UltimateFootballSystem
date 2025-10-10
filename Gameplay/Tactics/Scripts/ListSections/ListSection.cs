using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class ListSection : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI headerTextView;
        [SerializeField] public ScrollRect scrollRect;
        [SerializeField] public ListSectionType SectionType;
        [SerializeField] public Transform viewsContainer;
    
        private string headerTextFormat;
        private string defaultText;

        public void SetHeaderTextFormat(string format, string defaultText = "")
        {
            headerTextFormat = format;
            this.defaultText = defaultText;
            UpdateFormattedHeaderText(this.defaultText);
        }

        public void UpdateFormattedHeaderText(string text, bool appendToDefaultText = true)
        {
            // Guard against null format
            if (string.IsNullOrEmpty(headerTextFormat))
            {
                Debug.LogWarning($"UpdateFormattedHeaderText called before SetHeaderTextFormat on {gameObject.name}");
                // Fallback to simple text display
                SetHeaderText(text, appendToDefaultText);
                return;
            }
            
            string formattedText;
            if (appendToDefaultText && !string.IsNullOrEmpty(defaultText))
            {
                formattedText = string.Format(headerTextFormat, defaultText, text);
            }
            else
            {
                formattedText = string.Format(headerTextFormat, text);
            }

            headerTextView.text = formattedText;
        }
    
        public void SetHeaderText(string text, bool appendToDefaultText = true)
        {
            if (appendToDefaultText && !string.IsNullOrEmpty(defaultText))
            {
                headerTextView.text = $"{defaultText} {text}";
            }
            else
            {
                headerTextView.text = text;
            }
        }

        public void DisableScroll()
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = false;
        }
    }
}