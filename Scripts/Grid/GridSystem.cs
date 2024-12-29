using Gameplay;
using Levels;
using SerializedData;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace Grid
{
    public class GridSystem : MonoBehaviour
    {
        #region References
        private Player _player;
        private MapGenerator _mapGenerator;
        private LevelManager _levelManager;
    
        [Header("Debug")]
        [Inject(Id = "SystemLogger")] [ShowInInspector] private Logger _systemLogger;
        #endregion

        [ShowInInspector] private GridPosition _currentPressedCube = null;
        [ShowInInspector] private GridPosition _lastPressedCube = null;
        private const int GridSize = 9;
        private GridPosition[,] _sudokuBoardGridPositions = new GridPosition[GridSize, GridSize];
        [ShowInInspector] private int[,] _playable;
    
        [Inject]
        private void Construct(Player player, MapGenerator mapGenerator, LevelManager levelManager)
        {
            _player = player;
            _mapGenerator = mapGenerator;
            _levelManager = levelManager;
        }
        private void Start() 
        {
            _sudokuBoardGridPositions = _mapGenerator.GetSudokuBoardGridPositions(); 
        }

        #region Subscribing and Unsubscribing to events
        private void OnEnable()
        {
            _player.OnNumberPlacedOnBoard += Player_OnNumberPlacedOnBoard;
            _levelManager.OnLevelChange += LevelManager_OnLevelChange;
            _levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
        }

        private void OnDisable()
        {
            _player.OnNumberPlacedOnBoard -= Player_OnNumberPlacedOnBoard;
            _levelManager.OnLevelChange -= LevelManager_OnLevelChange;
            _levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
        }
        #endregion

        #region Getters and Setters
        public int[,] GetPlayableBoard() => _playable;
        public GridPosition GetCurrentPressedCube() => _currentPressedCube != null ?  _currentPressedCube : null;
        public GridPosition GetLastPressedCube() => _lastPressedCube != null ?  _lastPressedCube : null;

        private void SetPlayableBoard(SudokuDataContainer sudokuDataContainer)
        {
            _playable = sudokuDataContainer.GetPlayableBoard();
            _systemLogger.Log("GridSystem :: SetPlayableBoard with CurrentSudokuDataContainer", this);
        }
    
        public void SetCurrentPressedCube(GridPosition pressedCube)
        {
            if(_currentPressedCube != null)
            {
                if(_lastPressedCube != null)
                    _lastPressedCube.ResetColor();


                _lastPressedCube = _currentPressedCube;
                _lastPressedCube.ResetColor();
            }

            _currentPressedCube = pressedCube;
        }

        #endregion
    
        private void Player_OnNumberPlacedOnBoard(int number)
        {
            PlaceNumberOnBoard(number);
        }

        private void LevelManager_OnLevelChange(SudokuDataContainer sudokuDataContainer)
        {
            SetPlayableBoard(sudokuDataContainer);
            ResetButtonsOnLevelChange();
        }

        private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
        {
            _systemLogger.Log("OnReloadLevelDataAfterSolution: GridSystem playable board reset", this);
            _playable = container.GetPlayableBoard();
            ResetButtonsOnLevelChange();
        }

        public void PlaceNumberOnBoard(int number)
        {  
            if(_currentPressedCube != null)
            {
                _currentPressedCube.SetNumText(number);
                _systemLogger.Log("NUMBER CHANGED: " + number + "   Position: " + _currentPressedCube.GetPoint().ToString(), this);
            
                int x = _currentPressedCube.GetPoint().X;
                int y = _currentPressedCube.GetPoint().Y;
                _playable[x,y] = number;
            }
        }

        public void ResetCubeOnUndo(Point.Point point)
        {
            _sudokuBoardGridPositions[point.X, point.Y].SetNumText(0);

            if(_lastPressedCube != null)
                _lastPressedCube.ResetColor();
        }

        public void PlaceHintOnBoard(Point.Point point, int number)
        {
            GridPosition hintGridPosition = _sudokuBoardGridPositions[point.X, point.Y];
            hintGridPosition.SetNumText(number);
            _playable[point.X, point.Y] = number;

            if(_currentPressedCube != null)
                _currentPressedCube.ResetColor();

            if(_lastPressedCube != null)
                _lastPressedCube.ResetColor();

            // show the hint on the board
            hintGridPosition.LerpColorOnHintEffect();
        }
    
        private void ResetButtonsOnLevelChange()
        {
            if(_lastPressedCube != null)
                _lastPressedCube.ResetColor();

            if (_currentPressedCube != null)
                _currentPressedCube.ResetColor();

            _currentPressedCube = null;
            _lastPressedCube = null;
        }
    }
}
