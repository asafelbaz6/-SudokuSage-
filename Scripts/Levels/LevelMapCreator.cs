using System;
using Persistent_Singletons;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using Zenject;
using Logger = Logging.Logger;

namespace Levels
{
    public class LevelMapCreator : MonoBehaviour
    {   
        private const int Width = 5, Height = 8;

        #region References
        [Header("References")]
        [SerializeField] private Transform levelMap;
        
        [Inject(Id = "MapLogger")] [ShowInInspector] private Logger _mapLogger;
        private Level.Factory _levelFactory;
        [ShowInInspector] private Fader _fader;
        #endregion

        [Header("Level Map Properties")]
        [SerializeField] private int firstRowYPos = 700;
        [SerializeField] private int firstXPos = -400;
        [SerializeField] private int widthGapBetweenLevels = 200;
        [SerializeField] private int heightGapBetweenLevels = 175;

        private const int MaxLevelMapCounter = 5;
        [SerializeField] private int leveMapCounter = 1, levelCounter = 1;

        private readonly Level[] _levels = new Level[Width * Height];
        private readonly Tweenable[] _tweenables = new Tweenable[Width * Height];

        public event Action<LevelChange> OnMapLevelChange;

        [Inject]
        private void Construct(Level.Factory levelFactory, Fader fader)
        {
            _levelFactory = levelFactory;
            _fader = fader;
        }
        
        private void Start()
        {
            CreateBoardsOnScreen();
        }

        #region Getters and Setters
        public int GetLevelMapCounter() => leveMapCounter;
        public int GetMaxLevelCounter() => MaxLevelMapCounter;
        #endregion

        private void CreateBoardsOnScreen()
        {
            var levelsCompleted = GameManager.Instance.GetPlayableLevels();

            // creates the board
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    var level = _levelFactory.Create();//Instantiate(levelImage, levelMap); 
                    level.transform.parent = levelMap;
                    var xPos = firstXPos + j * widthGapBetweenLevels;
                    var yPos = firstRowYPos - i * heightGapBetweenLevels;
                    level.transform.localPosition = new Vector3(xPos, yPos, 0);
                
                    level.SetLevelNumberText(levelCounter);

                    level.SetAvailable(levelCounter <= levelsCompleted);

                    _levels[levelCounter - 1] = level;

                    _tweenables[levelCounter - 1] = level.GetComponentInChildren<Tweenable>();

                    level.ChangeStars(GameManager.Instance.GetStarsInSpecificLevel(levelCounter));

                    levelCounter++;
                }
            }
        }

        // this is getting called from the -> button
        public void NextLevelMap()
        {
            if(!_fader.GetCanTween()) 
            {
                _mapLogger.Log("fader.GetCanTween() == false", this);
                return;
            }

            if(leveMapCounter > 4)
            {
                _mapLogger.Log("Max level reached: " + (levelCounter + ((leveMapCounter - 1) * Width * Height)) + " " + 
                               (levelCounter + ((leveMapCounter - 1) * Width * Height) == 201), this);
                return;
            } 

            OnMapLevelChange?.Invoke(LevelChange.Next);

            FadeInBeforeMapTween(LevelChange.Next);
            leveMapCounter++;
        }

        // this is getting called from the <- button
        public void PreviousLevelMap()
        {
            if(!_fader.GetCanTween()) 
            {
                _mapLogger.Log("fader.GetCanTween() == false", this);
                return;
            }

            if(leveMapCounter <= 1)
            {
                _mapLogger.Log("leveMapCounter >= 1", this);
                return;
            }

            OnMapLevelChange?.Invoke(LevelChange.Previous);

            FadeInBeforeMapTween(LevelChange.Previous);
            leveMapCounter--;
        }

        private void FadeInBeforeMapTween(LevelChange levelChange)
        {
            if(_fader != null)
            {
                _fader.FadeOutLevelMap(ChangeLevelsLoop, levelChange);
            }
        }

        private void ChangeLevelsLoop(LevelChange levelChange)
        {
            for (var i = 0; i < _levels.Length; i++) 
            {
                ChangeLevels(i, levelChange);
            }
        }

        private void ChangeLevels(int i, LevelChange levelChange)
        {
            var levelsCompleted = GameManager.Instance.GetPlayableLevels();
            var number = _levels[i].GetLevelNumberText();

            var addOrSubMultiplier = levelChange == LevelChange.Next? 1 : -1;
            const int gridSize = Width * Height;

            var newNumber = number + (addOrSubMultiplier * gridSize);

            _levels[i].SetLevelNumberText(newNumber);
            _levels[i].SetAvailable(newNumber <= levelsCompleted);
        }
    }
}
