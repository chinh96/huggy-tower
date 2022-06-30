using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollRectScript : MonoBehaviour
{
    private ScrollRect scrollRect;
    private bool mouseDown, buttonLeft, buttonRight;    

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void ButtonLeftIsPressed(){
        mouseDown = true;
        buttonLeft = true;
    }

    public void ButtonRightIsPressed(){
        mouseDown = true;
        buttonRight = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(mouseDown){
            if(buttonLeft) ScrollLeft();
            else ScrollRight();
        }
    }

    private void ScrollLeft(){
        if(Input.GetMouseButtonUp(0)){
            mouseDown = false;
            buttonLeft = false;
        }else{
            if(scrollRect.horizontalNormalizedPosition > 0)scrollRect.horizontalNormalizedPosition -= 0.02f;
        }
    }

    private void ScrollRight(){
        if(Input.GetMouseButtonUp(0)){
            mouseDown = false;
            buttonRight = false;
        }else{
            if(scrollRect.horizontalNormalizedPosition < 1) scrollRect.horizontalNormalizedPosition += 0.02f;
        }
    }

    public void FocusOnCurrentRoom(int idx){
        Debug.Log(scrollRect.horizontalNormalizedPosition);
        scrollRect.horizontalNormalizedPosition=(float)idx/(float)(scrollRect.content.transform.childCount-1);
    }
}
