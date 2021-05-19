using System;
using Lance;
using Lance.Engine.Tool;
using TMPro;
using UnityEngine;

public class PopupLose : PopupBase
{
    [SerializeField] private TextMeshProUGUI txtMessage;
    [SerializeField] private UniButton btnReplayLevel;
    [SerializeField] private UniButton btnSkipLevel;


    private Action _actionReplayLevel;
    private Action _actionSkipLevel;

    public void Initialized(Action actionReplayLevel, Action actionSkipLevel, string message)
    {
        _actionReplayLevel = actionReplayLevel;
        _actionSkipLevel = actionSkipLevel;
        txtMessage.text = message;

        btnReplayLevel.onClick.RemoveListener(OnReplayLevelButtonPressed);
        btnReplayLevel.onClick.AddListener(OnReplayLevelButtonPressed);

        btnSkipLevel.onClick.RemoveListener(OnSkipLevelButtonPressed);
        btnSkipLevel.onClick.AddListener(OnSkipLevelButtonPressed);
    }

    private void OnSkipLevelButtonPressed() { _actionSkipLevel?.Invoke(); }

    private void OnReplayLevelButtonPressed() { _actionReplayLevel?.Invoke(); }
}