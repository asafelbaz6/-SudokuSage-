using System.Runtime.InteropServices;
using DG.Tweening;
using Gameplay;
using Persistent_Singletons;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Logger = Logging.Logger;

namespace Levels
{
    public class LevelCompleted : MonoBehaviour
    {
        #region References
        [Header("References")]
        [ShowInInspector] private Submit _submitButton;
        [ShowInInspector] private Hint _hint;
        [Header("Debug")]
        [Inject(Id = "LevelsLogger")] [ShowInInspector] private Logger _levelsLogger;
        [Header("UI")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Image[] starsChildren;
        #endregion

        private const float Duration = 1f;
        private int _hintsRemaining;
        [SerializeField] private bool isLevelCompleted = false;

        [Inject]
        private void Construct(Submit submitButton, Hint hint)
        {
            _submitButton = submitButton;
            _hint = hint;
        }
        
        #region Subscribing and Unsubscribing to submit.OnLevelCompleteRightSolution
        private void OnEnable() 
        {
            _submitButton.OnLevelCompleteRightSolutionUIPopUp += SubmitButton_OnLevelCompleteRightSolutionUIPopUp;  
            _submitButton.OnLevenCompleteWrongSolutionUIPopUp += SubmitButton_OnLevenCompleteWrongSolutionUIPopUp;  
            _submitButton.OnLevelCompleteSolutionUIPopUpTweenBackToStart += SubmitButton_OnLevelCompleteSolutionUIPopUpTweenBackToStart;
        }

        private void OnDisable()
        {
            _submitButton.OnLevelCompleteRightSolutionUIPopUp -= SubmitButton_OnLevelCompleteRightSolutionUIPopUp;
            _submitButton.OnLevenCompleteWrongSolutionUIPopUp -= SubmitButton_OnLevenCompleteWrongSolutionUIPopUp;  
            _submitButton.OnLevelCompleteSolutionUIPopUpTweenBackToStart -= SubmitButton_OnLevelCompleteSolutionUIPopUpTweenBackToStart;
        }
        #endregion

        #region Stopping all tweens
        private void OnDestroy()
        {
            // Stops all tweens associated with this GameObject
            DOTween.Kill(gameObject);
            DOTween.Kill(rectTransform);
        }
        #endregion

        private void UpdateStarsWithHintsValues()
        {
            _hintsRemaining = _hint.GetHintCounter();

            // update stars
            for (var i = 0; i < starsChildren.Length; i++)
            {
                if (!starsChildren[i].TryGetComponent<Star>(out var star)) continue;

                star.ChangeSprite(i < _hintsRemaining);
            }
        
            var currentLevel = GameManager.Instance.GetLevelToLoad();
            // this equals 0 if the level isn't solved yet
            var previousStarsIfLevelAlreadySolved = GameManager.Instance.GetStarsInSpecificLevel(currentLevel);

            // save the highest score of the level
            if(_hintsRemaining > previousStarsIfLevelAlreadySolved)
            {
                _levelsLogger.Log($"BEFORE    Total stars collected: {GameManager.Instance.GetTotalStarsCollected()}", this);
                var newScore = _hintsRemaining - previousStarsIfLevelAlreadySolved;
                GameManager.Instance.IncrementTotalStarsCollected(newScore);
 
                // set new record
                _levelsLogger.Log($"AFTER    Total stars collected: {GameManager.Instance.GetTotalStarsCollected()}", this);
            }

            GameManager.Instance.SetStarsInSpecificLevel(currentLevel, _hintsRemaining);
        }

        /// <summary>
        ///  Since there are 2 UI popups, we need to tween them back to their start positions
        ///  On the Win UI popup, isLevelCompleted is true
        ///  On the Lose UI popup, isLevelCompleted is false
        /// </summary>
        private void SubmitButton_OnLevelCompleteRightSolutionUIPopUp()
        {
            if(isLevelCompleted == false) return; // only the win screen is getting called after this line of code

            var centerXPos = 0f; 
            rectTransform.DOLocalMoveX(centerXPos, Duration).SetEase(Ease.OutBounce);

            UpdateStarsWithHintsValues();
        }
    
        private void SubmitButton_OnLevelCompleteSolutionUIPopUpTweenBackToStart(bool rightSolution)
        {
            var outOfScreenXPos = -1000f;
            if(rightSolution == true && isLevelCompleted == true) // right solution
            {
                rectTransform.DOLocalMoveX(outOfScreenXPos, Duration).SetEase(Ease.OutBack);
            }
            else if(rightSolution == false && isLevelCompleted == false) // wrong solution
            {
                rectTransform.DOLocalMoveX(-outOfScreenXPos, Duration).SetEase(Ease.OutBack);
            }
        }
    
        private void SubmitButton_OnLevenCompleteWrongSolutionUIPopUp()
        {
            if(isLevelCompleted == true) return;  // only the lose screen is getting called after this line of code,
            // the isLevelCompleted is assigned in the inspector

            var centerXPos = 0f;
            rectTransform.DOLocalMoveX(centerXPos, Duration).SetEase(Ease.OutBounce);
        }

    }
}
