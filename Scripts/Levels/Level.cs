using Persistent_Singletons;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Logger = Logging.Logger;

namespace Levels
{
    public class Level : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<Level>
        {
        
        }
        
        private int levelNumber;
        [SerializeField] private TextMeshProUGUI numberText;
        [Inject(Id = "LevelsLogger")] [ShowInInspector] private Logger _levelsLogger;

        [Header("Background Colors")]
        [SerializeField] private Sprite availableLevel;
        [SerializeField] private Sprite unavailableLevel;
        [SerializeField] private Image currentLevel;

        [Header("Stars")]
        [SerializeField] private Sprite[] starsSprites;
        [SerializeField] private Image currentStar;
        
        #region Getters and Setters
        public int GetLevelNumberText() => levelNumber;
    
        #endregion

        public void PrintNumber()
        {
            _levelsLogger.Log("Pressed! Go To level: " + levelNumber, this);
            GameManager.Instance.SetLevelToLoad(levelNumber);
        }

        public void SetLevelNumberText(int number)
        {
            levelNumber = number;
            numberText.text = number == 0 ? "" : number.ToString();
        }

        private void HideNumber() => numberText.text = "";
        private void ShowStars(bool show) => currentStar.gameObject.SetActive(show);
    
        public void SetAvailable(bool available)
        {
            if(available)
            {
                SetLevelNumberText(levelNumber);
            
            }
            else
            {
                HideNumber();
            }

            currentLevel.sprite = available ? availableLevel : unavailableLevel;
            ShowStars(available);

            ChangeStars(GameManager.Instance.GetStarsInSpecificLevel(levelNumber));
        }

        [Button]
        public void ChangeStars(int numStars)
        {
            if(numStars < 0 || numStars > starsSprites.Length)
            {
                _levelsLogger.Log("Invalid number of stars: " + numStars, this);
                return;
            }

            currentStar.sprite = starsSprites[numStars];
        }
    }
}
