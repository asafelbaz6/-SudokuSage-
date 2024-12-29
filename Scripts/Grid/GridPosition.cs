using DG.Tweening;
using Gameplay;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Logger = Logging.Logger;

namespace Grid
{
    public class GridPosition : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private TextMeshProUGUI numberText;
        [SerializeField] private Color32 pressedColor;
        [SerializeField] private Color32 unpressedColor;
        [SerializeField] private Color32 hintColor;
        private bool _isPressed;
        
        [Header("References")]
        [ShowInInspector] private Player _player;
        [ShowInInspector] private GridSystem _gridSystem;
    
        [Header("Debug")]
        [Inject(Id = "MapLogger")] [ShowInInspector] private Logger _mapLogger;
    
        private int Row {get; set;}
        private int Col {get; set;}
        [Header("Data")]
        private int _num;
        private bool _isTweening;
        
        [Inject]
        private void Construct(Player player, GridSystem gridSystem)
        { 
            _player = player;
            _gridSystem = gridSystem;
        }
        private void Start() 
        {
            if (_player == null) {
                Debug.Log("GridPosition: No player assigned!", transform);
            }
            if (_gridSystem == null) {
                Debug.Log("GridPosition: No grid system assigned!", transform);
            }
            if (_mapLogger == null) {
                Debug.Log("GridPosition: No map logger assigned!", transform);
            }
            
            backgroundImage.color = unpressedColor;
        }

        #region Getters and Setters
        public Logger GetLogger() => _mapLogger;
        public Player GetPlayer() => _player;
        public GridSystem GetGridSystem() => _gridSystem;
        public int GetNum() => _num; 
        public bool IsEmpty() => _num == 0;
        public Point.Point GetPoint() => new(Row, Col);
        public void SetRowAndCol(int row, int col)
        {
            Row = row;
            Col = col;
        }
        #endregion

        #region Cancel Tween
        void OnDestroy()
        {
            CancelTween();
        }

        public void CancelTween()
        {
            if (backgroundImage != null && _isTweening)
            {
                backgroundImage.DOKill(); // Cancels any active tween on backgroundImage

                backgroundImage.color = unpressedColor;
                _mapLogger.Log("Canceled tween", this);
            }
        }
        #endregion

        public void SetNumText(int number)
        {
            _num = number;
            if(number == 0)
            {
                numberText.text = "";
            }
            else
            {
                numberText.text = number.ToString();
            }
        }
    
        public override string ToString()
        {
            string positionData = "Row: " + Row + ", Col: " + Col + ", Num: " + _num;
            _mapLogger.Log(positionData, this);   
            return positionData;
        }

        // this is getting called upon pressing on the grid position
        public void ChangeColorOnPress()
        {
            _gridSystem.SetCurrentPressedCube(this);

            _player.PlaceNumberOnBoard();

            ChangeColor();
            ToString();       
        }

        public void ChangeColor()
        {
            _isPressed = !_isPressed;
            _mapLogger.Log($"isPressed: {_isPressed}", this);
            if(_isPressed)
            {
                backgroundImage.color = pressedColor;
            }
            else
            {
                ResetColor();
            }
        }

        public void ResetColor()
        {
            _isPressed = false;
            backgroundImage.color = unpressedColor;
        }

        public void LerpColorOnHintEffect()
        {
            if(backgroundImage != null && !_isTweening)
            {
                _isTweening = true;

                float duration = 0.75f; // duration for each color change
                Color32 startColor = backgroundImage.color; // the original color of the background

                // Animate the background color to targetColor and back to startColor three times
                backgroundImage.DOColor(hintColor, duration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(6, LoopType.Yoyo) // 6 loops (3 cycles back and forth)
                    .OnComplete(() => ResetColorOnTweenComplete(startColor)); // Reset to original color

                _mapLogger.Log("Lerping color", this);
            }
        }

        private void ResetColorOnTweenComplete(Color32 startColor)
        {
            if (backgroundImage != null)
                backgroundImage.color = startColor;
        }

    }
}
