using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingItem : MonoBehaviour
{
    [SerializeField] private SettingType settingType;
    [SerializeField] private GameObject _activeIcon;
    [SerializeField] private GameObject _unActiveIcon;

    // [SerializeField] private GameObject on;
    // [SerializeField] private GameObject off;

    private void Start()
    {
        CheckState();
    }

    public void Toggle()
    {
        switch (settingType)
        {
            case SettingType.Vibration:
                Data.VibrateState = !Data.VibrateState;
                break;
            case SettingType.Music:
                Data.MusicState = !Data.MusicState;
                if (Data.MusicState)
                {
                    SoundController.Instance.PlayBackground(SoundType.BackgroundHome);
                }
                else
                {
                    SoundController.Instance.PauseBackground();
                }
                break;
            case SettingType.Sound:
                Data.SoundState = !Data.SoundState;
                break;
        }

        CheckState();
    }

    private void CheckState()
    {
        switch (settingType)
        {
            case SettingType.Vibration:
                SetState(Data.VibrateState);
                break;
            case SettingType.Music:
                SetState(Data.MusicState);
                break;
            case SettingType.Sound:
                SetState(Data.SoundState);
                break;
        }
    }

    private void SetState(bool state)
    {
        _activeIcon.SetActive(state);
        _unActiveIcon.SetActive(!state);
    }
}

enum SettingType
{
    Vibration,
    Music,
    Sound
}
