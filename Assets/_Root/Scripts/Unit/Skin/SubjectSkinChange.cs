using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SubjectSkinChange : MonoBehaviour, IObservable<SkinData>
{
    private List<IObserver<SkinData>> observers = new List<IObserver<SkinData>>();
    private List<SkinData> flights;
    public IDisposable Subscribe(IObserver<SkinData> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
        return new Unsubscriber<SkinData>(observers, observer);
    }

    public void Next(SkinData info)
    {
        foreach (var observer in observers)
            observer.OnNext(info);
    }

}


internal class Unsubscriber<SkinData> : IDisposable
{
    private List<IObserver<SkinData>> _observers;
    private IObserver<SkinData> _observer;

    internal Unsubscriber(List<IObserver<SkinData>> observers, IObserver<SkinData> observer)
    {
        this._observers = observers;
        this._observer = observer;
    }

    public void Dispose()
    {
        if (_observers.Contains(_observer))
            _observers.Remove(_observer);
    }
}
