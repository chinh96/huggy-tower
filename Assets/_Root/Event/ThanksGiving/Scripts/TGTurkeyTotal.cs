using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TGTurkeyTotal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTurkeyTotal;
    public GameObject Target;

    private void Awake()
    {
        if (TGDatas.IsInTG)
        {
            EventController.TurkeyTotalTextChanged += UpdateTurkeyText;
            UpdateTurkeyText();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdateTurkeyText()
    {
        textTurkeyTotal.text = TGDatas.TotalTurkeyText.ToString();
    }

    public void ShowTGPopup()
    {
        PopupController.Instance.Show<TGPopup>(null, ShowAction.DoNothing);
    }
}
