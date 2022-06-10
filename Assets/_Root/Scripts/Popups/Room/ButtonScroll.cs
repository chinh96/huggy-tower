using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScroll : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ScrollRectScript scrollRectScript;
    [SerializeField] private bool isRightButton;
    // Start is called before the first frame update
    public void OnPointerDown(PointerEventData eventData){
        if(isRightButton){
            scrollRectScript.ButtonRightIsPressed();
        }
        else{
            scrollRectScript.ButtonLeftIsPressed();
        }
    }
}
