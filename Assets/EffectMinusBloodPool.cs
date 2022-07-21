using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using DG.Tweening;
public class EffectMinusBloodPool : MonoBehaviour
{
    [SerializeField] private GameObject effectMinusBlood;
    private List<GameObject> pooler;
    // Start is called before the first frame update
    void Start()
    {
        pooler = new List<GameObject>();
        for (int i = 0; i < 15; i++)
        {
            GameObject _new = Instantiate(effectMinusBlood, transform);
            if (i < 5) _new.GetComponent<TextMeshProUGUI>().text = "-49";
            else if (i < 10) _new.GetComponent<TextMeshProUGUI>().text = "-50";
            else _new.GetComponent<TextMeshProUGUI>().text = "-51";

            pooler.Add(_new);
        }
    }

    public void GenerateEffect()
    {
        int random = Random.Range(1, 4);
        if (random == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                if (!pooler[i].activeSelf)
                {
                    pooler[i].SetActive(true);
                    break;
                }
            }
        }
        else if (random == 2)
        {
            for (int i = 5; i < 10; i++)
            {
                if (!pooler[i].activeSelf)
                {
                    pooler[i].SetActive(true);
                    break;
                }
            }
        }
        else
        {
            for (int i = 10; i < 15; i++)
            {
                if (!pooler[i].activeSelf)
                {
                    pooler[i].SetActive(true);
                    break;
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void HideAllEffect()
    {
        foreach (var effect in pooler) effect.SetActive(false);
    }
}
