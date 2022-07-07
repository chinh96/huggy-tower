using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PopupController : Singleton<PopupController>
{
    [SerializeField] private Popup[] popupPrefabs;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasScaler canvasScaler;

    private Dictionary<Type, Popup> popupDict;
    private LinkedList<Popup> popups;
    private bool initialized;
    [SerializeField] private Camera cameraUi;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Debug.Log(cameraUi.aspect + "cam");
        canvasScaler.matchWidthOrHeight = cameraUi.aspect > .6f ? 1 : 0;
        Initialize();
    }

    public void Initialize()
    {
        if (initialized) return;

        popupDict = new Dictionary<Type, Popup>();
        popups = new LinkedList<Popup>();
        foreach (var prefab in popupPrefabs)
        {
            var popup = Instantiate(prefab, canvas.transform);

            popup.gameObject.SetActive(false);

            popup.Initialize(this);
            popup.SetOrder(canvas.sortingOrder + 1);

            var type = popup.GetType();
            popupDict.Add(type, popup);
        }
        initialized = true;
    }

    public Popup GetPopup<T>(){
        if(!popupDict.TryGetValue(typeof(T), out var popup)){
            Debug.Log("Cannot find that popup!");
        }
        return popup;
    }

    public void Show<T>(object data = null, ShowAction showAction = ShowAction.DismissCurrent)
    {
        if (!popupDict.TryGetValue(typeof(T), out var popup))
        {
            Debug.Log("Cannot find that popup!");
            return;
        }

        SoundController.Instance.PlayOnce(SoundType.OpenPopup);

        Show(data, popup, showAction);

        if (GameController.Instance)
        {
            GameController.Instance.SetEnableLeanTouch(false);
        }
    }

    public void Show(object data, Popup basePopup, ShowAction showAction = ShowAction.DismissCurrent)
    {
        var t = GetTopPopup();
        if (t == basePopup) return;

        if (t != null)
        {
            t.showAction = showAction;
            switch (showAction)
            {
                case ShowAction.DoNothing:
                    break;
                case ShowAction.DismissCurrent:
                    RemoveLast();

                    t.Dismiss(true);
                    break;
                case ShowAction.PauseCurrent:
                    t.Pause(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(showAction), showAction, null);
            }
        }

        AddLast(basePopup);

        basePopup.Show(data);
    }

    public void Dismiss<T>()
    {
        if (!popupDict.TryGetValue(typeof(T), out var popup))
        {
            Debug.Log("Cannot find that popup!");
            return;
        }

        Dismiss(popup);
    }

    public void DismissCurrent()
    {
        var last = popups.Last;
        if (last != null)
        {
            Dismiss(last.Value);
        }
    }

    public void Dismiss(Popup basePopup)
    {
        Remove(basePopup); // remove and reorder

        basePopup.Dismiss(true);

        var t = GetTopPopup();

        if (GameController.Instance && popups.Count == 0)
        {
            GameController.Instance.SetEnableLeanTouch(true);
        }

        if (t == null) return;

        if (t.showAction == ShowAction.DoNothing)
        {
            t.Resume(false);
        }
        else
        {
            t.Resume(true);
        }
    }

    void Reorder()
    {
        var p = popups.First;
        var i = canvas.sortingOrder;
        while (p != null)
        {
            p.Value.SetOrder(++i);
            p = p.Next;
        }
    }

    void AddLast(Popup basePopup)
    {
        if (popups.Contains(basePopup))
        {
            popups.Remove(basePopup);
        }

        popups.AddLast(basePopup);
        Reorder();
    }

    void Remove(Popup basePopup)
    {
        popups.Remove(basePopup);
        Reorder();
    }

    void RemoveLast()
    {
        popups.RemoveLast();
        Reorder();
    }

    public Popup GetTopPopup()
    {
        return popups.Last?.Value;
    }

    public void DismissAll()
    {
        while (true)
        {
            Popup popup = GetTopPopup();
            if (popup == null)
            {
                break;
            }
            popup.Close();
        }
    }
}
