using System;
using Lance.Common;
using Lance.Engine.Tool;
using Lance.TowerWar.Unit;
using UnityEngine;

namespace Lance.TowerWar.UI
{
    public class GamePopup : Singleton<GamePopup>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform container;

        [SerializeField] private PopupWin popupWinPrefab;
        [SerializeField] private PopupLose popupLosePrefab;


        public IPopupHandler winHandler;
        public IPopupHandler loseHandler;

        public void ShowPopupWin(Action actionNextLevel, string message)
        {
            if (winHandler != null)
            {
                if (winHandler.GameObject.activeSelf) return;

                Display();
                return;
            }

            winHandler = Instantiate(popupWinPrefab, container, false);
            Display();

            void Display()
            {
                var popup = (PopupWin) winHandler;
                popup.Initialized(actionNextLevel, message);
                Popup.Instance.Show(winHandler);
            }
        }

        public void ShowPopupLose(Action actionReplayLevel, Action actionSkipLevel, string message)
        {
            if (loseHandler != null)
            {
                if (loseHandler.GameObject.activeSelf) return;

                Display();
                return;
            }

            loseHandler = Instantiate(popupLosePrefab, container, false);
            Display();

            void Display()
            {
                var popup = (PopupLose) loseHandler;
                popup.Initialized(actionReplayLevel, actionSkipLevel, message);
                Popup.Instance.Show(loseHandler);
            }
        }
    }
}