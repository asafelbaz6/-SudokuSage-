using System.Collections.Generic;
using System.Linq;
using Levels;
using SerializedData;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Persistent_Singletons
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [ShowInInspector] private Dictionary<int, int> _levelStarsDic = new();
        [SerializeField] private int playableLevels = 1;
        [ShowInInspector] private int _levelToLoad;
        [ShowInInspector] private int _totalStarsCollected;

        [ShowInInspector] private List<SudokuDataContainer> _sudokuDataContainersList = new(); 

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                Debug.Log("THERES MORE THAN 1 GAME MANAGER");   
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() 
        {
            _sudokuDataContainersList = SavingSystem.Instance.LoadLevelsData();
            _levelToLoad = 1;
        
            _levelStarsDic = SavingSystem.Instance.LoadLevelDictionary();
            playableLevels = _levelStarsDic?.Count > 0 ?
                _levelStarsDic.Max(x => x.Key) + 1 :
                1;

            _totalStarsCollected = CalculateTotalStarsInLevelStarDic();
        }

        #region Getters and Setters

        public int GetLevelToLoad() => _levelToLoad;
        public int GetPlayableLevels() => playableLevels;
        public void SetPlayableLevels(int setPlayableLevels) => this.playableLevels = setPlayableLevels;
        public Dictionary<int, int> GetLevelDictionary() => new(_levelStarsDic);
        public List<SudokuDataContainer> GetAllSudokuDataContainers() => new(_sudokuDataContainersList); 
        public void SetStarsInSpecificLevel(int level, int stars) => _levelStarsDic[level] = stars;
        public int GetStarsInSpecificLevel(int level) => _levelStarsDic.ContainsKey(level) ? _levelStarsDic[level] : 0;
    
        #endregion

        public void IncrementPlayableLevles() => playableLevels++;
        public void IncrementTotalStarsCollected(int starsCollected) => _totalStarsCollected += starsCollected; 
        public int GetTotalStarsCollected() => _totalStarsCollected;

        public void SetLevelToLoad(int level)
        {
            if(level > playableLevels)
            {
                Debug.Log($"This Level is invalid! {level} > {playableLevels}", this);
                return;
            }

            _levelToLoad = level;
        
            if(_levelStarsDic.ContainsKey(level))
            {
                Debug.Log("levelStarsDic contains key: " + level);
            }
            else
            {
                _levelStarsDic.Add(level, 0);
            }

            var fader = FindObjectOfType<Fader>(); 
            if(fader != null)
            {
                fader.FadeOutBetweenLevels(LevelChange.Next);
                return;
            }
            SceneManager.LoadSceneAsync(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex + 1);
        }

        // this is getting called after winning a level
        public void BackToMenuOnWin()
        {
            IncrementPlayableLevles();

            // The Gamemanager is living in all the scenes, so we have to find the current active Fader
            var fader = FindObjectOfType<Fader>(); 
            if(fader != null)
            {
                fader.FadeOutBetweenLevels(LevelChange.Next);
                return;
            }

            SceneManager.LoadSceneAsync(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex - 1);
        }

        private int CalculateTotalStarsInLevelStarDic()
        {
            var total = 0;
            foreach(var level in _levelStarsDic)
            {
                total += level.Value;
            }

            return total;
        }
    }
}
