using System;
using System.Collections.Generic;
using Gameplay;
using Persistent_Singletons;
using SerializedData;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace Levels
{
    public class LevelManager : MonoBehaviour
    {
        #region  References
        [Header("References")]
        [ShowInInspector] private Submit _submitButton;
        [Header("Debug")]
        [Inject(Id = "SystemLogger")] [ShowInInspector] private Logger _systemLogger;
        #endregion

        [Header("Properties")]
        [ShowInInspector] private int _currentLevel;
        public event Action<Dictionary<Point.Point, int>> OnChangeHintDictionary;
        public event Action<SudokuDataContainer> OnLevelChange;
        [ShowInInspector] private List<SudokuDataContainer> _sudokuDataContainersList;
        public event Action <SudokuDataContainer>OnReloadLevelDataAfterSolution;


        private void Awake()
        {
            // Game manager will always be in the scene before the level manager, so this will always work
            _sudokuDataContainersList = GameManager.Instance.GetAllSudokuDataContainers();
        }

        [Inject]
        private void Construct(Submit submitButton)
        {
            _submitButton = submitButton;
        }
        
        private void Start() 
        {
            LoadLevel(GameManager.Instance.GetLevelToLoad());
        }

        #region Subscribing and Unsubscribing to events
        private void OnEnable() 
        {
            _submitButton.OnLevelCompleteWrongSolution += SubmitButton_OnLevelCompleteWrongSolution;
            _submitButton.OnLevelCompleteRightSolution += SubmitButton_OnLevelCompleteRightSolution;
        }
    
        private void OnDisable() 
        {
            _submitButton.OnLevelCompleteWrongSolution -= SubmitButton_OnLevelCompleteWrongSolution;
            _submitButton.OnLevelCompleteRightSolution -= SubmitButton_OnLevelCompleteRightSolution;
        }
        #endregion

        #region  Getters and Setters
        public int GetCurrentLevel() => _currentLevel;
        public Dictionary<Point.Point, int> GetCurrentLevelDeletedPositionsDictionary() => _sudokuDataContainersList[_currentLevel].GetDeletedPositions();
        public List<SudokuDataContainer> GetAllSudokuDataContainers() => new(_sudokuDataContainersList);
        public int GetCellValueFromSolutionBoard(Point.Point point) => _sudokuDataContainersList[_currentLevel].GetSolutionBoard()[point.X, point.Y];

        #endregion

        // load the current level data again
        private void SubmitButton_OnLevelCompleteWrongSolution()
        {
            _systemLogger.Log($"OnLevelCompleteWrongSolution: {_sudokuDataContainersList[_currentLevel]}, currentLevel: {_currentLevel}", this);
            OnReloadLevelDataAfterSolution?.Invoke(_sudokuDataContainersList[_currentLevel]);
        }

        // load the next level data
        private void SubmitButton_OnLevelCompleteRightSolution()
        {
            var nextLevel = _currentLevel + 1;
            _systemLogger.Log($"OnLevelCompleteRightSolution: {_sudokuDataContainersList[nextLevel]}, nextLevel: {nextLevel}", this);
            OnReloadLevelDataAfterSolution?.Invoke(_sudokuDataContainersList[nextLevel]);
 
            _currentLevel++;
        }

        private void LoadLevel(int level)
        {
            // sudokuDataContainersList.Clear();   
            // sudokuDataContainersList = SavingSystem.Instance.LoadLevelsData();

            if(level < 1)
            {
                _systemLogger.Log("INVALID LEVEL: " + level, this);
                return;
            }

            if(level > _sudokuDataContainersList.Count)
            {
                _systemLogger.Log("Creating a new level: " + level, this);
                return;
            }
        
            _currentLevel = level - 1;

            var hintMap = _sudokuDataContainersList[_currentLevel].GetDeletedPositions();
            OnChangeHintDictionary?.Invoke(hintMap);

            OnLevelChange?.Invoke(_sudokuDataContainersList[_currentLevel]);
        }


        // this function is used to add a new level to the list, which will be saved to the json file, 
        // but I created 200 levels throughout the Inspector, and this function is no longer needed
        /*
        // public void AddLevelToSudokuDataContainers(SudokuDataContainer sudokuDataContainer)
        // {
        //     sudokuDataContainersList.Add(sudokuDataContainer);

        //     SavingSystem.Instance.SaveDataLevels(sudokuDataContainersList);
        //     SavingSystem.Instance.SaveDataLevelsDictionary(GameManager.Instance.GetLevelDictionary());
        // }
        */
    }
}