using Persistent_Singletons;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TotalStarsDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI totalStarsText;

        private void Start()
        {
            // this is okay because the game manager will always be initialized in previous scenes
            var totalStarsCollected = GameManager.Instance.GetTotalStarsCollected();
            const int numberOfLevels = 200;
            const int maxStars = numberOfLevels * 3; // 3 stars per level
            totalStarsText.text = $"{totalStarsCollected}/{maxStars}";
        }
    }
}
