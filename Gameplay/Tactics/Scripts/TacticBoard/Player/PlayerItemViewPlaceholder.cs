using TMPro;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Core.TacticsEngine.Utils;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class PlayerItemViewPlaceholder : MonoBehaviour
    {
        [SerializeField]
        private PlayerItemView playerItemView;

        [SerializeField]
        private TextMeshProUGUI positionView;

        public void Show()
        {
            gameObject.SetActive(true);
            SetPositionText();
        }

        public void ToggleShow(bool show)
        {
            gameObject.SetActive(show);
            if (show)
            {
                SetPositionText();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdatePositionText()
        {
            SetPositionText();
        }

        private void Awake()
        {
            if (!playerItemView)
            {
                playerItemView = GetComponentInParent<PlayerItemView>();
            }
        }

         private void SetPositionText()
        {
            if (!playerItemView)
            {
                positionView.text = string.Empty;
                return;
            }
        
            if (playerItemView.ViewOwnerOption == PlayerItemViewOwnerOption.BenchList)
            {
                positionView.text = "S" + (playerItemView.BenchPlayersListIndex + 1);
            }
            else
            {
                positionView.text = TacticalPositionUtils.GetTypeForPosition(playerItemView.ParentPositionZoneView.tacticalPositionOption).ToString() ?? string.Empty;
            }
        }
        
        // private void SetPositionText()
        // {
        //     if (!playerItemView)
        //     {
        //         positionView.text = string.Empty;
        //         return;
        //     }
        //
        //     if (playerItemView.ViewOwnerOption == PlayerItemViewOwnerOption.BenchList)
        //     {
        //         positionView.text = "S" + (playerItemView.BenchPlayersListIndex + 1);
        //     }
        //
        //     else
        //     {
        //         switch (playerItemView.ParentPositionZoneView?.tacticalPositionOption)
        //         {
        //             case TacticalPositionOption.DCL:
        //             case TacticalPositionOption.DCR:
        //                 positionView.text = "DC";
        //                 break;
        //             case TacticalPositionOption.DMCL:
        //             case TacticalPositionOption.DMCR:
        //                 positionView.text = "DMC";
        //                 break;
        //             case TacticalPositionOption.DML:
        //                 positionView.text = "WBL";
        //                 break;
        //             case TacticalPositionOption.DMR:
        //                 positionView.text = "WBR";
        //                 break;
        //             case TacticalPositionOption.MCL:
        //             case TacticalPositionOption.MCR:
        //                 positionView.text = "MC";
        //                 break;
        //             case TacticalPositionOption.AMCL:
        //             case TacticalPositionOption.AMCR:
        //                 positionView.text = "AMC";
        //                 break;
        //             case TacticalPositionOption.STCL:
        //             case TacticalPositionOption.STC:
        //             case TacticalPositionOption.STCR:
        //                 positionView.text = "ST";
        //                 break;
        //             default:
        //                 positionView.text = playerItemView.ParentPositionZoneView?.tacticalPositionOption.ToString() ?? string.Empty;
        //                 break;
        //         }
        //     }
        // }
    }
}
