using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI level;

    private void Awake()
    {
        ChangeLevel();
    }

    private void Start()
    {
        EventController.CurrentLevelChanged = ChangeLevel;
    }

    private void ChangeLevel()
    {
        level.text = $"Level {Data.CurrentLevel + 1}";
    }
}
