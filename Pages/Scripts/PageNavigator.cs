using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateFootballSystem.Pages.Scripts
{
    public class PageNavigator : MonoBehaviour
    {
        [SerializeField] private List<Page> Pages;
    
        // Start is called before the first frame update
        void Start()
        {
            navigateToPage(null, true);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void navigateToPage(string route = null, bool defaultRoute = false)
        {
            if (defaultRoute)
            {
                Pages[0].gameObject.SetActive(true); return;
            }

            foreach (var page in Pages)
            {
                if (page.route.Equals(route, StringComparison.OrdinalIgnoreCase))
                {
                    page.gameObject.SetActive(true);
                }
                else
                {
                    page.gameObject.SetActive(false);
                }
            }
        }
    }
}
