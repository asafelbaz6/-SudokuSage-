using System.Collections.Generic;
using System.Linq;

namespace SerializedData
{
    [System.Serializable]
    public class SudokuDataContainer
    {
        public int level;
        public int[] solutionBoard;
        public int[] playableBoard;
        public Dictionary<string, int> deletedPositions;
    
        #region Constructors
        // Default constructor
        public SudokuDataContainer() { }

        public SudokuDataContainer(int level, int[,] solutionBoard, int[,] playableBoard, Dictionary<string, int> deletedPositions)
        {
            this.level = level;
            this.solutionBoard = solutionBoard.Cast<int>().ToArray();
            this.playableBoard = playableBoard.Cast<int>().ToArray();
            this.deletedPositions = deletedPositions;
        }

        public SudokuDataContainer(int level, int[,] solutionBoard, int[,] playableBoard, Dictionary<Point.Point, int> deletedPositions)
        {
            this.level = level;
            this.solutionBoard = solutionBoard.Cast<int>().ToArray();
            this.playableBoard = playableBoard.Cast<int>().ToArray();
            this.deletedPositions = ConvertDeletedPositions(deletedPositions);
        }
        #endregion

        #region Getters and Setters
        public int[,] GetPlayableBoard() => Copy1DArrayTo2DArray(playableBoard);
        public int[,] GetSolutionBoard() => Copy1DArrayTo2DArray(solutionBoard);
        public int GetLevel() => level;
        public Dictionary<Point.Point, int> GetDeletedPositionsDictionary() => GetDeletedPositions();
        #endregion

   
        /// <summary>
        /// Converts a dictionary with Point keys to a dictionary with string keys.
        /// The Point keys are converted to strings using their ToString() method.
        /// </summary>
        /// <param name="deletedPositions">The dictionary with Point keys to convert.</param>
        /// <returns>A new dictionary with string keys representing the original Point keys.</returns>
        public Dictionary<string, int> ConvertDeletedPositions(Dictionary<Point.Point, int> deletedPositions)
        {
            Dictionary<string, int> deletedPositionsStringKey = new();
            foreach (var kvp in deletedPositions)
            {
                string keyAsString = kvp.Key.ToString();
                deletedPositionsStringKey[keyAsString] = kvp.Value;
            }

            return deletedPositionsStringKey;
        }
 
        /// <summary>
        /// Converts a dictionary that was stored in a JSON file with string keys to a dictionary with Point keys.
        /// The string keys are in the format "X,Y".
        /// </summary>
        public Dictionary<Point.Point, int> GetDeletedPositions()
        {
            var converted = new Dictionary<Point.Point, int>();
            foreach (var kvp in deletedPositions)
            {
                var cleanedKey = kvp.Key.Substring(1, kvp.Key.Length - 2);
                var coordinates = cleanedKey.Split(',');
                int x = int.Parse(coordinates[0]);
                int y = int.Parse(coordinates[1]);
                converted[new Point.Point { X = x, Y = y }] = kvp.Value;
            }

            return converted;
        }

        private int[,] Copy1DArrayTo2DArray(int[] source)
        {
            int gridSize = 9;
            int[,] copy = new int[gridSize, gridSize];
            for(int i = 0; i < gridSize; i++)
            {
                for(int j = 0; j < gridSize; j++)
                {
                    copy[i, j] = source[i * gridSize + j];
                }
            }

            return copy;
        }
    }
}
