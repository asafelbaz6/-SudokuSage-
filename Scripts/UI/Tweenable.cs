using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Tweenable : MonoBehaviour
    {
        [Header("RectTransform")]
        [SerializeField] private float width, height; //, rotationAngle, rotatioDuration;
        [SerializeField] private RectTransform rectTransform;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI numberText;
        private const float FaderTweenDuration = 0.4f;
        private const float TweenDuration = FaderTweenDuration * 2;

        [Header("Stars")]
        [SerializeField] private RectTransform starsRectTransform;
        [SerializeField] private float starsWidth, starsHeight;
    
        private void Start()
        {
            ChangeSizeAndSpin();
        }

        #region Destroy Tweens
        private void OnDestroy()
        {
            DOTween.Kill(rectTransform);
            DOTween.Kill(numberText);
            DOTween.Kill(starsRectTransform);
        }
        #endregion

        private void ChangeSizeAndSpin()
        {
            ResetSizeBeforeTween()
                .OnComplete(() =>
                {
                    TweenRectTransform();
                    TweenText();
                    TweenStars();
                });
        }

        private Tween ResetSizeBeforeTween()
        {
            // destroy previous tweens before creating new ones
            if(rectTransform != null)
            {
                rectTransform.sizeDelta = Vector2.zero;
                DOTween.Kill(rectTransform);
            }   

            if(numberText != null)
            {
                numberText.fontSize = 0f;
                DOTween.Kill(numberText);
            }

            if(starsRectTransform != null)
            {
                starsRectTransform.sizeDelta = Vector2.zero;
                DOTween.Kill(starsRectTransform);
            }       
        
            // Create a placeholder tween with a very short duration to act as a delay
            return DOVirtual.DelayedCall(0.01f, () => { });
        }
    
        private void TweenRectTransform()
        {
            if(rectTransform == null) return;

            Vector2 newSize = new(width, height);
            rectTransform.DOSizeDelta(newSize, TweenDuration);
        }

        private void TweenText()
        {
            if(numberText == null) return;

            var endFontSize = 55f; 
            DOTween.To(() => numberText.fontSize, x => numberText.fontSize = x, endFontSize, TweenDuration);
        }

        private void TweenStars()
        {
            if(starsRectTransform == null) return;

            Vector2 starsSize = new(starsWidth, starsHeight);
            starsRectTransform.DOSizeDelta(starsSize, TweenDuration);
        }
    }
}
