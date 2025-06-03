using NinjaTools.FlexBuilder;
using UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard.Player;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics.Scripts.TacticBoard
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

        
            // foreach (PositionZoneView view in ZoneViews)
            // {
            //     // Debug.Log(view.gameObject.name);
            //     // Debug.Log(view.childPlayerItemView);
            //     
            //     // Debug.Log(view.childPlayerItemView.gameObject.name);
            // }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
