using NinjaTools.FlexBuilder;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PositionZonesContainerView : MonoBehaviour
    {
        // [SerializeField]
        // public TacticalPositionOption TacticalPositionOption;
    
        [SerializeField]
        public  PositionZoneView[] ZoneViews = new PositionZoneView[5];
    
        [SerializeField]
        public  PlayerItemView[] PlayerItemViews = new PlayerItemView[5];
        // public  PlayerItemView[] PlayerItemViews = new PlayerItemView[30];

        public FlexContainer flexContainer;
    

        private void Awake()
        {
            flexContainer = GetComponent<FlexContainer>();
        
            int zoneItemViewIndex = 0;
            int playerItemViewIndex = 0;
            foreach (var zone in ZoneViews)
            {
                if (zone == null) continue;

                ZoneViews[zoneItemViewIndex] = zone;
                zoneItemViewIndex++;
                PlayerItemViews[playerItemViewIndex] = zone.childPlayerItemView;
                playerItemViewIndex++;
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        
            // foreach (PositionZoneView tacticsPitch in ZoneViews)
            // {
            //     // Debug.Log(tacticsPitch.gameObject.name);
            //     // Debug.Log(tacticsPitch.childPlayerItemView);
            //     
            //     // Debug.Log(tacticsPitch.childPlayerItemView.gameObject.name);
            // }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
