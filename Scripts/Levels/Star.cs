using UnityEngine;
using UnityEngine.UI;

namespace Levels
{
    public class Star : MonoBehaviour
    {
        [SerializeField] private Sprite availableStar, unavailableStar;

        private Image _starImage;

        private void Awake() 
        {
            _starImage = GetComponent<Image>();    
        }

        public void ChangeSprite(bool available)
        {
            _starImage.sprite = available ? availableStar : unavailableStar;
        }
    }
}
