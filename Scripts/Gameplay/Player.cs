using System;
using Grid;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace Gameplay
{
    public class Player : MonoBehaviour
    {
        [ShowInInspector] private int _numberToPlaceOnBoard = 0;
        public event Action<int> OnNumberPlacedOnBoard;
        public event Action<Point.Point> OnNumberPlacedOnBoardStack;
        public event Action<Point.Point> OnNumberPlacedOnBoardDeletedPosition;
    
        [Header("Dependencies")]
        [Inject(Id = "PlayerLogger")] [ShowInInspector] private Logger _playerLogger;
        [Inject] [ShowInInspector] private GridSystem _gridSystem;
        
        private void ResetNumber() => _numberToPlaceOnBoard = 0;
        public void SetNumberToPlaceOnBoard(int number) 
        {
            _numberToPlaceOnBoard = number;
            _playerLogger.Log("SetNumberToPlaceOnBoard(): " + _numberToPlaceOnBoard, this);
        }
        public void PlaceNumberOnBoard()
        {
            var currentPressedCube = _gridSystem.GetCurrentPressedCube();
            var currentPressedCubePoint = currentPressedCube.GetPoint();
            if(_numberToPlaceOnBoard != 0 && currentPressedCube.IsEmpty())
            {
                OnNumberPlacedOnBoard?.Invoke(_numberToPlaceOnBoard);
                OnNumberPlacedOnBoardStack?.Invoke(currentPressedCubePoint);
                OnNumberPlacedOnBoardDeletedPosition?.Invoke(currentPressedCubePoint);

                ResetNumber();
            }
        }
    }
}
