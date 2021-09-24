using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Spine;
using UnityEngine.EventSystems;

public class Onboarding : Singleton<Onboarding>
{
    public List<GameObject> Rounds;
    private int indexRound = 0;

    public bool IsDone
    {
        get
        {
            Data.IdCheckUnlocked = "Onboarding" + Data.CurrentLevel;
            return Data.IsUnlocked;
        }

        set
        {
            Data.IdCheckUnlocked = "Onboarding" + Data.CurrentLevel;
            Data.IsUnlocked = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (IsDone)
        {
            Destroy(gameObject);
        }
        else
        {
            Rounds.ForEach(round => round.SetActive(false));
        }
    }

    private void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        GameController.Instance.IsOnboarding = true;
        Rounds[indexRound].SetActive(true);
    }

    public void EndRound()
    {
        GameController.Instance.IsOnboarding = false;
        Rounds[indexRound].SetActive(false);
        indexRound++;
        if (indexRound >= Rounds.Count)
        {
            IsDone = true;
            Destroy(gameObject);
        }
    }
}
