using System;
using System.Collections.Generic;
using Levels;
using Persistent_Singletons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace Sudoku_Generator
{
    public class SudokuGenerator : MonoBehaviour
    {
        /// <summary>
        /// This class is not active right now in any scene.
        /// It is used to generate a valid Sudoku board and solve it.
        /// </summary>

        private const int GridSize = 9;
        private int[,] _solutionBoard = new int[GridSize, GridSize];
        private readonly int[,] _playableBoard = new int[GridSize, GridSize];
        private List<Point.Point> _availablePositions;
        private Dictionary<Point.Point, int> _deletedPositions = new();
        [SerializeField] private int tryToRemoveXCells = 40;
   
        public event Action<int[,]> OnSudokuGenerated;

        [ShowInInspector] private LevelManager _levelManager;

        [Header("Debug")]
        [Inject(Id = "MapLogger")] [ShowInInspector] private Logger _mapLogger;

        [Inject]
        private void Construct(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        #region Getters and Setters
        private List<Point.Point> GetAvailablePositions() => new(_availablePositions);
        #endregion

        public Dictionary<Point.Point, int> CopyDeletedPositions(Dictionary<Point.Point, int> copyDeletedPositions) => new(copyDeletedPositions);
        public void CopySolutionBoardToPlayableBoard() => CopyBoard(_solutionBoard, _playableBoard);


        // this function generates the sudoku
        // can be linked to a button and create new boards
        public void GenerateSudoku()
        {
            _solutionBoard = new int[GridSize, GridSize];
            if (!FillSudoku(_solutionBoard))
            {
                _mapLogger.Log("Failed to fill the board", this);
                return;
            }

            // the board is now valid
            // should initialize the list of available positions
            CreateAvailablePositionsList();
            CopySolutionBoardToPlayableBoard();

            // Remove numbers from the board
            // tryToRemoveXCells + 1 for every 10 levels
            // for 200 levels we will try to remove 60 cells
            int cellsToRemove = tryToRemoveXCells + (SavingSystem.Instance.LoadLevelsData().Count / 10);
            RemoveNumbers(cellsToRemove);


            OnSudokuGenerated?.Invoke(_playableBoard);

            // add the new level to the list of levels - the function is not used anymore since all the 200 levels are in the Json file
            // UNCOMMENT THIS IF YOU WANT TO ADD NEW LEVELS
            /*
        //int nextLevel = levelManager.GetAllSudokuDataContainers().Count;
        //levelManager.AddLevelToSudokuDataContainers(new SudokuDataContainer(nextLevel, solutionBoard, playableBoard, deletedPositions));
        */
        
            _mapLogger.Log("Sudoku generated", this);
        }


        private void CreateAvailablePositionsList()
        {
            _availablePositions = new List<Point.Point>();

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    _availablePositions.Add(new Point.Point(row, col));
                }
            }
        }


        // Helper method to copy the board
        private void CopyBoard(int[,] source, int[,] destination)
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    destination[i, j] = source[i, j];
                }
            }
        }

    
        bool IsSafe(int[,] board, int row, int col, int num)
        {
            for (int x = 0; x < GridSize; x++)
            {
                if (board[row, x] == num || board[x, col] == num)
                    return false;
            }

            int startRow = row - row % 3;
            int startCol = col - col % 3;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i + startRow, j + startCol] == num)
                        return false;
                }
            }
            return true;
        }

        bool FillSudoku(int[,] board)
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (board[row, col] == 0)
                    {
                        List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        Shuffle(numbers);

                        foreach (int num in numbers)
                        {
                            if (IsSafe(board, row, col, num))
                            {
                                board[row, col] = num;
                                if (FillSudoku(board))
                                    return true;
                                board[row, col] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1); 
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    
    
        /// <summary>
        /// This function tries up to maxAttempts times to remove the specified number of cells.
        /// If it can't remove the specified number of cells in maxAttempts tries, it will return the best result it could find.
        /// This function modifies the playableBoard and deletedPositions fields of this object.
        /// </summary>
        /// <param name="numberOfCellsToRemove">The number of cells to remove from the board.</param>
        /// <param name="maxAttempts">The maximum number of attempts to try to remove the specified number of cells. Defaults to 3.</param>
        public void RemoveNumbers(int numberOfCellsToRemove, int maxAttempts = 3)
        {
            int bestCellsRemoved = 0;
            int[,] bestBoardState = new int[GridSize, GridSize];

            // Initialize the backup board
            int[,] initialBoard = new int[GridSize, GridSize];
            CopyBoard(_solutionBoard, initialBoard);  // Copy the solved board to the initial state

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                // Reset to the initial board state for each attempt
                CopyBoard(initialBoard, _playableBoard);

                int cellsRemoved = 0;
                List<Point.Point> availablePositions = GetAvailablePositions();
                Dictionary<Point.Point, int> tempDeletedPositionsDict = new();
                Shuffle(availablePositions);

                foreach (Point.Point point in availablePositions)
                {
                    if (cellsRemoved >= numberOfCellsToRemove)
                        break;

                    int row = point.X;
                    int col = point.Y;

                    if (_playableBoard[row, col] != 0)
                    {
                        int backup = _playableBoard[row, col];
                        _playableBoard[row, col] = 0;

                        if (HasUniqueSolution(_playableBoard))
                        {
                            cellsRemoved++;
                            tempDeletedPositionsDict.Add(point, backup);
                        }
                        else
                        {
                            _playableBoard[row, col] = backup; // Restore if it does not have a unique solution
                        }
                    }
                }

                // Update best result if this attempt is better
                if (cellsRemoved > bestCellsRemoved)
                {
                    bestCellsRemoved = cellsRemoved;
                    CopyBoard(_playableBoard, bestBoardState);
                    _deletedPositions = CopyDeletedPositions(tempDeletedPositionsDict);
                }

                if (cellsRemoved >= numberOfCellsToRemove)
                    break;

                _mapLogger.Log($"attempt: {attempt}  removed: {cellsRemoved}", this);
            }

            // Apply the best result to the final board
            CopyBoard(bestBoardState, _playableBoard);
            _mapLogger.Log($"Successfully removed {bestCellsRemoved} cells in the best attempt out of {maxAttempts}.", this);
        }

        public bool HasUniqueSolution(int[,] board)
        {
            int solutionCount = 0;
            bool hasMultipleSolutions = false;
            CountSolutions(board, ref solutionCount, ref hasMultipleSolutions);
            return solutionCount == 1;
        }


        /// <summary>
        /// Recursively counts the number of solutions for the given Sudoku board.
        /// This function modifies the board by placing numbers in empty cells to test solutions.
        /// </summary>
        /// <param name="board">The Sudoku board represented as a 2D array of integers.</param>
        /// <param name="solutionCount">Reference to an integer that keeps track of the number of solutions found.</param>
        /// <param name="hasMultipleSolutions">Reference to a boolean that indicates if more than one solution has been found.</param>
        private void CountSolutions(int[,] board, ref int solutionCount, ref bool hasMultipleSolutions)
        {
            if (solutionCount > 1)
            {
                hasMultipleSolutions = true;
                return;
            }

            int row, col;
            if (!FindEmptyCell(board, out row, out col))
            {
                solutionCount++;
                return;
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsSafe(board, row, col, num))
                {
                    board[row, col] = num;
                    CountSolutions(board, ref solutionCount, ref hasMultipleSolutions);
                    board[row, col] = 0;

                    if (hasMultipleSolutions)
                    {
                        return;
                    }
                }
            }
        }

        private bool FindEmptyCell(int[,] board, out int row, out int col)
        {
            for (row = 0; row < GridSize; row++)
            {
                for (col = 0; col < GridSize; col++)
                {
                    if (board[row, col] == 0)
                    {
                        return true;
                    }
                }
            }
            row = col = -1;
            return false;
        }
    }
}