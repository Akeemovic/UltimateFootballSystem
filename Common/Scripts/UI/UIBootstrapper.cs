using UnityEngine;

namespace UltimateFootballSystem.Common.Scripts.UI
{
    public class UIBootstrapper : MonoBehaviour
    {
        [SerializeField] private GameObject RootObject;
    
        // Start is called before the first frame update
        void Start()
        {
            using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
            {
                RootObject.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
