using System;
using DG.Tweening;
using Levels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class Fader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup fadeImageCanvasGroup;
        private const float TweenDuration = 0.4f;
        private bool _canTween = true;
        public event Action<LevelChange> OnFadeCompleteLevelChange;

        private void Start() 
        {
            fadeImageCanvasGroup.alpha = 1;
            FadeInAfterMapTween(TweenDuration);
        }

        #region Destroy Tweens OnDestroy
        private void OnDestroy()
        {
            DOTween.Kill(fadeImageCanvasGroup);
        }
        #endregion

        #region Getters and Setters
        public bool GetCanTween() => _canTween;
        #endregion

        // callback1 = OnLevelCompleteRightSolution / OnLevelCompleteWrongSolution
        // callback2 = OnLevelCompleteSolutionUIPopUpTweenBack
        // rightSolution = true / false
        public void FadeOnSubmit(Action callback1, Action<bool> callback2, bool rightSolution)
        {
            if(!_canTween) return;

            _canTween = false;
        
            fadeImageCanvasGroup.DOFade(1, TweenDuration)
                .OnComplete(() =>
                {
                    callback1?.Invoke();

                    callback2?.Invoke(rightSolution);
                 
                    FadeInAfterMapTween(TweenDuration);
                });
        }

        public void FadeOutLevelMap(Action<LevelChange> callback, LevelChange levelChange)
        {
            if(!_canTween) return;

            _canTween = false;
        
            fadeImageCanvasGroup.DOFade(1, TweenDuration)
                .OnComplete(() =>
                {
                    callback?.Invoke(levelChange);
                    OnFadeCompleteLevelChange?.Invoke(levelChange);
                    FadeInAfterMapTween(TweenDuration);
                });
        }

        public void FadeOutBetweenLevels(LevelChange levelChange)
        {
            if(!_canTween) return;

            int levelToLoad = levelChange == LevelChange.Next
                ? SceneManager.GetActiveScene().buildIndex + 1 
                : SceneManager.GetActiveScene().buildIndex - 1;

            // Invalid level
            if(levelToLoad < 0 || levelToLoad >= SceneManager.sceneCountInBuildSettings) return;

            _canTween = false;

            fadeImageCanvasGroup.DOFade(1, TweenDuration)
                .OnComplete(() =>
                {
                    SceneManager.LoadSceneAsync(sceneBuildIndex: levelToLoad);
                });
        }

        private void FadeInAfterMapTween(float tweenDuration)
        {
            // fade in after switching the level
            fadeImageCanvasGroup.blocksRaycasts = true;
            fadeImageCanvasGroup.DOFade(0, tweenDuration)
                .OnComplete(() => UnblockRayCastsOnFadeComplete());
        }

        private void UnblockRayCastsOnFadeComplete()
        {
            fadeImageCanvasGroup.blocksRaycasts = false;
            _canTween = true;
            Debug.Log("FADER:: canTween: " + _canTween);  
            Debug.Log("FADER:: blocksRaycasts: " + fadeImageCanvasGroup.blocksRaycasts); 
        }
    }
}