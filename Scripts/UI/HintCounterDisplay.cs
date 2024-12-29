using System;
using Gameplay;
using Levels;
using SerializedData;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using Zenject;

public class HintCounterDisplay : MonoBehaviour
{
    #region References
    [Header("References")]
    [ShowInInspector] private Hint _hint;
    [ShowInInspector] private LevelManager _levelManager;
    [SerializeField] private TextMeshProUGUI hintCounterText;
    #endregion

    [Header("Properties")]
    [ShowInInspector] private int _maxHintCounter = 0;
    [ShowInInspector] private int _currentHintcounter = 0;

    [Inject]
    private void Construct(Hint hint, LevelManager levelManager)
    {
        _hint = hint;
        _levelManager = levelManager;
    }
    private void Start() 
    {
        if (_hint == null) return;

        _maxHintCounter = _hint.GetMaxHintCounter();
        _currentHintcounter = _hint.GetHintCounter();

        UpdateHintCounterUI();
    }

    #region Subscribing and Unsubscribing to Events
    private void OnEnable() 
    {
        _hint.OnHintUsed += Hint_OnHintUsed;
        _levelManager.OnReloadLevelDataAfterSolution += LevelManager_OnReloadLevelDataAfterSolution;
    }

    private void OnDisable()
    {
        _hint.OnHintUsed -= Hint_OnHintUsed;
        _levelManager.OnReloadLevelDataAfterSolution -= LevelManager_OnReloadLevelDataAfterSolution;
    }
    #endregion
    
    private void LevelManager_OnReloadLevelDataAfterSolution(SudokuDataContainer container)
    {
        // reset the hint counter
        _currentHintcounter = _maxHintCounter;
        UpdateHintCounterUI();
    }

    private void Hint_OnHintUsed()
    {
        _currentHintcounter--;
        UpdateHintCounterUI();
    }

    private void UpdateHintCounterUI()
    {
        hintCounterText.text = $"{_currentHintcounter}/{_maxHintCounter}";
    }
}
