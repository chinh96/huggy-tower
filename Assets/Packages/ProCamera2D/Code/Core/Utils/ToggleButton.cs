using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private GameObject bg;
    private bool _isCheck;
    [SerializeField]
    public bool isCheck
    {
        get
        {
            return _isCheck;
        }
        set
        {
            _isCheck = value;
            setStateCheck();
        }
    }
    private void setStateCheck()
    {
        if (_isCheck)
        {
            bg.active = true;
        }
        else
        {
            bg.active = false;
        }
    }
}
