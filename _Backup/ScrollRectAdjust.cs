using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectAdjust : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    private RectTransform contentRect;

    void Start()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponent<ScrollRect>();
        }
        
        contentRect = scrollRect.content;

        // Optionally set the initial scroll position if needed
        // scrollRect.verticalNormalizedPosition = 1; // Start at the top
    }

    private void OnDestroy()
    {
        // Unregister from the event
        // EventSystem.current.onSelectionChanged -= HandleSelectionChanged;
    }
    
    private void OnSelect(BaseEventData baseEventData)
    {
        Debug.Log("ScrollRect selected");
        RectTransform selectedRectTransform = baseEventData.selectedObject.GetComponent<RectTransform>();
        
        var width = scrollRect.GetComponent<RectTransform>().rect.width;
        var contentWidth = contentRect.rect.width;
        var overflow = (contentWidth - width) / 2f; 
        
        var leftBorder = overflow - selectedRectTransform.offsetMin.x;
        var rightBorder = -(overflow + (selectedRectTransform.offsetMax.x - contentWidth));

        if (leftBorder > contentRect.anchoredPosition.x)
            contentRect.anchoredPosition = new Vector2(leftBorder, contentRect.anchoredPosition.y);
        else if (rightBorder < contentRect.anchoredPosition.x)
            contentRect.anchoredPosition = new Vector2(rightBorder, contentRect.anchoredPosition.y);
    }
}