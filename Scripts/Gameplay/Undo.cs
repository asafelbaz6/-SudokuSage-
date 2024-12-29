using System;
using System.Collections.Generic;
using Grid;
using Levels;
using SerializedData;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace Gameplay
{
    public class Undo : MonoBehaviour
    {
        #region References
        [ShowInInspector] private Player _player;
        [ShowInInspector] private GridSystem _gridSystem;
        [ShowInInspector] private LevelManager _levelManager;

        [Header("Debug")]
        [Inject(Id = "SystemLogger")] [ShowInInspector] private Logger _systemLogger;
        #endregion
        [ShowInInspector] private readonly Stack<Point.Point> _undoStack = new();
    
        public event Action<Point.Point,int> OnUndoAddPositionToHintDictionary;
    
        [Inject]
        private void Construct(Player player, GridSystem gridSystem, LevelManager levelManager)
        {
            _player = player;
            _gridSystem = gridSystem;
            _levelManager = levelManager;
        }

        #region Subscribing / Unsubscribing to events
        private void Start() 
        {
            _player.OnNumberPlacedOnBoardStack += Player_OnNumberPlacedOnBoardStack;
            _levelManager.OnLevelChange += LevelManager_OnLevelChange;
            _levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
        }

        private void OnDisable() 
        {
            _player.OnNumberPlacedOnBoardStack -= Player_OnNumberPlacedOnBoardStack;
            _levelManager.OnLevelChange -= LevelManager_OnLevelChange;
            _levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
        }
        #endregion
    
        private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
        {
            _systemLogger.Log("OnReloadLevelDataAfterSolution: RemoveAllPointsFromUndoStack", this);
            RemoveAllPointsFromUndoStack();
        }

        private void LevelManager_OnLevelChange(SudokuDataContainer sudokuDataContainer)
        {
            _systemLogger.Log("OnLevelChange: RemoveAllPointsFromUndoStack", this);
            RemoveAllPointsFromUndoStack();
        }

        private void RemoveAllPointsFromUndoStack()
        {
            _undoStack.Clear();
        }


        // this is getting called from the UNDO button
        public void UndoAction()
        {
            _systemLogger.Log("Undo", this);
            if(_undoStack.Count == 0)
            {
                _systemLogger.Log("Nothing to undo", this);
                return;
            }

            Point.Point point = _undoStack.Pop();
            _systemLogger.Log("Undo: " + point.ToString(), this);

            _gridSystem.ResetCubeOnUndo(point);

            // undo = return the last number placed on the board to the hint dictionary
            int number = _levelManager.GetCellValueFromSolutionBoard(point);
            OnUndoAddPositionToHintDictionary?.Invoke(point, number);
        }

        private void Player_OnNumberPlacedOnBoardStack(Point.Point point)
        {
            AddPointToUndoStack(point);
        }

        public void AddPointToUndoStack(Point.Point point)
        {
            _systemLogger.Log("AddPointToUndoStack: " + point.ToString(), this);
            _undoStack.Push(point);
        }

    }
}
