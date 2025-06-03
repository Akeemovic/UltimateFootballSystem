using UnityEngine;
using UnityEngine.UI;

namespace UltimateFootballSystem.Gameplay.Common.Widgets.StarRating
{
    public class StarRatingView : MonoBehaviour 
    {
        [SerializeField]
        public float rating;
    
        public GameObject[] stars;

        void Awake()
        {
            for(int i = 0; i < stars.Length; i++)
            {
                SetColorAlpha(stars[i], 0f);
            }
        
            SetRating(rating);
            // SetRating(1.7f);
            // SetRating(4);
        }

        public void SetRating(float rating)
        {
            int numStars = GetNumberOfStars(rating);

            for(int i = 0; i < numStars; i++)
            {
                SetColorAlpha(stars[i], 1f);
            }

            for(int i = numStars; i < stars.Length; i++)
            {
                SetColorAlpha(stars[i], 0f);
            }
        }

        int GetNumberOfStars(float rating)
        {
            return rating switch
            {
                <= 0 => 0,
                <= 0.5f => 1,
                <= 1f => 2,
                <= 1.5f => 3,
                <= 2f => 4,
                <= 2.5f => 5,
                <= 3f => 6,
                <= 3.5f => 7,
                <= 4f => 8,
                <= 4.5f => 9,
                <= 5f => 10,
                _ => 0
            };
        }

        void SetColorAlpha(GameObject obj, float alpha)
        {
            Image img = obj.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }
    }
}
