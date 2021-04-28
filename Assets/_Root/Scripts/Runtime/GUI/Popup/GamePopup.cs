using System;
using Lance.Common;
using Lance.Engine.Tool;
using UnityEngine;

namespace Lance.TowerWar.UI
{
    public class GamePopup : Singleton<GamePopup>
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Transform container;

        [SerializeField] private PopupWin popupWinPrefab;

        public IPopupHandler winHandler;

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
    }
}