using System;
using Levels;
using SerializedData;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class LevelScoreDisplay : MonoBehaviour
    {
        #region References
        [ShowInInspector] private LevelManager _levelManager;
        [SerializeField] private TextMeshProUGUI scoreText;
        #endregion

        [Inject]
        private void Construct(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        private void Awake()
        {
            scoreText = GetComponent<TextMeshProUGUI>();
        }

        #region Subscribing And Unsubscribing To Events
        private void OnEnable() 
        {
            _levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
            _levelManager.OnLevelChange += LevelManager_OnLevelChange;
        }

        private void OnDisable() 
        {
            _levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
            _levelManager.OnLevelChange -= LevelManager_OnLevelChange;
        }
        #endregion

        private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
        {
            SetScoreText(container.GetLevel());
        }
    
        private void LevelManager_OnLevelChange(SudokuDataContainer container)
        {
            SetScoreText(container.GetLevel());
        }

        private void SetScoreText(int level) 
        {
            var showLevel = level + 1; // + 1 because the level is 0-indexed 
            scoreText.text = $"Level: {showLevel}";
        }
    }
}
