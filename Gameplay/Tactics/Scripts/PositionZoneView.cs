using UltimateFootballSystem.Core.Tactics;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PositionZoneView : MonoBehaviour
    {
    
        [SerializeField]
        public TacticalPositionOption tacticalPositionOption;
    
        [SerializeField]
        public PlayerItemView childPlayerItemView;
    
        public bool InUseForFormation { get; private set; }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    
        public void ToggleShow(bool show)
        {
            gameObject.SetActive(show);
        }
    
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetInUseForFormation(bool inUseForFormation,  bool isInitFormation = false, bool isCalledFromChildView = false)
        {
            InUseForFormation = inUseForFormation;

            if (!isCalledFromChildView)
            {
                childPlayerItemView.SetInUseForFormation(inUseForFormation, isCalledFromParentView: true);
            }
        }
    
        private void Awake()
        {  
            // TacticalPosition = GetComponent<TacticalPosition>();
            childPlayerItemView.ParentPositionZoneView = this;
        }
    
        // Start is called before the first frame update
        private void Start()
        {
            // Debug.Log("player vie name in zoneview: " + childPlayerItemView.gameObject.name);
        }

        // Update is called once per frame
        private void Update()
        {
        
        }
    }
}
