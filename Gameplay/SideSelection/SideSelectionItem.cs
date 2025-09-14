using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay
{
    [RequireComponent(typeof(Image))]
    public class SideSelectionItem : MonoBehaviour
    {
        [Header("Item Configuration")]
        [SerializeField] private SideSelectionItemSide itemSide;
        [SerializeField] private Image image;

        [Header("Visual States")]
        [SerializeField] private Color normalColor = Color.gray;
        [SerializeField] private Color selectedColor = Color.white;

        public SideSelectionItemSide ItemSide => itemSide;
        public bool IsSelected { get; private set; }
        public RectTransform RectTransform => transform as RectTransform;

        void Start()
        {
            if (image == null)
                image = GetComponent<Image>();

            SetSelected(false);
            Debug.Log($"[SideSelectionItem] Initialized item for side: {itemSide}");
        }

        public void SetSelected(bool selected)
        {
            IsSelected = selected;
            if (image != null)
            {
                image.color = selected ? selectedColor : normalColor;
            }
            Debug.Log($"[SideSelectionItem] {itemSide} item selected: {selected}");
        }

        public void SetItemSide(SideSelectionItemSide side)
        {
            itemSide = side;
            Debug.Log($"[SideSelectionItem] Item side set to: {side}");
        }
    }
}