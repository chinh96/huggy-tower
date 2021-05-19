using System;
using Lance;
using Lance.Engine.Tool;
using TMPro;
using UnityEngine;

public class PopupWin : PopupBase
{
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private UniButton btnNextLevel;

    private Action _actionNextLevel;

    public void Initialized(Action actionNextLevel, string message)
    {
        _actionNextLevel = actionNextLevel;
        txtMessage.text = message;

        btnNextLevel.onClick.RemoveListener(OnNextLevelButtonPressed);
        btnNextLevel.onClick.AddListener(OnNextLevelButtonPressed);
    }

    private void OnNextLevelButtonPressed() { _actionNextLevel?.Invoke(); }
}