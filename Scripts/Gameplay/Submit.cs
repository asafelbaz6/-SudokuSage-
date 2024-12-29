using System;
using DG.Tweening;
using Grid;
using Levels;
using Persistent_Singletons;
using SerializedData;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Logger = Logging.Logger;

namespace Gameplay
{
    public class Submit : MonoBehaviour
    {
        #region References
        [ShowInInspector] private LevelManager _levelManager;
        [ShowInInspector] private GridSystem _gridSystem;
        [Inject(Id = "SystemLogger")] [ShowInInspector] private Logger _systemLogger;
        #endregion

        #region UI References
        [SerializeField] private Button sumbitButton;
        [SerializeField] private RectTransform winScreenRectTransform;
        #endregion

        [Header("Data")]
        [ShowInInspector] private int[,] _solution;
        [ShowInInspector] private int[,] _playable;
        private bool _isCheckingSolution;

        public event Action<bool> OnLevelCompleteSolutionUIPopUpTweenBackToStart;

        #region Events - Right solution
        public event Action OnLevelCompleteRightSolution;
        public event Action OnLevelCompleteRightSolutionUIPopUp;
        #endregion

        #region Events - Wrong solution
        public event Action OnLevenCompleteWrongSolutionUIPopUp;
        public event Action OnLevelCompleteWrongSolution;
        #endregion
    
        [Inject]
        private void Construct(LevelManager levelManager, GridSystem gridSystem)
        {
            _levelManager = levelManager;
            _gridSystem = gridSystem;
        }
    
        private void Start() 
        {
            if (sumbitButton == null) {
                _systemLogger.Log("sumbitButton is null", this);
            }

            if (winScreenRectTransform == null) {
                _systemLogger.Log("winScreenRectTransform is null", this);
            }
            SetPlayable(_gridSystem.GetPlayableBoard());
        }

        #region Subscribing and Unsubscribing to events
        private void OnEnable()
        {
            sumbitButton.onClick.AddListener(() => SumbitButton_onClick()); 

            _levelManager.OnLevelChange += LevelManager_OnLevelChange;
            _levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
        }
        private void OnDisable() 
        {
            sumbitButton.onClick.RemoveListener(() => SumbitButton_onClick());

            _levelManager.OnLevelChange -= LevelManager_OnLevelChange;
            _levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
        }
        #endregion

        #region Stopping all tweens
        private void OnDestroy()
        {
            int killed = DOTween.Kill(gameObject);
            _systemLogger.Log($"Submit :: OnDestroy() {killed} ", this);
        }
        #endregion

        #region Getters and Setters
        public void SetPlayable(int[,] playableBoard) 
        {
            _playable = playableBoard;

            _systemLogger.Log("Submit :: SetPlayable()", this);
        }
        #endregion

        private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer sudokuDataContainer)
        {
            SetSolution(sudokuDataContainer);
            _systemLogger.Log("Submit :: OnReloadLevelDataAfterSolution", this);
        }

        private void LevelManager_OnLevelChange(SudokuDataContainer sudokuDataContainer)
        {
            SetSolution(sudokuDataContainer);
            _systemLogger.Log("Submit :: LevelManager_OnLevelChange", this);
        }

        private void SetSolution(SudokuDataContainer sudokuDataContainer)
        {
            _solution = sudokuDataContainer.GetSolutionBoard();
            _systemLogger.Log("Submit :: SetSolutionBoard with CurrentSudokuDataContainer", this);
        }

        private void SumbitButton_onClick()
        {
            CheckSolution();
        }

        private void CheckSolution()
        {
            if(_isCheckingSolution)
            {
                _systemLogger.Log("Already checking solution", this);
                return;
            }
        
            _isCheckingSolution = true;
        
            _systemLogger.Log("Checking solution", this);
            SetPlayable(_gridSystem.GetPlayableBoard());

            var gridSize = _playable.GetLength(0);
            for(var i = 0; i < gridSize; i++)
            {
                for(var j = 0; j < gridSize; j++)
                {
                    if (_playable[i, j] != _solution[i, j])
                    {
                        _systemLogger.Log("Wrong solution", this);
                        _systemLogger.Log($"Wrong solution: playable[{i}, {j}] = {_playable[i, j]}, solution[{i}, {j}] = {_solution[i, j]}", this);
                    
                        // pop up UI lose screen
                        OnLevenCompleteWrongSolutionUIPopUp?.Invoke();
                        return;
                    }
                }
            }

            OnLevelCompleteRightSolutionUIPopUp?.Invoke();

            // should save the level and stars stats here
            if(_levelManager.GetCurrentLevel() == GameManager.Instance.GetPlayableLevels() - 1) // if last level (one)
                GameManager.Instance.IncrementPlayableLevles();

            // save the level and stars stats after a win
            SavingSystem.Instance.SaveDataLevelsDictionary(GameManager.Instance.GetLevelDictionary());
        }


        // this is getting called from the restart button in the UI lose screen
        public void OnLevelCompleteWrongSolutionRestart()
        {
            _isCheckingSolution = false;

            var fader = FindObjectOfType<Fader>(); 
            if(fader != null)
            {
                fader.FadeOnSubmit(OnLevelCompleteWrongSolution, OnLevelCompleteSolutionUIPopUpTweenBack, false);
            }else{
                _systemLogger.Log("ERROR   Fader not found", this);
            }
        }

        // this is getting called from the NextLevelButton in the UI win screen
        public void OnLevelCompletedButton()
        {
            _isCheckingSolution = false;

            var fader = FindObjectOfType<Fader>(); 
            if(fader != null)
            {
                fader.FadeOnSubmit(OnLevelCompleteRightSolution, OnLevelCompleteSolutionUIPopUpTweenBack, true);
            }
            else{
                _systemLogger.Log("ERROR   Fader not found", this);
            }
        }  

        private void OnLevelCompleteSolutionUIPopUpTweenBack(bool rightSolution)
        {
            // tween back to the start position
            OnLevelCompleteSolutionUIPopUpTweenBackToStart?.Invoke(rightSolution);
        }
    }
}