using System;
using Gameplay;
using Levels;
using SerializedData;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace Grid
{
    public class MapGenerator : MonoBehaviour
    {   
        [Inject] private DiContainer _diContainer; 
        
        #region References
        [Header("Debug")]
        [Inject(Id = "MapLogger")] [ShowInInspector] private Logger _mapLogger;
        [ShowInInspector] private LevelManager _levelManager;
        [ShowInInspector] private Submit _submitButton;
        #endregion

        #region Grid stats
        [SerializeField] private int firstCubeYPosition = 650;
        [SerializeField] private int firstCubeXPosition = -400;
        [SerializeField] private int lastRowYPos = -450;
        [SerializeField] private int gapBetweenCubes = 100;
        [SerializeField] private int gapBetweenRows = 100;
        #endregion

        [SerializeField] private GameObject cubeImage;
        [SerializeField] private GameObject clickableNumber;
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private Transform grid;
        private const int GridSize = 9;

        private readonly GameObject[,] _sudokuBoardGameOjbects = new GameObject[GridSize, GridSize];
        private readonly GridPosition[,] _sudokuBoardGridPositions = new GridPosition[GridSize, GridSize];

        private void Awake()
        {
            CreateBoardsOnScreen();
        }

        [Inject]
        private void Construct(LevelManager levelManager, Submit submitButton)
        {
            _levelManager = levelManager;
            _submitButton = submitButton;
        }

        #region Subscribing and Unsubscribing
        private void OnEnable() 
        {
            _levelManager.OnLevelChange += LevelManager_OnLevelChange;   
            _levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
            _submitButton.OnLevenCompleteWrongSolutionUIPopUp += SubmitButton_OnLevenCompleteWrongSolutionUIPopUp;
        }

        private void OnDisable()
        {
            _levelManager.OnLevelChange -= LevelManager_OnLevelChange;
            _levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
            _submitButton.OnLevenCompleteWrongSolutionUIPopUp -= SubmitButton_OnLevenCompleteWrongSolutionUIPopUp;
        }
        #endregion

        #region Getters and Setters
        public GridPosition[,] GetSudokuBoardGridPositions()
        {
            var rows = _sudokuBoardGridPositions.GetLength(0);
            var cols = _sudokuBoardGridPositions.GetLength(1);
            var copy = new GridPosition[rows, cols];

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    copy[i, j] = _sudokuBoardGridPositions[i, j];
                }
            }

            return copy;
        }
        #endregion

        private void LevelManager_OnLevelChange(SudokuDataContainer sudokuDataContainer)
        {
            SetBoardWithSudokuDataContainer(sudokuDataContainer);
        }

        private void CreateBoardsOnScreen()
        {
            for (var i = 0; i < GridSize; i++)
            {
                for (var j = 0; j < GridSize; j++)
                {
                    //var cube = Instantiate(cubeImage, grid);
                    var cube = _diContainer.InstantiatePrefab(cubeImage, grid);
                    var xPos = firstCubeXPosition + j * gapBetweenCubes + mainCanvas.transform.position.x;
                    var yPos = firstCubeYPosition - i * gapBetweenRows + mainCanvas.transform.position.y;
                    cube.transform.position = new Vector3(xPos, yPos, 0);
                    cube.name = "Cube (" + i + ", " + j + ")";
                    _sudokuBoardGameOjbects[i, j] = cube;

                    _sudokuBoardGridPositions[i, j] = cube.GetComponent<GridPosition>();
                    _sudokuBoardGridPositions[i, j].SetRowAndCol(i, j);
                }
            }

            CreateButtons();
        }

        // creates the non pressable buttons
        private void CreateButtons()
        {
            // Spawn the playerable buttons
            for(var i = 0; i < GridSize; i++)
            {
                var clickable = _diContainer.InstantiatePrefab(clickableNumber, grid);
                var xPos = firstCubeXPosition + i * gapBetweenCubes + mainCanvas.transform.position.x;
                var yPos = lastRowYPos + mainCanvas.transform.position.y;
                clickable.transform.position = new Vector3(xPos, yPos, 0);
                clickable.name = "Clickable (Last Row Clickable: " + i + ")";
                var gridPos = clickable.GetComponent<GridPosition>();
                gridPos.SetNumText(i + 1);
            }
        }

        // public void SetBoardWithNumbers(int [,] playableBoard)
        // {
        //     var sudokuBoard = playableBoard;
        //     for (var i = 0; i < GridSize; i++)
        //     {
        //         for (var j = 0; j < GridSize; j++)
        //         {
        //             _sudokuBoardGridPositions[i, j].SetNumText(sudokuBoard[i, j]);
        //         }
        //     }
        // }

        private void SetBoardWithSudokuDataContainer(SudokuDataContainer sudokuDataContainer)
        {
            var sudokuBoard = sudokuDataContainer.GetPlayableBoard();
            for (var i = 0; i < GridSize; i++)
            {
                for (var j = 0; j < GridSize; j++)
                {
                    _sudokuBoardGridPositions[i, j].SetNumText(sudokuBoard[i, j]);
                }
            }
        }

    
        private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
        {
            _mapLogger.Log("OnLevelRestartAfterWrongSolution: ResetVisibleBoard", this);
            ResetVisibleBoard(container);
        }

    
        private void ResetVisibleBoard(SudokuDataContainer container)
        {
            var playableBoard = container.GetPlayableBoard();
            for (var i = 0; i < GridSize; i++)
            {
                for (var j = 0; j < GridSize; j++)
                {
                    _sudokuBoardGridPositions[i, j].SetNumText(playableBoard[i, j]);
                    // cancel any tween
                    _sudokuBoardGridPositions[i, j].CancelTween();
                }
            }

            _mapLogger.Log("OnLevelRestartAfterWrongSolution: ResetVisibleBoard = completed", this);
        }

        // Cancels all the tweens, the value is not used
        private void SubmitButton_OnLevenCompleteWrongSolutionUIPopUp()
        {
            _mapLogger.Log("SubmitButton_OnLevenCompleteWrongSolutionUIPopUp: Cancel Tweens", this);

            for (var i = 0; i < GridSize; i++)
            {
                for (var j = 0; j < GridSize; j++)
                {
                    _sudokuBoardGridPositions[i, j].CancelTween();
                }
            }
        }

    }
}