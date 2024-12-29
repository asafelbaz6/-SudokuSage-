using Levels;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class MapCounterDisplay : MonoBehaviour
    {
        #region References
        [Header("References")]
        [ShowInInspector] private LevelMapCreator _levelMapCreator;
        [ShowInInspector] private Fader _fader;
        [SerializeField] private TextMeshProUGUI mapCounterText;
        #endregion

        [Header("Properties")]
        [ShowInInspector] private int _maxMapCounter = 0;
        [ShowInInspector] private int _currentMapCounter = 0;

        [Inject]
        private void Construct(LevelMapCreator levelMapCreator, Fader fader)
        {
            _levelMapCreator = levelMapCreator;
            _fader = fader;
        }
    
        private void Start() 
        {
            if (_levelMapCreator == null) return;

            _maxMapCounter = _levelMapCreator.GetMaxLevelCounter();
            _currentMapCounter = _levelMapCreator.GetLevelMapCounter();

            UpdateHintCounterUI();
        }

        #region Subscribing and Unsubscribing to Events
        private void OnEnable() 
        {
            _fader.OnFadeCompleteLevelChange += Fader_OnFadeCompleteLevelChange;
        }

        private void OnDisable()
        {
            _fader.OnFadeCompleteLevelChange -= Fader_OnFadeCompleteLevelChange;
        }

        #endregion

        private void Fader_OnFadeCompleteLevelChange(LevelChange levelChange)
        {
            if(levelChange == LevelChange.Next)
                _currentMapCounter++;
            else
                _currentMapCounter--;

            UpdateHintCounterUI();
        }
        private void UpdateHintCounterUI()
        {
            mapCounterText.text = $"MAP: {_currentMapCounter}/{_maxMapCounter}";
        }

        // x = 350
        // width = 400
        // font = 60
        // MAP: 1/5
    }
}
