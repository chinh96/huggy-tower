using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardTab : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite spriteActive;
    [SerializeField] private Sprite spriteDeactive;

    public void SetActive(bool active)
    {
        image.sprite = active ? spriteActive : spriteDeactive;
    }
}
