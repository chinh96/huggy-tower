using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Castle : MonoBehaviour
{
    [SerializeField] private Image progress;
    [SerializeField] private Image from;
    [SerializeField] private Image to;
    [SerializeField] private GameObject buildButton;
    [SerializeField] private GameObject upgradeButton;
    [SerializeField] private GameObject disableButton;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject doneIcon;
    [SerializeField] private List<GameObject> starActives;
    [SerializeField] private GameObject arrow;

    private int index;
    private CastleResources castle;
    private CastlePopup castlePopup;
    private CastleData castleData;
    private WorldResources worldCurrent;

    public void Init(int index, CastlePopup castlePopup, WorldResources worldCurrent)
    {
        HideAll();

        this.index = index;
        this.castlePopup = castlePopup;
        this.castle = worldCurrent.Castles[index];
        this.worldCurrent = worldCurrent;

        Reset();
    }

    private void Reset()
    {
        bool done = true;
        for (int i = 0; i < castle.Castles.Count; i++)
        {
            CastleData item = castle.Castles[i];
            if (!item.IsUnlocked)
            {
                castleData = item;
                done = false;
                cost.text = item.Cost.ToString();
                to.gameObject.SetActive(true);
                to.sprite = item.Sprite;
                progress.fillAmount = (float)i / (castle.Castles.Count - 1);

                if (Data.CoinTotal >= item.Cost)
                {
                    if (i == 0)
                    {
                        buildButton.SetActive(true);
                    }
                    else
                    {
                        arrow.SetActive(true);
                        from.gameObject.SetActive(true);
                        from.sprite = castle.Castles[i - 1].Sprite;
                        upgradeButton.SetActive(true);
                    }
                }
                else
                {
                    disableButton.SetActive(true);
                }
                break;
            }
            else
            {
                starActives[i].SetActive(true);
            }
        }

        if (done)
        {
            doneIcon.SetActive(true);
            to.gameObject.SetActive(true);
            to.sprite = castle.Castles[castle.Castles.Count - 1].Sprite;
        }
        else
        {
            right.SetActive(true);
        }

        from.SetNativeSize();
        to.SetNativeSize();

        if (Data.WorldCurrent == WorldType.Earth && index == 3)
        {
            from.transform.localScale = Vector3.one * .7f;
            to.transform.localScale = Vector3.one * .7f;

            to.transform.parent.GetComponent<HorizontalLayoutGroup>().spacing = -15;
        }
    }

    private void HideAll()
    {
        from.gameObject.SetActive(false);
        to.gameObject.SetActive(false);
        buildButton.SetActive(false);
        upgradeButton.SetActive(false);
        disableButton.SetActive(false);
        right.SetActive(false);
        doneIcon.SetActive(false);
        starActives.ForEach(item => item.SetActive(false));
        arrow.SetActive(false);
    }

    public void OnClickBuildOrUpgrade()
    {
        AnalyticController.BuildCastle();

        Data.CoinTotal -= castleData.Cost;
        castlePopup.Close();
        castle.BuildOrUpgrade();
        EventController.CastleBuilded(index);
        Reset();
        ResourcesController.Achievement.CheckCompleteCastle();
        ResourcesController.DailyQuest.CheckCompleteCastle();
    }
}
