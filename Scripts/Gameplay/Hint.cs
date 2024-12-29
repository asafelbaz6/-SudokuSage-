using System;
using System.Collections.Generic;
using Grid;
using Levels;
using SerializedData;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace Gameplay
{
    public class Hint : MonoBehaviour
    {
        #region References
        [ShowInInspector] private Player _player;
        [ShowInInspector] private Undo _undo;
        [ShowInInspector] private LevelManager _levelManager;
        [ShowInInspector] private GridSystem _gridSystem;
        [ShowInInspector] private Submit _submitButton;
        #endregion

        [ShowInInspector] private Dictionary<Point.Point, int> _hintMap = new();
        [ShowInInspector] private List<Point.Point> _hintList = new();
        private const int OriginalHintCounter = 3;
        private int _hintCounter;
        public event Action OnHintUsed;

        [Header("Debug")]
        [Inject(Id = "SystemLogger")] [ShowInInspector] private Logger _systemLogger;

        private void Awake()
        {
            _hintCounter = OriginalHintCounter;
        }
    
        [Inject]
        private void Construct(Player player, Undo undo, LevelManager levelManager, GridSystem gridSystem ,Submit submitButton)
        {
            _player = player;
            _undo = undo;
            _levelManager = levelManager;
            _gridSystem = gridSystem;
            _submitButton = submitButton;
        }

        private void Start() 
        {
            CreateDictionary();
        }

        #region Subscribing / Unsubscribing to events
        private void OnEnable()
        {
            _player.OnNumberPlacedOnBoardDeletedPosition += Player_OnNumberPlacedOnBoardDeletedPosition;
            _undo.OnUndoAddPositionToHintDictionary += Undo_OnUndoAddPositionToHintDictionary;
            _levelManager.OnChangeHintDictionary += LevelManager_OnChangeLevelChangeHintDictionary;
            _levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
        }

        private void OnDisable()
        {
            _player.OnNumberPlacedOnBoardDeletedPosition -= Player_OnNumberPlacedOnBoardDeletedPosition;
            _undo.OnUndoAddPositionToHintDictionary -= Undo_OnUndoAddPositionToHintDictionary;
            _levelManager.OnChangeHintDictionary -= LevelManager_OnChangeLevelChangeHintDictionary;
            _levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
        }
        #endregion

        #region Getters and Setters
        public int GetHintCounter() => _hintCounter;
        public int GetMaxHintCounter() => OriginalHintCounter;
        #endregion

        private void Undo_OnUndoAddPositionToHintDictionary(Point.Point point, int number)
        {
            AddHintToDictionaryFromUndoStack(point, number);
        }

        private void AddHintToDictionaryFromUndoStack(Point.Point point, int number)
        {
            if (_hintMap.ContainsKey(point)) return;
            
            _hintMap.Add(point, number);
            if(!_hintList.IsNullOrEmpty())
            {
                _hintList.Add(point);
                _systemLogger.Log($"hintList is not empty. Count: {_hintList.Count}, point added: {point}", this);
            }
            else
            {
                // Convert the dictionary keys to a list
                _hintList = new List<Point.Point>(_hintMap.Keys);
                _systemLogger.Log("AddHintToDictionaryFromUndoStack   hintList: Created!. Count: " + _hintList.Count, this);
            }

            _systemLogger.Log("AddHintToDictionary: " + point.ToString(), this);
            _systemLogger.Log("AddHintToHintList: " + point.ToString(), this);
        }

        private void CreateDictionary()
        {
            _hintMap = _levelManager.GetCurrentLevelDeletedPositionsDictionary();
        }

        // this is getting called from the Hint button
        public void ShowHintAndRemoveFromDictionary()
        {
            var point = GetRandomPointFromDictionary(); 
            if(point == null || !_hintMap.ContainsKey(point))
            {
                _systemLogger.Log("Can't find hint on this position", this);
                return;
            }
            if(_hintCounter == 0)
            {
                _systemLogger.Log("hintCounter is 0, can't get any more hints", this);
                return;
            }

            _gridSystem.PlaceHintOnBoard(point, _hintMap[point]);
         
            _hintMap.Remove(point);
            _hintCounter--;

            if(_hintMap.Count > 0 && _hintCounter == 0)
            {
                _hintMap.Clear();
                _systemLogger.Log("hintMap cleared", this);
            }

            // update the hint counter UI
            OnHintUsed?.Invoke();

            _systemLogger.Log("hintCounter: " + _hintCounter, this);
        }

        private void Player_OnNumberPlacedOnBoardDeletedPosition(Point.Point point)
        {
            RemoveHintFromDictionary(point);
        }
    
        private void RemoveHintFromDictionary(Point.Point point)
        {
            _systemLogger.Log("RemoveHintFromDictionary: " + point.X + ", " + point.Y, this);
            _hintMap.Remove(point);

            if(!_hintList.IsNullOrEmpty())
                _hintList.Remove(point);
        }

        private Point.Point GetRandomPointFromDictionary()
        {
            if (_hintMap.Count == 0)
            {
                _systemLogger.Log("The hintMap is empty. Cannot get a random point.", this);
                return null; 
            }

            // Convert the dictionary keys to a list
            if(_hintList.IsNullOrEmpty())
            {
                _hintList = new List<Point.Point>(_hintMap.Keys);
                _systemLogger.Log("hintList: Created!. Count: " + _hintList.Count, this) ;
            }
        
            var randomIndex = UnityEngine.Random.Range(0, _hintList.Count);
            
            var result = _hintList[randomIndex];
            _hintList.RemoveAt(randomIndex);

            return result;
        }

        private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
        {
            _systemLogger.Log($"OnReloadLevelDataAfterSolution: reset hintMap with the current level: {container.GetLevel()}", this);
            _hintMap.Clear();
            _hintList.Clear();
            _hintMap = container.GetDeletedPositionsDictionary();
            _systemLogger.Log("hintMap: " + _hintMap.Count, this);
            _hintCounter = OriginalHintCounter;
        }
     

        [Button("Change hint map")]
        public void LevelManager_OnChangeLevelChangeHintDictionary(Dictionary<Point.Point, int> deletedPositions)
        {
            _hintMap = deletedPositions;
        }
    }
}
